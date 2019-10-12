using System;
using MediatR;

namespace services.player
{
    public abstract class IEvent<T> : INotification where T : IAggregate
    {
        public Guid Id { get; set; }
        public abstract  bool IsValid(T state);
    }

    public interface IAggregate
    {
        
    }
}