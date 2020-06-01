using System;
using System.Collections.Generic;
using System.Text;
using ComNet;
namespace GameServer
{
    public class TalismanRoom : GameRoom
    {
        public class TalismanPlayer : Player
        {
            public Character character;
            public TalismanPlayer(Client client) : base(client) { }            
        }
        public TalismanRoom(string name, int maxPlayers) : base(name, maxPlayers) { }
                
        public override void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Network Packages");
            receivers = new Dictionary<int, ClientMessageReceiver>()
            {               
                { (int)ClientPackets.CGameReady, HandleGameReady}
            };
        }
        public override void AddClient(Client client)
        {
            base.AddClient(client);
            players.Add(new TalismanPlayer(client));
        }
        protected override void AllReady()
        {
            AssignCharacters();
        }

        private void AssignCharacters()
        {
            foreach (TalismanPlayer player in players)
            {
                player.character = new Warrior();
                ServerTCP.SendObject(player.client.index, ServerPackets.SCharacterAssignment, player.character.characterInfo);
            }
        }



        private void HandleGameReady(int index, byte[] data)
        {
            SetPlayerReady(index);
        }
    }

    public abstract class GameRoom
    {
        public static int counter = 0;
        public int id { get; set; }
        public GameRoomInfo gameRoomInfo { get; set; }
        public List<Player> players { get; set; }

        protected Dictionary<int, ClientMessageReceiver> receivers;
        public abstract void InitializeNetworkPackages();
        protected abstract void AllReady();

        public GameRoom(string name, int maxPlayers)
        {
            id = counter++;
            gameRoomInfo = new GameRoomInfo(name, maxPlayers);
            players = new List<Player>();
            InitializeNetworkPackages();
        }
        public virtual void AddClient(Client client)
        {
            client.gameRoom = this;            
            gameRoomInfo.clientInfos.Add(client.clientInfo);

        }
        public void SetPlayerReady(int index)
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

            if (allReady) AllReady();
        }

        

        public bool IsFull()
        {
            return players.Count >= gameRoomInfo.maxPlayers;
        }

        public class Player
        {
            public Client client;
            public bool gameReady = false;            
            public Player(Client client) { this.client = client; }
        }

        public void HandleClientMessage(int packetID, int index, byte[] data)
        {
            if(receivers.TryGetValue(packetID, out ClientMessageReceiver receiver))
            {
                receiver.Invoke(index, data);
            }
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