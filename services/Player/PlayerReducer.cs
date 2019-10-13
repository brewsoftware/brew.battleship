using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Services.Player
{
    public class PlayerReducer : INotificationHandler<IEvent<Player>>
    {
        private static IDictionary<Guid, List<IEvent<Player>>> Events { get; } = new Dictionary<Guid, List<IEvent<Player>>>();
        private Player _player { get; set; }
        private IMediator _mediator;
        public PlayerReducer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Apply(PlayerCreated action)
        {
            _player = new Player()
            {
                Id = action.Id,
                Name = action.Name
            };
        }
        
        public async Task Handle(IEvent<Player> @notification, CancellationToken cancellationToken)
        {
            if (!Events.ContainsKey(@notification.Id))
            {
                Events.Add(@notification.Id, new List<IEvent<Player>>());
            }
           
            // Validate event before adding
            if (@notification is IValidate<Player>)
            {
                if (!((IValidate<Player>) @notification).Validate(_player))
                {
                    Events[@notification.Id].Add(new PlayerError()
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

            foreach (var evt in Events[@notification.Id])
            {
                Apply((dynamic)@notification);
            }
        }
    }
 
}