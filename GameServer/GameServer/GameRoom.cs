using System;
using System.Collections.Generic;
using System.Text;
using ComNet;
namespace GameServer
{
    public class TalismanRoom : GameRoom
    {
        private Player currentPlayerMoving;
        private MapInfo mapInfo;

        public class TalismanPlayer : Player
        {
            public override PlayerInfo playerInfo { get => _playerInfo; set => _playerInfo = (TalismanPlayerInfo) value; }
            private TalismanPlayerInfo _playerInfo;
            public Character character { get { return _character; } set { _character = value; _playerInfo.characterInfo = _character.characterInfo; } }
            private Character _character;
            public TalismanPlayer(Client client, int roomID, bool isAdmin) : base(client, roomID, isAdmin) { }            
        }
        public TalismanRoom(string name, int maxPlayers) : base(name, maxPlayers) { }
                
        public override void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Network Packages");
            receivers = new Dictionary<int, ClientMessageReceiver>()
            {               
                { (int)ClientPackets.CGameReady, HandleGameReady},
                { (int)ClientPackets.CAdminMapInfo, HandleMapInfo},
            };
        }

        private void HandleMapInfo(int index, byte[] data)
        {
            try
            {
                mapInfo = ServerTCP.GetData<MapInfo>(data);
            }
            catch { return; }
            SendToAll(ServerPackets.SMapInfo, mapInfo);
        }

        public override PlayerInfo AddClient(Client client, bool isAdmin)
        {
            base.AddClient(client, isAdmin);
            TalismanPlayer newPlayer = new TalismanPlayer(client, players.Count, isAdmin);
            players.Add(newPlayer);
            return newPlayer.playerInfo;
        }
        protected override void Play()
        {
            AssignCharacters();
            SetMovingPlayer(players[0]);
        }

        private void AssignCharacters()
        {
            foreach (TalismanPlayer player in players)
            {
                player.character = new Warrior();
                ServerTCP.SendObject(player.client.index, ServerPackets.SCharacterAssignment, player.character.characterInfo);
            }
        }
        private void SetMovingPlayer(Player player)
        {
            currentPlayerMoving = player;
            SendToAll(ServerPackets.PlayerTurn, currentPlayerMoving.playerInfo);
        }


        private void HandleGameReady(int index, byte[] data)
        {
            SetPlayerReady(index);
        }
    }

    public abstract class GameRoom
    {
        private static int counter = 0;
        public int id { get; set; }
        public GameRoomInfo gameRoomInfo { get; set; }
        public List<Player> players { get; set; }

        protected Dictionary<int, ClientMessageReceiver> receivers;
        public abstract void InitializeNetworkPackages();
        protected abstract void Play();

        public GameRoom(string name, int maxPlayers)
        {
            id = counter++;
            gameRoomInfo = new GameRoomInfo(name, maxPlayers);
            players = new List<Player>();
            InitializeNetworkPackages();
        }
        protected void SendToAll(ServerPackets packetID, object obj = null)
        {
            for(int i=0; i<players.Count; i++)
            {
                ServerTCP.SendObject(players[i].client.index, packetID, obj);
            }
        }
        public virtual PlayerInfo AddClient(Client client, bool isAdmin)
        {
            client.gameRoom = this;            
            gameRoomInfo.clientInfos.Add(client.clientInfo);
            return null;

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

            if (allReady) Play();
        }

        

        public bool IsFull()
        {
            return players.Count >= gameRoomInfo.maxPlayers;
        }

        public abstract class Player
        {
            public abstract PlayerInfo playerInfo { get; set; }            
            public Client client;
            public bool gameReady = false;            
            public Player(Client client, int roomID, bool isAdmin) { this.client = client; this.playerInfo = new PlayerInfo(roomID, isAdmin); }
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