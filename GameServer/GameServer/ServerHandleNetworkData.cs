using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ComNet;
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
                { (int)ClientPackets.CJoinRoom, HandleJoinRoom},
                { (int)ClientPackets.CGameReady, HandleGameReady}

            };
        }

        private static void HandleGameReady(int index, byte[] data)
        {
            ServerTCP.clients[index].gameRoom.SetClientReady(index);
        }        

        private static void HandleJoinRoom(int index, byte[] data)
        {
            ClientRequests.JoinRoom request = ServerTCP.GetData<ClientRequests.JoinRoom>(data);

            bool success = false;
            string message = "No such room";            
            foreach (GameRoom room in ServerTCP.gameRooms)
            {
                if (request.gameRoomInfo.name == room.gameRoomInfo.name)
                {
                    if (room.IsFull())
                    {
                        success = false;
                        message = "Room is full";
                    }
                    else
                    {
                        //Player player = new Player(index, request.playerName);
                        room.AddClient(ServerTCP.clients[index]);
                        //ServerTCP.clients[index].player = player;                        
                        success = true;
                    }                    
                    break;
                }
            }
            ServerResponds.RequestResult<ClientRequests.JoinRoom> result = new ServerResponds.RequestResult<ClientRequests.JoinRoom>(success, ClientPackets.CJoinRoom, ServerTCP.clients[index].gameRoom.gameRoomInfo, success ? null : message);
            //ServerTCP.SendString(index, ServerPackets.SRequestResult, JsonConvert.SerializeObject(result, settings));
            ServerTCP.SendObject(index, ServerPackets.SRequestResult, result);

            //ServerTCP.JoinRoom(index,request.playerName,request.gameRoom);
        }

        private static void HandleCreateRoom(int index, byte[] data)
        {            
            ClientRequests.CreateRoom request = ServerTCP.GetData<ClientRequests.CreateRoom>(data);
            //ServerTCP.CreateRoom(index,request.name, request.maxPlayers);
        }

        private static void HandleRequestRoomsList(int index, byte[] data)
        {
            ClientRequests.RoomsList request = ServerTCP.GetData<ClientRequests.RoomsList>(data);

            GameRoomInfo[] gameRoomInfos = new GameRoomInfo[ServerTCP.gameRooms.Count];
            for (int i = 0; i < ServerTCP.gameRooms.Count; i++) gameRoomInfos[i] = ServerTCP.gameRooms[i].gameRoomInfo;

            ServerResponds.RequestResult<ClientRequests.RoomsList> result = new ServerResponds.RequestResult<ClientRequests.RoomsList>(true, ClientPackets.CRequestRoomsList, gameRoomInfos);
            ServerTCP.SendObject(index, ServerPackets.SRequestResult, gameRoomInfos);
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
