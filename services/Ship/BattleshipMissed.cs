using System;
using MediatR;
using Services.Game;

namespace Services.Ship
{
    public class BattleshipMissed : INotification
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Coordinate Shot { get; set; }
    }
}