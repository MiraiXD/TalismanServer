using System;
using System.Collections.Generic;
using System.Text;

namespace ComNet
{
    public class MapTileInfo : IComparable<MapTileInfo>
    {
        private static int counter = 0;
        public enum MapTiles { City, Harbor, Tavern, Fields, Cave, Windmill, Graveyard, Forest}
        public MapTiles tileType;
        public int id;
        public MapTileInfo(MapTiles tileType)
        {
            id = counter++;
            this.tileType = tileType;
        }       

        public int CompareTo(MapTileInfo other)
        {
            if (this.id == other.id && this.tileType == other.tileType) return 0;
            else return -1;
        }
    }
    public class MapInfo : IComparable<MapInfo>
    {
        //public static MapInfo defaultMap;
        public MapTileInfo[] mapTiles;
        public MapInfo(MapTileInfo[] tiles)
        {
            mapTiles = tiles;
        }

        public int CompareTo(MapInfo other)
        {
            for (int i = 0; i < mapTiles.Length; i++)
            {
                if (mapTiles[i].CompareTo(other.mapTiles[i]) != 0) return -1;
            }
            return 0;
        }
    }
    public class RollInfo
    {
        public int rollResult;
    }
    public class PlayerInfo
    {
        public ClientInfo clientInfo;
        public int inRoomID;
        public bool isAdmin;
        public PlayerInfo(ClientInfo clientInfo, int roomID, bool isAdmin)
        {
            this.clientInfo = clientInfo;
            this.inRoomID = roomID;
            this.isAdmin = isAdmin;
        }
    }
    public class TalismanPlayerInfo
    {
        public CharacterInfo characterInfo;
        public int rerollsLeft;
        public TalismanPlayerInfo(CharacterInfo characterInfo, int maxRerolls)
        {
            this.characterInfo = characterInfo;
            this.rerollsLeft = maxRerolls;
        }
    }
    public class ClientInfo
    {
        public string name;        
        public ClientInfo(string name)
        {
            this.name = name;
        }
    }
    public class GameRoomInfo
    {
        public List<ClientInfo> clientInfos { get; set; }
        public string name { get; set; }
        public int maxPlayers { get; set; }
        public GameRoomInfo(string name, int maxPlayers)
        {
            this.name = name;
            this.maxPlayers = maxPlayers;
            clientInfos = new List<ClientInfo>();
        }
    }
    // move to TalismanComNet.dll
    public class CharacterInfo
    {
        public enum Characters { Barbarian, DarkKnight, Mage, Ogre, Marksman, Warlock, Assassin, Thief }
        
        public CharacterInfo(Characters character, MapTileInfo.MapTiles startingTileType, int maxHealth, int maxStrength, int maxPower)
        {
            this.character = character;
            this.startingTileType = startingTileType;
            this.maxHealth = maxHealth;
            this.maxStrength = maxStrength;
            this.maxPower = maxPower;

            currentHealth = maxHealth;
            currentStrength = maxStrength;
            currentPower = maxPower;
        }
        public Characters character;
        public MapTileInfo startingTile;
        public MapTileInfo.MapTiles startingTileType;
        public int maxHealth, currentHealth;
        public int maxStrength, currentStrength;
        public int maxPower, currentPower;
    }
    
}
