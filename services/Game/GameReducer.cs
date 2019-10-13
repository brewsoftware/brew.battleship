using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Services.Ship;

namespace Services.Game
{
    public class GameReducers : INotificationHandler<IEvent<Game>>,
        IRequestHandler<GetGameState, Game>
    {
        private static IDictionary<Guid, List<IEvent<Game>>> Events { get; } =
            new Dictionary<Guid, List<IEvent<Game>>>();

        private Game _game { get; set; }
   
        public void Apply(GameCreated action)
        {
            _game = new Game()
            {
                Id = action.Id,
                PlayerA = action.Author,
                Status = Game.GameCreated
            };
        }

        public void Apply(GameErrorsViewed action)
        {
            _game.Errors.Clear();
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

        public void Apply(GameError action)
        {
            _game.Errors.AddRange(action.ErrorMessages);
        }

        public void Apply(BattleshipPlaced battleship)
        {
            battleship.Battleship.Hits = new List<Coordinate>();

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
                if (battleship.Hits.Where(x => x.X == coordinate.X 
                                               && x.Y == coordinate.Y).Count() == 0)
                {
                    battleship.Hits.Add(coordinate);
                    return true;
                }
            
            }
            return false;
        }

        public async Task Apply(ShotFired shot)
        {
            if (_game.Status == Game.GameCreated)
            {
                _game.Status = Game.GameStarted;
            }

            List<Battleship> targets = null;
            if (_game.PlayerA == shot.PlayerId)
            {
                targets = _game.ShipsB;
            }
            else
            {
                targets = _game.ShipsA;
            }

            int shipsSunk = 0;
            foreach (var ship in targets)
            { 
                if (ProcessBattleship(shot.Coordinate, ship))
                {
                    if (ship.Length == ship.Hits.Count())
                    {
                        shipsSunk++;
                    }
                }
            }

            if (shipsSunk == targets.Count())
            {
                _game.Status = Game.GameEnded;
                _game.Winner = shot.PlayerId;
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
            if (@notification is IValidate<Game>)
            {
                if (!((IValidate<Game>) @notification).Validate(_game))
                {
                    Events[@notification.Id].Add(new GameError()
                    {
                        Event = @notification,
                        Id = @notification.Id,
                        ErrorMessages = @notification.ErrorMessages
                    });
                    
                    return;
                }
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