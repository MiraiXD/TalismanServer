using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Bindings;
namespace GameServer
{
    class ServerTCP
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] buffer = new byte[1024];

        private static Client[] _clients = new Client[Constants.MAX_PLAYERS];
        public static List<GameRoom> gameRooms { get; private set; }
        public static void SetupServer()
        {            
            gameRooms = new List<GameRoom>();            
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

        public static void CreateRoom(Request.CreateRoom request)
        {

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
