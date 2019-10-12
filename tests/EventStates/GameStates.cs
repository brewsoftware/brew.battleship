using System;
using System.Threading.Tasks;
using MediatR;
using services.player.Events;
using services.player.Game;

namespace services.tests.EventStates
{
    public class GameStates
    {
        public static Guid PlayerAGuid = Guid.NewGuid();
        public static Guid PlayerBGuid = Guid.NewGuid();
        public static Guid GameGuid = Guid.NewGuid();
        public static Guid ShipA = Guid.NewGuid();
        public static Guid ShipB = Guid.NewGuid();

        public static async Task GivenABasicGame(IMediator _mediator)
        {
            await _mediator.Publish(new PlayerCreated() {Id = PlayerAGuid, Name = "PlayerA"});

            await _mediator.Publish(new PlayerCreated() {Id = PlayerBGuid, Name = "PlayerB"});

            await _mediator.Publish(new GameCreated()
            {
                Author = PlayerAGuid,
                Id = GameGuid
            });
            await _mediator.Publish(new GameJoined()
            {
                PlayerId = PlayerBGuid,
                Id = GameGuid
            });
            
        }

        public static async Task GivenSomeShips(IMediator _mediator)
        {
            await _mediator.Publish(new BattleshipPlaced()
            {
                Id = GameGuid, PlayerId = PlayerAGuid,
                Battleship = new Battleship()
                {
                    Id = ShipA,
                    Length = 3,
                    X = 3,
                    Y = 3,
                    IsVertical = false,
                    PlayerId = PlayerAGuid
                }
            });

            await _mediator.Publish(new BattleshipPlaced()
            {
                Id = GameGuid, PlayerId = PlayerBGuid,
                Battleship = new Battleship()
                {
                    Id = ShipB,
                    Length = 3,
                    X = 3,
                    Y = 3,
                    IsVertical = false,
                    PlayerId = PlayerBGuid
                }
            });
        
        }
    }
}