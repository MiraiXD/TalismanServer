using System;
using System.Collections.Generic;
using System.Text;

namespace ComNet
{    
    public class ServerResponds
    {
        public abstract class RequestResult // needed for client side
        {
            public RequestResult(bool success, string message = null)
            {
                this.success = success;                
                this.message = message;                
            }
            public bool success { get; set; }            
            public string message { get; set; }            
        }
        public class RequestResult<T> : RequestResult
        {
            public T result;         
            //public bool success { get; set; }            
            //public string message { get; set; }
            public RequestResult(bool success, T result = default(T), string message = null) : base(success, message)
            {
              //  this.success = success;
               // this.message = message;
                this.result = result;
            }
        }

        public class JoinRoomResult
        {
            public GameRoomInfo joinedRoomInfo;
            public PlayerInfo newPlayerInfo;
        }
        public class RoomsListResult
        {
            public GameRoomInfo[] rooms;
        }
        public class RollDiceResult
        {
            public int diceCount;
            public int rollResult;
        }
        public class RandomCharacterResult
        {
            public CharacterInfo characterInfo;
            public int rerollsLeft;
        }
        public class CharactersAssigned
        {
            public List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
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
        public class RollDiceRequest
        {
            public int diceCount;         
        }
        public class RandomCharacter { }        
    }
}
