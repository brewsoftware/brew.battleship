using System;
using System.Collections.Generic;
using Services.Ship;

namespace Services.Game
{
    public class Game : IAggregate
    {

        public const string GameCreated = "GameCreated";
        public const string GameStarted = "GameStarted";
        public const string GameEnded = "GameEnded";
        public List<string> Errors { get; } = new List<string>();
        public Guid Id { get; set; }
        public Guid PlayerA { get; set; }
        public Guid PlayerB { get; set; }
        public bool AllShipsHit { get; set; } = false;
        public string Status { get; set; }
        public List<Battleship> ShipsA { get; } = new List<Battleship>();
        public List<Battleship> ShipsB { get; } = new List<Battleship>();
        
        public Guid Winner { get; set; }
        
    }
}