using System;
using System.Collections.Generic;
using Services;

namespace Services.Player
{
    public class Player : IAggregate
    {
        
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}