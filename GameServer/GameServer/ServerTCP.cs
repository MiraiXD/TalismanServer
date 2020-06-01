using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using ComNet;
namespace GameServer
{
    class ServerTCP
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] buffer = new byte[1024];
        private static JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        public static Client[] clients = new Client[Constants.MAX_PLAYERS];
        public static List<GameRoom> gameRooms { get; private set; }
        public static void SetupServer()
        {
            gameRooms = new List<GameRoom>();
            gameRooms.Add(new TalismanRoom("11", 20));
            gameRooms.Add(new TalismanRoom("121", 2));
            gameRooms.Add(new TalismanRoom("11f", 2));
            gameRooms.Add(new TalismanRoom("1fsafas1", 2));
            gameRooms.Add(new TalismanRoom("1sa1", 2));
            gameRooms.Add(new TalismanRoom("1s1", 2));
            
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                clients[i] = new Client();
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
                if (clients[i].socket == null)
                {
                    clients[i].socket = socket;
                    clients[i].index = i;
                    clients[i].ip = socket.RemoteEndPoint.ToString();
                    clients[i].clientInfo = new ClientInfo("Client" + i.ToString());
                    clients[i].gameRoom = null;
                    clients[i].StartClient();
                    Console.WriteLine("Connection from '{0}' received", clients[i].ip);
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

            clients[index].socket.Send(sizeInfo);
            clients[index].socket.Send(data
);
        }

        public static void SendConnectionOK(int index)
        {           
            SendString(index, ServerPackets.SConnectionOK, "You are successfully connected to the server");
        }                
        public static void SendObject(int index, ServerPackets packetID, object obj = null)
        {
            SendString(index, packetID, JsonConvert.SerializeObject(obj, settings));
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
        public ClientInfo clientInfo;
        public GameRoom gameRoom = null;
        public bool gameReady = false;
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
