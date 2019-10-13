using System;

namespace Services.Game
{
    public class GameJoined : IEvent<Game>
    {
        public Guid PlayerId { get; set; }
    }
}