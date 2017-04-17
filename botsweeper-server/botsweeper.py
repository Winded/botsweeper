from argparse import ArgumentParser
import socket
import uuid
from select import select
from time import sleep
import json
import traceback

class MessageType(object):
    LIST_GAMES_REQ = 1
    LIST_GAMES_RES = 2

    CREATE_GAME_REQ = 3
    CREATE_GAME_RES = 4

    REMOVE_GAME_REQ = 5
    REMOVE_GAME_RES = 6

    ENDPOINT_MESSAGE = 7
    ENDPOINT_MESSAGE_ERROR = 8

class Client(object):
    def __init__(self, id, endpoint, socket):
        self.id = id
        self.address = endpoint[0]
        self.port = endpoint[1]
        self.socket = socket

    def disconnect(self):
        if self.socket:
            self.socket.close()
            self.socket = None

    def to_dict(self):
        return {"id": self.id.hex}

class Game(object):
    def __init__(self, id, name, client_id):
        self.id = id
        self.name = name
        self.client_id = client_id

    def to_dict(self):
        return {"id": self.id.hex, "name": self.name, "client_id": self.client_id.hex}

class MasterServer(object):
    def __init__(self, address, port):
        self.address = address
        self.port = port

        self.clients = []
        self.games = []

    def init(self):
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.bind((self.address, self.port))
        self.socket.listen(1)

    def update(self):
        rs, ws, xs = select([self.socket], [], [], 0)
        if len(rs) > 0:
            ep = self.socket.accept()
            c = Client(uuid.uuid4(), ep[1], ep[0])
            print("Client %s:%i connected" % (c.address, c.port))
            self.clients.append(c)

        rs, ws, xs = select(map(lambda c: c.socket, self.clients), [], [], 0)
        for c in self.clients:
            if c.socket not in rs:
                continue

            data = c.socket.recv(4096)
            if data == "":
                # Empty message = Client disconnected
                print("Client %s:%i disconnected" % (c.address, c.port))
                c.disconnect()
                self.remove_client_game(c)
                self.clients.remove(c)
                continue

            jdata = None
            try:
                jdata = json.loads(data)
            except ValueError:
                print("JSON parse failure")
                continue

            try:
                jresp = self.process_message(c, jdata)
                if jresp:
                    c.socket.sendall(json.dumps(jresp))
            except:
                print("Error processing message: %s" % data)

    def cleanup(self):
        self.socket.close()
        for c in self.clients:
            c.disconnect()

    def run(self):
        try:
            self.init()
            while True:
                self.update()
                sleep(0.1)
        except KeyboardInterrupt, SystemExit:
            pass
        finally:
            self.cleanup()

    def process_message(self, client, data):
        if "type" not in data:
            print("JSON msg type not present")
            return None

        type = data["type"]
        if type == MessageType.LIST_GAMES_REQ:
            return self.list_games()
        elif type == MessageType.CREATE_GAME_REQ:
            return self.create_game(client, data)
        elif type == MessageType.REMOVE_GAME_REQ:
            return self.remove_game(client)
        elif type == MessageType.ENDPOINT_MESSAGE:
            return self.endpoint_message(client, data)
        else:
            print("Unknown message type")
            return None

    def remove_client_game(self, client):
        for g in self.games:
            if g.client_id == client.id:
                print("Removed game: %s" % g.id.hex)
                self.games.remove(g)
                return True
        return False

    def list_games(self):
        r = {"type": MessageType.LIST_GAMES_RES}
        l = []
        for g in self.games:
            l.append(g.to_dict())
        r["games"] = l
        return r

    def create_game(self, client, data):
        r = {"type": MessageType.CREATE_GAME_RES}
        name = data["name"]
        g = Game(uuid.uuid4(), name, client.id)
        self.games.append(g)
        r["game"] = g.to_dict()
        r["success"] = True
        return r

    def remove_game(self, client):
        removed = self.remove_client_game(client)
        if removed:
            return {"type": MessageType.REMOVE_GAME_RES, "success": True}
        else:
            return {"type": MessageType.REMOVE_GAME_RES, "success": False}

    def endpoint_message(self, client, data):
        data["from_id"] = client.id.hex
        to_id = data["to_id"]
        r = {"type": MessageType.ENDPOINT_MESSAGE, "from_id": client.id.hex, "to_id": data["to_id"], "message": data["message"]}
        for c in self.clients:
            if c.id.hex == to_id:
                c.socket.sendall(json.dumps(r))
                return None
        return {"type": MessageType.ENDPOINT_MESSAGE_ERROR, "reason": "Client %s not found" % to_id}

if __name__ == "__main__":
    
    argp = ArgumentParser(description="Botsweeper Server")
    argp.add_argument("-a", "--address", type=str, help="Address in which the server listens on", default="0.0.0.0")
    argp.add_argument("-p", "--port", type=int, help="Port in which the server listens on", default=8080)

    r = argp.parse_args()

    print("Listening on %s:%i" % (r.address, r.port))
    s = MasterServer(r.address, r.port)
    s.run()
    print("\n")