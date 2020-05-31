using System;
using System.Collections.Generic;
using System.Text;
using ComNet;
namespace GameServer
{
    public class GameRoom
    {
        public static int counter = 0;
        public int id { get; set; }
        public GameRoomInfo gameRoomInfo { get; set; }
        public List<Player> players { get; set; }

        public GameRoom(string name, int maxPlayers)
        {
            id = counter++;
            gameRoomInfo = new GameRoomInfo(name, maxPlayers);
            players = new List<Player>();
        }
        public void AddClient(Client client)
        {
            client.gameRoom = this;
            players.Add(new Player(client));
            gameRoomInfo.clientInfos.Add(client.clientInfo);

        }
        public void SetClientReady(int index)
        {
            Player player = null;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].client == ServerTCP.clients[index]) { player = players[i]; break; }
            }
            if (player == null) Console.WriteLine("No such client!");

            player.gameReady = true;

            bool allReady = true;
            foreach (Player p in players)
                if (!p.gameReady) { allReady = false; break; }

            if (allReady) AssignCharacters();
        }

        private void AssignCharacters()
        {
            foreach(Player player in players)
            {
                player.character = new Warrior();
                ServerTCP.SendObject(player.client.index, ServerPackets.SCharacterAssignment, player.character);
            }
        }

        public bool IsFull()
        {
            return players.Count >= gameRoomInfo.maxPlayers;
        }

        private class Player
        {
            public Client client;
            public bool gameReady = false;
            public Character character;
            public Player(Client client) { this.client = client; }
        }
    }
    //public class Client
    //{
    //    public Client(int clientIndex, string name)
    //    {
    //        this.clientIndex = clientIndex;
    //        this.name = name;
    //        this.isReady = false;
    //    }

    //    public int clientIndex { get; set; }
    //    public string name { get; set; }
    //    public Character character { get; set; }
    //    public bool isReady { get; set; }
    //} 
    
}