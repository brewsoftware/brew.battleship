using System;
using Redux;

namespace services.player.Events
{
    
    public class GameCreated : IEvent<services.Game>
    {
        public override bool IsValid(services.Game state)
        {
            return true;
        }

        public Guid Author { get; set; }   
    }
}