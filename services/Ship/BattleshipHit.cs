using System;
using MediatR;
using Redux;
using services.player.Game;

namespace services.player.Events
{
    public class BattleshipHit : INotification
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Coordinate Shot { get; set; }
        public Battleship Battleship { get; set; }
    }
}