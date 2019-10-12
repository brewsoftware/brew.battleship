using System;
using MediatR;

namespace services.player.Game
{
    public class GameWon : INotification
    {
        public Guid PlayerId { get; set; }
    }
}