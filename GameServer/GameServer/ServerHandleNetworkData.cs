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
                { (int)ClientPackets.CCreateRoom, HandleCreateRoom}
            };
        }

        private static void HandleCreateRoom(int index, byte[] data)
        {
            PacketBuffer receivedBuffer = new PacketBuffer();
            receivedBuffer.WriteBytes(data);
            receivedBuffer.ReadInteger();
            string msg = receivedBuffer.ReadString();
            receivedBuffer.Dispose();
            Request.CreateRoom request = JsonConvert.DeserializeObject<Request.CreateRoom>(msg);
            ServerTCP.CreateRoom(request);
        }

        private static void HandleRequestRoomsList(int index, byte[] data)
        {
            //PacketBuffer buffer = new PacketBuffer();
            //buffer.WriteBytes(data);
            //buffer.ReadInteger();
            //string msg = buffer.ReadString();
            //buffer.Dispose();
            PacketBuffer sendBuffer = new PacketBuffer();
            sendBuffer.WriteInteger((int)ServerPackets.SReplyRoomsList);
            sendBuffer.WriteString(JsonConvert.SerializeObject(ServerTCP.gameRooms));
            
            // ADD YOUR CODE YOU WANT TO EXEC HERE
            ServerTCP.SendDataTo(index, sendBuffer.ToArray());
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
