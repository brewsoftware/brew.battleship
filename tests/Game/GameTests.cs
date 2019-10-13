using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Services.Game;
using services.player;
using Services.Player;
using Services.Ship;
using services.tests.EventStates;

namespace services.tests
{
    public class GameTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task EnsureGameCreated()
        {
            // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);

            // Assert
            var playerState = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });
            Assert.AreEqual(Game.GameCreated,playerState.Status);
            Assert.AreEqual(GameStates.PlayerAGuid, playerState.PlayerA );
            Assert.AreEqual( GameStates.PlayerBGuid, playerState.PlayerB);

            Assert.IsNotEmpty(notifications);
        }


        [Test]
        public async Task EnsureShipsCantOverlap()
        {  // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);
            await GameStates.GivenSomeShips(_mediator);
            
            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });
            Assert.AreEqual(2, _game.Errors.Count);
            Assert.AreEqual(1, _game.ShipsB.Count);
            Assert.AreEqual(1, _game.ShipsA.Count);
            
            Assert.IsNotEmpty(notifications);
        }

        [Test]
        public async Task EnsureErrorsCanBeCleared()
        {
            // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);
            await GameStates.GivenSomeShips(_mediator);

            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });
            Assert.AreEqual(2, _game.Errors.Count);
            Assert.AreEqual(1, _game.ShipsB.Count);
            Assert.AreEqual(1, _game.ShipsA.Count);

            // clear errors
            await _mediator.Publish(new GameErrorsViewed()
            {
                Id = GameStates.GameGuid
            });
            _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });
            Assert.AreEqual(0, _game.Errors.Count);

            Assert.IsNotEmpty(notifications);
        }
        
        [Test]
        public async Task EnsureCanAddMoreThanOneShipPerPlayer()
        {
            // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);
           
            await _mediator.Publish(new BattleshipPlaced()
            {
                Id = GameStates.GameGuid, 
                PlayerId = GameStates.PlayerBGuid,
                Battleship = new Battleship()
                {
                    Id = Guid.NewGuid(),
                    Length = 3,
                    X = 4,
                    Y = 3,
                    IsVertical = true,
                    PlayerId = GameStates.PlayerBGuid
                }
            });
            
            // Assert
            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });

            Assert.AreEqual(0, _game.Errors.Count);
            Assert.AreEqual(2, _game.ShipsB.Count );
            Assert.AreEqual(1, _game.ShipsA.Count );
            Assert.IsNotEmpty(notifications);
        }
        
        [Test]
        public async Task EnsureGameCanBeWon()
        {
            // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);

            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 3 }
            });
            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 4 }
            });
            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 5 }
            });
            
            // Assert
            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });

            Assert.AreEqual(0, _game.Errors.Count);
            Assert.AreEqual(1, _game.ShipsB.Count );
            Assert.AreEqual(1, _game.ShipsA.Count );
            Assert.AreEqual(Game.GameEnded, _game.Status);
            Assert.AreEqual(GameStates.PlayerAGuid, _game.Winner);
        }
        
        [Test]
        public async Task EnsureShotsArentDoubleRegistered()
        {
            // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);
            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 3 }
            });
            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 4 }
            });
            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 4 }
            });
            
            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });

            Assert.AreEqual(0, _game.Errors.Count);
            Assert.AreEqual(2, _game.ShipsB.First().Hits.Count());
            Assert.AreEqual(Game.GameStarted, _game.Status);
        }
        
        [Test]
        public async Task EnsureShotIsReported()
        {
            // Arrange
            var notifications = new List<INotification>();
            var serviceProvider = new ServiceCollection()
                .CanProcessGames()
                .CanProcessPlayers()
                .CanPerformBasicOperations()
                .WithHandler<INotification>((x) => notifications.Add(x))
                .BuildServiceProvider();

            var _mediator = serviceProvider.GetService<IMediator>();

            // Act
            await GameStates.GivenABasicGame(_mediator);
            await GameStates.GivenSomeShips(_mediator);
            Guid shotId = Guid.NewGuid();
            await _mediator.Publish(new ShotFired()
            {
                Id = GameStates.GameGuid, 
                PlayerId = GameStates.PlayerAGuid,
                Coordinate = new Coordinate() { X = 3, Y = 3, ShotId = shotId}
            });
           
            
            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });

            Assert.AreEqual(0, _game.Errors.Count);
            Assert.AreEqual(1, _game.ShipsB.First().Hits.Count());
            Assert.AreEqual(shotId, _game.ShipsB.First().Hits.First().ShotId);
            Assert.AreEqual(Game.GameStarted, _game.Status);
        }
    }
}