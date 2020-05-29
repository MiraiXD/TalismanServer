using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public class GameRoom
    {
        public int id { get; set; }
        public string name { get; set; }
        public int maxPlayers { get; set; }
        public List<Player> players { get; set; }

        public GameRoom(int id, string name, int maxPlayers)
        {
            this.id = id;
            this.name = name;
            this.maxPlayers = maxPlayers;
            players = new List<Player>();
        }
        public void AddPlayer(Player player)
        {
            players.Add(player);
        }
        public bool IsFull()
        {
            return players.Count >= maxPlayers;
        }
    }
    public class Player
    {
        public Player(int clientIndex, string name)
        {
            this.clientIndex = clientIndex;
            this.name = name;
        }

        public int clientIndex { get; set; }
        public string name { get; set; }
        public Character character { get; set; }
    }
    public class Character
    {
        public int health { get; set; }
        public int strength { get; set; }
        public int power { get; set; }

    }
}