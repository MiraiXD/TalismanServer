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
        public List<int> clients { get; set; }

        public GameRoom(int id, string name, int maxPlayers)
        {
            this.id = id;
            this.name = name;
            this.maxPlayers = maxPlayers;
            clients = new List<int>();
        }
        public void AddClient(int index)
        {
            clients.Add(index);
        }
    }

}