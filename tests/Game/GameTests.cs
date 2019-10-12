using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using services.player;
using services.player.Events;
using services.player.Game;
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
            var playerState = await _mediator.Send(new GetGameState());
            Assert.Equals(Game.GameCreated,playerState.Status);
            Assert.Equals(GameStates.PlayerAGuid, playerState.PlayerA );
            Assert.Equals( GameStates.PlayerBGuid, playerState.PlayerB);

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
            
            Assert.AreEqual(1, _game.ShipsB.Count);
            Assert.AreEqual(1, _game.ShipsA.Count);
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
            
            // Assert
            var _game = await _mediator.Send(new GetGameState()
            {
                GameId = GameStates.GameGuid
            });
            
            Assert.AreEqual(_game.ShipsB.Count, 1);
            Assert.AreEqual(_game.ShipsA.Count, 1);
            Assert.IsNotEmpty(notifications);
            await _mediator.Publish(new BattleshipPlaced()
            {
                Id = GameStates.GameGuid, 
                PlayerId = GameStates.PlayerBGuid,
                Battleship = new Battleship()
                {
                    Id = Guid.NewGuid(),
                    Length = 3,
                    X = 3,
                    Y = 4,
                    IsVertical = false,
                    PlayerId = GameStates.PlayerBGuid
                }
            });
            
            // Assert
            Assert.AreEqual(2, _game.ShipsB.Count );
            Assert.AreEqual(1, _game.ShipsA.Count );
            Assert.IsNotEmpty(notifications);
        }

        [Test]
        public void EnsureCantAddShipsOutsideBoard()
        {
        }
        
        public void EnsureGameCanBeWon()
        {
            
        }
    }
}