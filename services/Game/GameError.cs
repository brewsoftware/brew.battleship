using System;
using System.Collections.Generic;
using services.player;

namespace Services.Game
{
    public class GameError : IEvent<Game>
    {
        public List<string> ErrorMessages { get; set;  } = new List<string>();
        public Guid PlayerId { get; set; }

        public IEvent<Game> Event { get; set; }
    }
}