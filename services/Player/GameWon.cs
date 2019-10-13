using System;
using MediatR;

namespace Services.Player
{
    public class GameWon : IEvent<Player>
    {
        public Guid GameId { get; set; }
    }
}