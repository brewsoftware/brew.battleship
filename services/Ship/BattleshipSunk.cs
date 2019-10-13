using System;
using MediatR;

namespace Services.Ship
{
    public class BattleshipSunk : INotification
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Battleship Battleship { get; set; }
    }
}