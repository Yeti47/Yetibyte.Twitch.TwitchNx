import nxbt
import logging
import sys, getopt

from dataclasses import dataclass, field
from dataclasses_json import config, dataclass_json
from websocket_server import WebsocketServer

@dataclass_json
@dataclass
class SwitchBridgeMessage:
    id: str = field(metadata=config(field_name='Id'))
    message_type: str = field(metadata=config(field_name='MessageType'))
    payload: dict = field(metadata=config(field_name='Payload'))

class SwitchBridgeServer:

    MSG_TYPE_CONNECT = "CONNECT"
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
        self._websocket_server.set_fn_message_received = (lambda client, server, message: self._on_message_received(client, message))

        self._message_processor_map = {
            SwitchBridgeServer.MSG_TYPE_GET_STATUS:           lambda client, message: self._process_get_status_message(client, message),
            SwitchBridgeServer.MSG_TYPE_CREATE_CONTROLLER:    lambda client, message: self._process_create_controller_message(client, message),
            SwitchBridgeServer.MSG_TYPE_REMOVE_CONTROLLER:    lambda client, message: self._process_remove_controller_message(client, message),
            SwitchBridgeServer.MSG_TYPE_CONNECT:              lambda client, message: self._process_connect_message(client, message),
            SwitchBridgeServer.MSG_TYPE_GET_SWITCH_ADDRESSES: lambda client, message: self._process_get_switch_addresses_message(client, message),
            SwitchBridgeServer.MSG_TYPE_MACRO:                lambda client, message: self._process_macro_message(client, message),
        }


    def run_forever(self)->None:
        self._websocket_server.run_forever()


    def _on_message_received(self, client, message)->None:
        
        switch_bridge_msg = None

        try:
            switch_bridge_msg = SwitchBridgeMessage.from_json(message)
        except Exception as ex:
            self._logger.error(f'SwitchBridgeServer: Error deserializing message. Details: {ex}')

        if not switch_bridge_msg:
            self._logger.error(f'SwitchBridgeServer: Unable to process message "{message}".')
            return

        processor = self._message_processor_map.get(switch_bridge_msg.message_type, None)

        if not processor:
            self._logger.error(f'SwitchBridgeServer: Message type "switch_bridge_msg.message_type" cannot be processed.')
            return

        response_msg = processor(client, switch_bridge_msg)

        if response_msg:
            response_json = response_msg.to_json()
            self._websocket_server.send_message(client, response_json)


    def _process_get_status_message(self, client, message)->SwitchBridgeMessage:
        nxbt_status = self._nxbt.status

        payload = { 'Status': 'OK', 'ControllerStates': [] }

        for k in nxbt_status.keys():
            payload['ControllerStates'].append({ 
                'Id': k, 
                'State': nxbt_status[k]['state'],
                'Type': nxbt_status[k]['type'],
                'IsError': nxbt_status[k]['errors'],
                'FinishedMacros': nxbt_status[k]['finished_macros']
            })

        response_msg = SwitchBridgeMessage(message.id, message.message_type, payload)

        return response_msg
         

    def _process_create_controller_message(self, client, message)->SwitchBridgeMessage:
        pass


    def _process_remove_controller_message(self, client, message)->SwitchBridgeMessage:
        pass


    def _process_connect_message(self, client, message)->SwitchBridgeMessage:
        pass


    def _process_macro_message(self, client, message)->SwitchBridgeMessage:
        pass


    def _process_get_switch_addresses_message(self, client, message)->SwitchBridgeMessage:
        payload = { 'SwitchAddresses': self._nxbt.get_switch_addresses() }

        response_msg = SwitchBridgeMessage(message.id, message.message_type, payload)

        return response_msg


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

    switch_bridge_server = SwitchBridgeServer(port=port, logger=logger)

    switch_bridge_server.run_forever()


if __name__ == '__main__':

    main(sys.argv[1:])