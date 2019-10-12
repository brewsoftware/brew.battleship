using System;
using System.Collections.Generic;
using services.player;

namespace services
{
    public class Game : IAggregate
    {

        public const string GameCreated = "GameCreated";
        public const string GameStarted = "GameStarted";
        public const string GameEnded = "GameEnded";
        public Guid Id { get; set; }
        public Guid PlayerA { get; set; }
        public Guid PlayerB { get; set; }
        
        public string Status { get; set; }
        public List<Battleship> ShipsA { get; } = new List<Battleship>();
        public List<Battleship> ShipsB { get; } = new List<Battleship>();
        
    }
}