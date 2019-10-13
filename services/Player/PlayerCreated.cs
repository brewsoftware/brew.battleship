using System;

namespace Services.Player
{
    public class PlayerCreated : IEvent<Player>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string email { get; set; }
    }
}