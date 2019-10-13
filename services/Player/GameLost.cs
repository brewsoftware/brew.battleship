using System;
using MediatR;

namespace Services.Player
{
    public class GameLost : IEvent<Player>
    {
     public Guid GameId { get; set; }
    }
}