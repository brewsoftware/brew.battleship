using System;
using System.Dynamic;
using MediatR;

namespace services.player.Game
{
    public class GameLost : INotification
    {
        public Guid PlayerId { get; set; }
    }
}