using System;
using System.Collections.Generic;
using Services;

namespace Services.Player
{
    public class PlayerError: IEvent<Player>
    {
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public IEvent<Player> Event { get; set; }
        
    }
}