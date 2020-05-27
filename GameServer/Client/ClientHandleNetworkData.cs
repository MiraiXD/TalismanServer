using System;
using System.Collections.Generic;
using Bindings;

namespace Client
{
    class ClientHandleNetworkData
    {
        private delegate void Packet_(byte[] data);
        private static Dictionary<int, Packet_> packets;

        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Network Packages");
            packets = new Dictionary<int, Packet_>()
            {
                { (int)ServerPackets.SConnectionOK, HandleConnectionOK}
            };
        }

        public static void HandleNetworkInformation(byte[] data)
        {            
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            int packetNumber = buffer.ReadInteger();
            buffer.Dispose();
            if(packets.TryGetValue(packetNumber, out Packet_ Packet))
            {
                Packet.Invoke(data);
            }
        }

        private static void HandleConnectionOK(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            // ADD YOUR CODE YOU WANT TO EXEC HERE
            Console.WriteLine(msg);

            ClientTCP.ThankYouServer();
        }
    }
}
