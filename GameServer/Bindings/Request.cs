using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public class Request
    {
        public class CreateRoom
        {
            public CreateRoom(string name, int maxPlayers)
            {
                this.name = name;
                this.maxPlayers = maxPlayers;
            }

            public string name { get; set; }
            public int maxPlayers {get; set;}
            
        }

    }
}
