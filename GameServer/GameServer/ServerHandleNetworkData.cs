using System;
using System.Collections.Generic;
using Bindings;
using Newtonsoft.Json;
namespace GameServer
{
    class ServerHandleNetworkData
    {
        private delegate void Packet_(int index, byte[] data);
        private static Dictionary<int, Packet_> packets;

        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Network Packages");
            packets = new Dictionary<int, Packet_>()
            {
                { (int)ClientPackets.CRequestRoomsList, HandleRequestRoomsList},
                { (int)ClientPackets.CCreateRoom, HandleCreateRoom},
                { (int)ClientPackets.CJoinRoom, HandleJoinRoom}
            };
        }

        private static void HandleJoinRoom(int index, byte[] data)
        {
            ClientRequests.JoinRoom request = ServerTCP.GetData<ClientRequests.JoinRoom>(data);
            ServerTCP.JoinRoom(index,request.playerName,request.gameRoom);
        }

        private static void HandleCreateRoom(int index, byte[] data)
        {
            //PacketBuffer receivedBuffer = new PacketBuffer();
            //receivedBuffer.WriteBytes(data);
            //receivedBuffer.ReadInteger();
            //string msg = receivedBuffer.ReadString();
            //receivedBuffer.Dispose();
            //string msg = ServerTCP.GetString(data);
            //Request.CreateRoom request = JsonConvert.DeserializeObject<Request.CreateRoom>(msg);
            ClientRequests.CreateRoom request = ServerTCP.GetData<ClientRequests.CreateRoom>(data);
            ServerTCP.CreateRoom(index,request.name, request.maxPlayers);
        }

        private static void HandleRequestRoomsList(int index, byte[] data)
        {
            ServerTCP.SendRoomsList(index);           
            Console.WriteLine("Sending rooms list");
        }

        public static void HandleNetworkInformation(int index, byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInteger();
            buffer.Dispose();
            if (packets.TryGetValue(packetID, out Packet_ Packet))
            {
                Packet.Invoke(index, data);
            }
        }
    }
}
