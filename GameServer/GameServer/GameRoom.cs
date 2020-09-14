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
            public bool characterAccepted = false;
            public override PlayerInfo playerInfo { get => _playerInfo; set => _playerInfo = (TalismanPlayerInfo)value; }
            private TalismanPlayerInfo _playerInfo;
            public Character character { get { return _character; } set { _character = value; _playerInfo.characterInfo = _character.characterInfo; } }
            private Character _character;
            public TalismanPlayer(Client client, int roomID, bool isAdmin) : base(client, roomID, isAdmin)
            {
                playerInfo = new TalismanPlayerInfo(roomID, isAdmin, null);
            }
        }
        public TalismanRoom(string name, int maxPlayers) : base(name, maxPlayers) { }

        public override void InitializeNetworkPackages()
        {
            Console.WriteLine("TalismanRoom - Initialize Network Packages");
            receivers = new Dictionary<int, ClientMessageReceiver>()
            {
                { (int)ClientPackets.CGameReady, HandleGameReady},
                { (int)ClientPackets.CAdminMapInfo, HandleMapInfo},
                { (int)ClientPackets.CCharacterAccepted, HandleCharacterAccepted},
                { (int)ClientPackets.CRoll, HandleRoll},
            };
        }

        private void HandleRoll(int index, byte[] data)
        {
            TalismanPlayer player = (TalismanPlayer)GetPlayerByIndex(index);
            ClientRequests.RollDiceRequest request = ServerTCP.GetData<ClientRequests.RollDiceRequest>(data);
            int rollResult = new Random().Next(1, request.diceCount * 6 + 1);
            SendToAll(ServerPackets.SRequestResult, new ServerResponds.RollDiceResult() { diceCount = request.diceCount, rollResult = rollResult });
        }

        private void HandleCharacterAccepted(int index, byte[] data)
        {
            TalismanPlayer player = (TalismanPlayer) GetPlayerByIndex(index);

            player.characterAccepted = true;

            bool allReady = true;
            foreach (TalismanPlayer p in players)
                if (!p.characterAccepted) { allReady = false; break; }

            if (allReady) SetMovingPlayer(players[0]);
        }

        private void HandleMapInfo(int index, byte[] data)
        {
            mapInfo = ServerTCP.GetData<MapInfo>(data);
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
            SendToAll(ServerPackets.SGameReady);
            //AssignCharacters();
            //SetMovingPlayer(players[0]);
        }

        private void AssignCharacters()
        {
            TalismanPlayerInfo[] talismanPlayerInfos = new TalismanPlayerInfo[players.Count];
            List<Character> characters = new List<Character> { new Warrior(), new Warrior(), new Warrior(), new Warrior(), new Warrior(), new Warrior(), new Warrior(), new Warrior(), new Warrior(), new Warrior() };
            for (int i = 0; i < players.Count; i++)
            {
                TalismanPlayer player = (TalismanPlayer)players[i];
                int rand = new Random().Next(0, characters.Count);
                Character character = characters[rand];
                characters.RemoveAt(rand);

                character.characterInfo.startingTile = GetTile(mapInfo, character.characterInfo.startingTileType);
                player.character = character;
                talismanPlayerInfos[i] = (TalismanPlayerInfo)player.playerInfo;
            }
            Console.WriteLine("Assigning characters");
            SendToAll(ServerPackets.SCharacterAssignment, talismanPlayerInfos);
        }
        private MapTileInfo GetTile(MapInfo mapInfo, MapTileInfo.MapTiles tileType)
        {
            foreach (MapTileInfo tileInfo in mapInfo.mapTiles)
            {
                if (tileInfo.tileType == tileType)
                    return tileInfo;
            }
            return null;
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

        public override Player GetPlayerByIndex(int index)
        {
            return (TalismanPlayer) base.GetPlayerByIndex(index);
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
            for (int i = 0; i < players.Count; i++)
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
        public virtual Player GetPlayerByIndex(int index)
        {
            Player player = null;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].client == ServerTCP.clients[index]) { player = players[i]; break; }
            }
            if (player == null) { Console.WriteLine("No such client!"); return null; }
            return player;
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
            public Player(Client client, int roomID, bool isAdmin)
            {
                this.client = client;
                //this.playerInfo = new PlayerInfo(roomID, isAdmin);
            }
        }

        public void HandleClientMessage(int packetID, int index, byte[] data)
        {
            if (receivers.TryGetValue(packetID, out ClientMessageReceiver receiver))
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