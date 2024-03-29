using System;
using MediatR;

namespace Services.Game
{
    public class GetGameState : IRequest<Game>
    {
        public Guid GameId { get; set; }
    }
}