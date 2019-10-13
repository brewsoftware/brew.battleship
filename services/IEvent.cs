using System;
using System.Collections.Generic;
using MediatR;

namespace Services
{
    public abstract class IEvent<T> : INotification where T : IAggregate
    {
        public Guid Id { get; set; }
        
        public List<String> ErrorMessages { get; } = new List<string>();
    }

    public interface IAggregate
    {
        
    }
}