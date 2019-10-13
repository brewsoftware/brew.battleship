using System;
using services.player;

namespace Services.Game
{
    
    public class GameCreated : IEvent<Game>
    {

        public Guid Author { get; set; }   
    }
}