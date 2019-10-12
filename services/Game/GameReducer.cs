using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using services.player;
using services.player.Events;
using services.player.Game;

namespace services
{
    public class GameReducers: INotificationHandler<IEvent<Game>>, 
        IRequestHandler<GetGameState, Game>
    {
        
        private static IDictionary<Guid, List<IEvent<Game>>> Events { get; } = new Dictionary<Guid, List<IEvent<Game>>>();
        private Game _game { get; set; }
        private IMediator _mediator;
        public GameReducers(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public void Apply(GameCreated action)
        {
            _game = new Game()
            {
                Id = action.Id,
                PlayerA = action.Author,
                Status = Game.GameCreated
            };
        }

        public void Apply(GameJoined action)
        {
            _game.PlayerB = action.PlayerId;
        }

        public void Apply(GameEnded action)
        {
            _game.Status = Game.GameEnded; 
        }

        public void Apply(GameStarted action)
        {
            _game.Status = Game.GameStarted;
        }

        public void Apply(BattleshipPlaced battleship)
        {
            if (_game.PlayerA == battleship.PlayerId)
            {
                _game.ShipsA.Add(battleship.Battleship);
            }

            if (_game.PlayerB == battleship.PlayerId)
            {
                _game.ShipsB.Add(battleship.Battleship);
            }
        }

        private bool ProcessBattleship(Coordinate coordinate, Battleship battleship)
        {
            if (battleship.GetCoords().Exists(x => x.X == coordinate.X && x.Y == coordinate.Y))
            {
                battleship.Hits.Add(coordinate);
                return true;
            }

            return false;
        }
        
        public void Apply(ShotFired shot)
        {
            if (_game.PlayerA == shot.PlayerId)
            {
                foreach (var ship in _game.ShipsB)
                {
                    if (ProcessBattleship(shot.Coordinate, ship))
                    {
                        _mediator.Publish(new BattleshipHit()
                        {
                            Battleship = ship,
                            Shot = shot.Coordinate,
                            GameId = _game.Id,
                            PlayerId = _game.PlayerA
                        });
                    } else {
                        _mediator.Publish(new BattleshipMissed()
                        {
                            Shot = shot.Coordinate,
                            GameId = _game.Id,
                            PlayerId = _game.PlayerA
                        });
                    };
                }
            }
            if (_game.PlayerB == shot.PlayerId)
            {
                foreach (var ship in _game.ShipsA)
                {
                    if (ProcessBattleship(shot.Coordinate, ship))
                    {
                        _mediator.Publish(new BattleshipHit()
                        {
                            Battleship = ship,
                            Shot = shot.Coordinate,
                            GameId = _game.Id,
                            PlayerId = _game.PlayerB
                        });
                    }
                    else
                    {
                        _mediator.Publish(new BattleshipMissed()
                        {
                            Shot = shot.Coordinate,
                            GameId = _game.Id,
                            PlayerId = _game.PlayerB
                        });
                    };
                }
            }
        }

        public async Task Handle(IEvent<Game> @notification, CancellationToken cancellationToken)
        {
            if (!Events.ContainsKey(@notification.Id))
            {
                Events.Add(@notification.Id, new List<IEvent<Game>>());
            }
            
            foreach (var evt in Events[@notification.Id])
            {
                Apply((dynamic)evt);
            }
            
            // Validate event before adding
            if (!@notification.IsValid(_game))
            {
                return;
            }
            
            // Note: DB Insert at this stage. Hydrate if needed.
            Events[@notification.Id].Add(@notification);
            Apply((dynamic)@notification);
           
        }

        public async Task<Game> Handle(GetGameState request, CancellationToken cancellationToken)
        {
            // Note: DB Insert at this stage. Hydrate 
            foreach (var evt in Events[request.GameId])
            {
                Apply((dynamic)evt);
            }

            return _game;
        }
    }
}