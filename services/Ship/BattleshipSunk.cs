using System;
using MediatR;
using Redux;

namespace services.player.Events
{
    public class BattleshipSunk : INotification
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Battleship Battleship { get; set; }
    }
}