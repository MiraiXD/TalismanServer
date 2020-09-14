using System;
using System.Collections.Generic;
using System.Text;

namespace ComNet
{
    public class Constants
    {
        public const int MAX_PLAYERS = 100;
    }
    // gets sent from server to client
    public enum ServerPackets
    {
        SConnectionOK,
        SReplyRoomsList,
        SRequestResult,
        SMapInfo,
        SCharacterAssignment,
        PlayerTurn,
    }
    // gets sent from client to server
    public enum ClientPackets
    {
        CRequestRoomsList,
        CCreateRoom,
        CJoinRoom,
        CGameReady,
        CAdminMapInfo,
        CCharacterAccepted,
        CRoll,
    }
}
