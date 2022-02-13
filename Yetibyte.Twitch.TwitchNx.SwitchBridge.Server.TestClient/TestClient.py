import websocket
import logging
import json

from dataclasses import dataclass, field
from dataclasses_json import config, dataclass_json

@dataclass_json
@dataclass
class SwitchBridgeMessage:
    id: str = field(metadata=config(field_name='Id'))
    message_type: str = field(metadata=config(field_name='MessageType'))
    payload: dict = field(metadata=config(field_name='Payload'))


def main():

    address = input("Please enter the server's address (including the port):")

    client_websocket = websocket.WebSocket()
    client_websocket.connect(address)

    message = SwitchBridgeMessage("12345", "GET_STATUS", {})
    message_json = message.to_json()

    print("Sending GET_STATUS request...")

    client_websocket.send(message_json)

    response_json = client_websocket.recv()

    print("Status Received: " + response_json)

    contr_message = SwitchBridgeMessage("12346", "CREATE_CONTROLLER", { "ControllerType": "PRO_CONTROLLER" })
    contr_message_json = contr_message.to_json()

    print("Sending CREATE_CONTROLLER request...")

    client_websocket.send(contr_message_json)

    response_json = client_websocket.recv()

    print("CreateController Response Received: " + response_json)

    message = SwitchBridgeMessage("12347", "GET_STATUS", {})
    message_json = message.to_json()

    print("Sending GET_STATUS request...")

    client_websocket.send(message_json)

    response_json = client_websocket.recv()

    print("Status Received: " + response_json)

    addr_message = SwitchBridgeMessage("12348", "GET_SWITCH_ADDRESSES", {})
    addr_message_json = addr_message.to_json()

    print("Sending GET_SWITCH_ADDRESSES request...")

    client_websocket.send(addr_message_json)

    response_json = client_websocket.recv()

    print("Switch Addresses Received: " + response_json)

    print("Press any key to quit.")
    


if __name__ == "__main__":
    main()