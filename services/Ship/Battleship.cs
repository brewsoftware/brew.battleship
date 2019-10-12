using System;
using System.Collections.Generic;
using services.player;
using services.player.Game;

namespace services
{
    public class Battleship : IAggregate
    {
        public int Length { get; set; }   
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public bool IsVertical { get; set; }
        public List<Coordinate> Hits { get; } = new List<Coordinate>();
        
        public List<Coordinate> GetCoords(){
            var coords = new List<Coordinate>();
            for (var i = 0; i < Length; i++)
            {
                coords.Add(new Coordinate()
                {
                    X = X  + (!IsVertical? i:0),
                    Y = Y + (IsVertical? i:0)
                        
                });
            }

            return coords;
        }
    }
}