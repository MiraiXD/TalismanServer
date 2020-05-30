using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Bindings;
using Newtonsoft.Json;
namespace GameServer
{
    class ServerTCP
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] buffer = new byte[1024];
        private static JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        private static Client[] _clients = new Client[Constants.MAX_PLAYERS];
        public static List<GameRoom> gameRooms { get; private set; }
        public static void SetupServer()
        {
            gameRooms = new List<GameRoom>();
            gameRooms.Add(new GameRoom(0, "11", 20));
            gameRooms.Add(new GameRoom(6, "121", 2));
            gameRooms.Add(new GameRoom(1, "11f", 2));
            gameRooms.Add(new GameRoom(6, "1fsafas1", 2));
            gameRooms.Add(new GameRoom(2, "1sa1", 2));
            gameRooms.Add(new GameRoom(3, "1s1", 2));
            
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                _clients[i] = new Client();
            }
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (_clients[i].socket == null)
                {
                    _clients[i].socket = socket;
                    _clients[i].index = i;
                    _clients[i].ip = socket.RemoteEndPoint.ToString();
                    _clients[i].StartClient();
                    Console.WriteLine("Connection from '{0}' received", _clients[i].ip);
                    SendConnectionOK(i);
                    return;
                }
            }
        }

        public static void SendDataTo(int index, byte[] data)
        {
            byte[] sizeInfo = new byte[4];
            sizeInfo[0] = (byte)data.Length;
            sizeInfo[1] = (byte)(data.Length >> 8);
            sizeInfo[2] = (byte)(data.Length >> 16);
            sizeInfo[3] = (byte)(data.Length >> 24);

            _clients[index].socket.Send(sizeInfo);
            _clients[index].socket.Send(data
);
        }

        public static void SendConnectionOK(int index)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ServerPackets.SConnectionOK);
            buffer.WriteString("You are successfully connected to the server");
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }
        public static void SendRoomsList(int index)
        {
            SendString(index, ServerPackets.SReplyRoomsList, JsonConvert.SerializeObject(gameRooms, settings));
        }
        public static void CreateRoom(int index, string name, int maxPlayers)
        {
            
        }
        public static void JoinRoom(int index, string playerName,GameRoom gameRoom)
        {
            bool success = false;
            string message = "No such room";
            GameRoom room = null;
            foreach (GameRoom r in gameRooms)
            {
                if (gameRoom.name == r.name)
                {                    
                    if (r.IsFull())
                    {
                        success = false;
                        message = "Room is full";
                    }
                    else
                    {                        
                        r.AddPlayer(new Player(index, playerName));
                        success = true;
                    }
                    room = r;
                    break;
                }
            }
            ServerResponds.RequestResult<ClientRequests.JoinRoom> result = new ServerResponds.RequestResult<ClientRequests.JoinRoom>(success, ClientPackets.CJoinRoom, success ? null : message, room);
            SendString(index, ServerPackets.SRequestResult, JsonConvert.SerializeObject(result, settings));
        }
        public static void SendString(int index, ServerPackets packetID, string msg = null)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)packetID);
            if (msg != null)
                buffer.WriteString(msg);

            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }
        public static string GetString(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            return msg;
        }
        public static T GetData<T>(byte[] data)
        {
            string msg = GetString(data);
            T obj = default;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
                obj = JsonConvert.DeserializeObject<T>(msg, settings);
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            return obj;
        }
    }

    public class Client
    {
        public int index;
        public string ip;
        public Socket socket;
        public bool closing = false;
        private byte[] _buffer = new byte[1024];

        public void StartClient()
        {
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            try
            {
                int received = socket.EndReceive(ar);
                if (received <= 0)
                {
                    CloseClient(index);
                }
                else
                {
                    byte[] dataBuffer = new byte[received];
                    Array.Copy(_buffer, dataBuffer, received);
                    ServerHandleNetworkData.HandleNetworkInformation(index, dataBuffer);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
            }
            catch
            {
                CloseClient(index);
            }
        }

        private void CloseClient(int index)
        {
            closing = true;
            Console.WriteLine("Connection from {0} has been terminated", ip);
            socket.Close();
        }
    }

}
