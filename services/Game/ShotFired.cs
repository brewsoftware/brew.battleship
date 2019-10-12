using System;
using Redux;
using services.player.Game;

namespace services.player.Events
{
    public class ShotFired : IEvent<services.Game>
    {
        public Guid PlayerId { get; set; }
        public Coordinate Coordinate { get; set; }
        public override bool IsValid(services.Game state)
        {
            return true;
        }
    }
}