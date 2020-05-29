using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public class ServerResponds
    {
        public class RequestResult
        {
            public RequestResult(bool success, ClientPackets packetID, string message = null, object obj = null)
            {
                this.success = success;
                this.packetID = packetID;
                this.message = message;
                this.obj = obj;
            }
            public bool success { get; set; }
            public ClientPackets packetID { get; set; }
            public string message { get; set; }
            public object obj { get; set; }
        }
        public class RequestResult<T> : RequestResult
        {
            public T request;
            public RequestResult(T request, bool success, ClientPackets packetID, string message = null, object obj = null) : base(success, packetID, message, obj)
            {
                this.request = request;
            }
        }
    }
    public class ClientRequests
    {
        public class CreateRoom
        {
            public CreateRoom(string name, int maxPlayers)
            {
                this.name = name;
                this.maxPlayers = maxPlayers;
            }

            public string name { get; set; }
            public int maxPlayers { get; set; }

        }
        public class JoinRoom
        {
            public JoinRoom(GameRoom gameRoom, string playerName)
            {
                this.gameRoom = gameRoom;
                this.playerName = playerName;
            }

            public GameRoom gameRoom { get; set; }
            public string playerName { get; set; }

        }
    }
}
