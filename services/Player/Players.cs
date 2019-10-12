using System;
using System.Collections.Generic;

namespace services.player
{
    public class Player : IAggregate
    {
        
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}