using System;
using System.Collections.Generic;
using System.Text;

namespace ComNet
{
    public class PlayerInfo
    {
        public int roomID;
        public PlayerInfo(int roomID)
        {
            this.roomID = roomID;
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
        public enum Characters { Warrior, Mage }
        
        public CharacterInfo(Characters character, int maxHealth, int maxStrength, int maxPower)
        {
            this.character = character;
            this.maxHealth = maxHealth;
            this.maxStrength = maxStrength;
            this.maxPower = maxPower;

            currentHealth = maxHealth;
            currentStrength = maxStrength;
            currentPower = maxPower;
        }
        public Characters character;
        public int maxHealth, currentHealth;
        public int maxStrength, currentStrength;
        public int maxPower, currentPower;
    }

}
