using System;
using Redux;

namespace services.player.Events
{
    public class GameJoined : IEvent<services.Game>
    {
        public Guid PlayerId { get; set; }

        public override bool IsValid(services.Game state)
        {
            return true;
        }
    }
}