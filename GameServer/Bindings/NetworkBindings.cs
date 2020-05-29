using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    // gets sent from server to client
    public enum ServerPackets
    {
        SConnectionOK,
        SReplyRoomsList,        
        SRequestResult
    }
    // gets sent from client to server
    public enum ClientPackets
    {
        CRequestRoomsList,
        CCreateRoom,
        CJoinRoom
    }
}
