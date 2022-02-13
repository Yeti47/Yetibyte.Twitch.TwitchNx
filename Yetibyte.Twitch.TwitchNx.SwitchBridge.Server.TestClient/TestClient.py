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

    client_websocket.send(message_json)

    response_json = client_websocket.recv()

    print("Status Received: " + response_json)

    response = SwitchBridgeMessage.from_json(response_json)

    print("Press any key to quit.")
    


if __name__ == "__main__":
    main()