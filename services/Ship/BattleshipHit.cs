using System;
using MediatR;
using Services.Game;

namespace Services.Ship
{
    public class BattleshipHit : INotification
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Coordinate Shot { get; set; }
        public Battleship Battleship { get; set; }
    }
}