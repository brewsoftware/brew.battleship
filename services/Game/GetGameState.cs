using System;
using MediatR;

namespace services.player.Game
{
    public class GetGameState : IRequest<services.Game>
    {
        public Guid GameId { get; set; }
    }
}