using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    // gets sent from server to client
    public enum ServerPackets
    {
        SConnectionOK = 1,
        SReplyRoomsList = 2
    }
    // gets sent from client to server
    public enum ClientPackets
    {
        CRequestRoomsList = 1,
        CCreateRoom = 2
    }
}
