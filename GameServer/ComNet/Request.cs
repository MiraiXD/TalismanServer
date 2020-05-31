using System;
using System.Collections.Generic;
using System.Text;

namespace ComNet
{    
    public class ServerResponds
    { 
        public class RequestResult
        {
            public RequestResult(bool success, object obj = null, string message = null)
            {
                this.success = success;
               // this.packetID = packetID;
                this.message = message;
                this.obj = obj;
            }
            public bool success { get; set; }
            //public ClientPackets packetID { get; set; }
            public string message { get; set; }
            public object obj { get; set; }
        }
        public class RequestResult<T> : RequestResult
        {
            public RequestResult(bool success, ClientPackets packetID, object obj = null, string message = null) : base(success, obj, message)
            {
            }
        }
    }    
    public class ClientRequests
    {     
        public class CreateRoom
        {
            public CreateRoom(string name, int maxPlayers)
            {
                gameRoomInfo = new GameRoomInfo(name, maxPlayers);
            }
            public GameRoomInfo gameRoomInfo; 

        }       
        public class JoinRoom
        {
            public JoinRoom(GameRoomInfo gameRoomInfo)
            {
                this.gameRoomInfo = gameRoomInfo;                
            }

            public GameRoomInfo gameRoomInfo { get; set; }            
        }
        public class RoomsList { }
    }
}
