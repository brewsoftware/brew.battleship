using System;
using Redux;

namespace services.player.Events
{
    public class PlayerCreated : IEvent<Player>
    {
        public Guid Id { get; set; }
        public override bool IsValid(Player state)
        {
            return true;
        }

        public string Name { get; set; }
        public string email { get; set; }
    }
}