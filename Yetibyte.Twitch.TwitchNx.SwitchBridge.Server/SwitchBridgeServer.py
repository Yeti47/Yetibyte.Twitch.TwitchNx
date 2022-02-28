import nxbt
import logging
import sys, getopt
import os
import asyncio

from dataclasses import dataclass, field
from dataclasses_json import config, dataclass_json
from websocket_server import WebsocketServer

@dataclass_json
@dataclass
class SwitchBridgeMessage:
    id: str = field(metadata=config(field_name='Id'))
    message_type: str = field(metadata=config(field_name='MessageType'))
    payload: dict = field(metadata=config(field_name='Payload'))
    is_error: bool = field(metadata=config(field_name='IsError'), default=False)
    error_message: str = field(metadata=config(field_name='ErrorMessage'), default='')
    error_code: str = field(metadata=config(field_name='ErrorCode'), default='')

class SwitchBridgeServer:

    ERROR_CODE_PAYLOAD_FORMAT = "PAYLOAD_FORMAT"
    ERROR_CODE_BAD_PAYLOAD = "BAD_PAYLOAD"
    ERROR_CODE_CONTROLLER_EXISTS = "CONTROLLER_EXISTS"
    ERROR_CODE_CONTROLLER_DOES_NOT_EXISTS = "CONTROLLER_DOES_NOT_EXIST"
    ERROR_CODE_INTERNAL = "INTERNAL"

    MSG_TYPE_GET_STATUS = "GET_STATUS"
    MSG_TYPE_CREATE_CONTROLLER = "CREATE_CONTROLLER"
    MSG_TYPE_REMOVE_CONTROLLER = "REMOVE_CONTROLLER"
    MSG_TYPE_GET_SWITCH_ADDRESSES = "GET_SWITCH_ADDRESSES"
    MSG_TYPE_MACRO = "MACRO"
    MSG_TYPE_MACRO_COMPLETE = "MACRO_COMPLETE"

    def __init__(self, port = 4769, logger: logging.Logger = None):
        self._logger = logger or logging.Logger("_NULL_LOGGER_")
        self._nxbt = nxbt.Nxbt()
        self._websocket_server = WebsocketServer(port=port, host='', loglevel=self._logger.level)
        self._websocket_server.set_fn_message_received(lambda client, server, message: self._on_message_received(client, message))

        self._message_processor_map = {
            SwitchBridgeServer.MSG_TYPE_GET_STATUS:           lambda client, message: self._process_get_status_message(client, message),
            SwitchBridgeServer.MSG_TYPE_CREATE_CONTROLLER:    lambda client, message: self._process_create_controller_message(client, message),
            SwitchBridgeServer.MSG_TYPE_REMOVE_CONTROLLER:    lambda client, message: self._process_remove_controller_message(client, message),
            SwitchBridgeServer.MSG_TYPE_GET_SWITCH_ADDRESSES: lambda client, message: self._process_get_switch_addresses_message(client, message),
            SwitchBridgeServer.MSG_TYPE_MACRO:                lambda client, message: asyncio.run(self._process_macro_message(client, message)),
        }


    def run_forever(self)->None:
        self._websocket_server.run_forever(False)


    def _on_message_received(self, client, message)->None:

        self._logger.info('SwitchBridgeServer: Received message')
        
        switch_bridge_msg = None

        try:
            switch_bridge_msg = SwitchBridgeMessage.from_json(message)
        except Exception as ex:
            self._logger.error(f'SwitchBridgeServer: Error deserializing message. Details: {ex}')

        if not switch_bridge_msg:
            self._logger.error(f'SwitchBridgeServer: Unable to process message "{message}".')
            return

        self._logger.info(f'SwitchBridgeServer: MessageType: {switch_bridge_msg.message_type} | MessageId: {switch_bridge_msg.id}')

        processor = self._message_processor_map.get(switch_bridge_msg.message_type, None)

        if not processor:
            self._logger.error(f'SwitchBridgeServer: Message type "switch_bridge_msg.message_type" cannot be processed.')
            return

        self._logger.info('SwitchBridgeServer: Processing message...')

        response_msg = processor(client, switch_bridge_msg)

        if response_msg:

            if response_msg.is_error:
                self._logger.error(f'SwitchBridgeServer: Request was processed with errors. Error message: {response_msg.error_message}')
            else:
                self._logger.info('SwitchBridgeServer: Processing complete.')

            response_json = response_msg.to_json()
            self._logger.info(f'SwitchBridgeServer: Sending response\n\t{response_json}')
            self._websocket_server.send_message(client, response_json)


    def _process_get_status_message(self, client, message)->SwitchBridgeMessage:
        nxbt_status = self._nxbt.state

        payload = { 'Status': 'OK', 'ControllerStates': [] }

        for k in nxbt_status.keys():
            payload['ControllerStates'].append({ 
                'Id': k, 
                'State': nxbt_status[k]['state'],
                'Type': str(nxbt_status[k]['type']).replace('ControllerTypes.', ''),
                'Errors': nxbt_status[k]['errors'] or '',
                'FinishedMacros': nxbt_status[k]['finished_macros']
            })

        response_msg = SwitchBridgeMessage(message.id, message.message_type, payload)

        return response_msg
         

    def _process_create_controller_message(self, client, message)->SwitchBridgeMessage:     
        
        controller_type = nxbt.PRO_CONTROLLER
        controller_type_str = "PRO_CONTROLLER"

        try:
            controller_type_str = message.payload.get("ControllerType", "PRO_CONTROLLER")

            if controller_type_str == "JOYCON_L":
                controller_type = nxbt.JOYCON_L
            elif controller_type_str == "JOYCON_R":
                controller_type = nxbt.JOYCON_R
            else:
                controller_type_str = "PRO_CONTROLLER"

        except Exception as ex:
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_PAYLOAD_FORMAT, 
                                       error_message=str(ex))

        controller_id = -1

        self._logger.info(f'SwitchBridgeServer: Creating controller {controller_type_str}...')

        try:
            controller_id = self._nxbt.create_controller(controller_type)
        except Exception as ex:
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_BAD_PAYLOAD,
                                       error_message=str(ex))

        self._logger.info('SwitchBridgeServer: Controller created.')

        return SwitchBridgeMessage(message.id, message.message_type, 
                                   { "ControllerId": controller_id, "ControllerType": controller_type_str })
            

    def _process_remove_controller_message(self, client, message)->SwitchBridgeMessage:
        controller_id = -1

        try:
            controller_id = message.payload['ControllerId']
        except Exception as ex:
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_PAYLOAD_FORMAT, 
                                       error_message=str(ex))

        if not self._has_controller(controller_id):
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_CONTROLLER_DOES_NOT_EXISTS,
                                       error_message=f'Controller with ID {controller_id} does not exist.')

        self._logger.info(f'SwitchBridgeServer: Removing controller {controller_id}...')

        try:
            self._nxbt.remove_controller(controller_id)
        except Exception as ex:
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_INTERNAL, 
                                       error_message=str(ex))
        
        self._logger.info('SwitchBridgeServer: Controller removed.')

        return SwitchBridgeMessage(message.id, message.message_type, {} )


    async def _process_macro_message(self, client, message)->SwitchBridgeMessage:
        
        macro = ''
        controller_id = -1
        macro_id = ''

        try:
            macro = message.payload['Macro']
            controller_id = message.payload['ControllerId']
        except Exception as ex:
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_PAYLOAD_FORMAT, 
                                       error_message=str(ex))

        if not self._has_controller(controller_id):
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_CONTROLLER_DOES_NOT_EXISTS,
                                       error_message=f'Controller with ID {controller_id} does not exist.')

        self._logger.info(f'SwitchBridgeServer: Executing macro for controller {controller_id}...')

        try:
            macro_id = self._nxbt.macro(controller_id, macro, block=False)

            send_macro_complete_task = asyncio.create_task(self._wait_for_macro_completion(client, macro_id, controller_id, message.id))
        except Exception as ex:
            return SwitchBridgeMessage(message.id, message.message_type, {}, 
                                       is_error=True, 
                                       error_code = SwitchBridgeServer.ERROR_CODE_INTERNAL,
                                       error_message=str(ex))

        self._logger.info('SwitchBridgeServer: Macro executed.')

        return SwitchBridgeMessage(message.id, message.message_type, { "MacroId": macro_id, "ControllerId": controller_id } )


    async def _wait_for_macro_completion(self, client, macro_id, controller_id, orig_msg_id)->None:

        self._logger.info("SwitchBridgeServer: Waiting for completion of macro '" + macro_id + "'...")

        try:
            while controller_id in self._nxbt.state and macro_id not in self._nxbt.state[controller_id]['finished_macros'] and self._nxbt.state[controller_id]['state'] == 'connected':
                asyncio.sleep(0)
        except Exception as ex:
            self._logger.error(f'SwitchBridgeServer: Error while waiting for macro completion: {ex}')

        self._logger.info("SwitchBridgeServer: Macro '" + macro_id + "' completed. Sending completion response.")
        self._send_macro_complete_message(client, macro_id, controller_id, orig_msg_id)


    def _send_macro_complete_message(self, client, macro_id, controller_id, orig_msg_id)->SwitchBridgeMessage:

        message_id = os.urandom(24).hex()

        message = SwitchBridgeMessage(message_id, SwitchBridgeServer.MSG_TYPE_MACRO_COMPLETE, { "MacroId": macro_id, "ControllerId": controller_id, "OriginalMessageId": orig_msg_id } )
        message_json = message.to_json()

        self._websocket_server.send_message(client, message_json)

        return message


    def _process_get_switch_addresses_message(self, client, message)->SwitchBridgeMessage:
        payload = { 'SwitchAddresses': self._nxbt.get_switch_addresses() }

        response_msg = SwitchBridgeMessage(message.id, message.message_type, payload)

        return response_msg

    def _has_controller(self, controller_id)->bool:
        return controller_id in self._nxbt.state


def main(argv):
    port = 4769

    try:
        opts, args = getopt.getopt(argv, 'p:', ['port='])
    except getopt.GetoptError:
        pass
    for opt, arg in opts:
        if opt in ('-p', '--port'):
            port = int(arg)

    logger = logging.Logger('SWITCH_BRIDGE_SERVER', logging.INFO)
    logger.addHandler(logging.StreamHandler())

    switch_bridge_server = SwitchBridgeServer(port=port, logger=logger)

    switch_bridge_server.run_forever()


if __name__ == '__main__':
    main(sys.argv[1:])