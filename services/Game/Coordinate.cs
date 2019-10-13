using System;

namespace Services.Game
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Guid? ShotId { get; set; }
    }
}