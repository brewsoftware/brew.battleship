using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using services;
namespace services.tests
{
    public class DelegatNotificationHandlerExtension<TA> : INotificationHandler<TA> where TA : INotification {
        private Action<TA> handler;

        public DelegatNotificationHandlerExtension(Action<TA> delegatFunc) {
            handler = delegatFunc;
        }

        public Task Handle(TA request, CancellationToken cancellationToken) {
            handler(request);
            return Task.CompletedTask;
        }
    }

    public class DelegateHandlerExtension<TA, TB> : IRequestHandler<TA, TB> where TA : IRequest<TB> {
        private Func<TA, TB> handler;

        public DelegateHandlerExtension(Func<TA, TB> delegatFunc) {
            handler = delegatFunc;
        }

        public Task<TB> Handle(TA request, CancellationToken cancellationToken) {
            return Task.FromResult(handler(request));
        }
    }
    public static class Extensions
    {
        //_fixture.ServiceProvider.ReturnsForAll<IRequestHandler<GetPerson, Person>>(new PersonHandler());
        //public static void WithRequestHandler<TA, TB>(this IServiceProvider provider, Func<TA, TB> handler) where TA : IRequest<TB> {
        //    var h = new DelegateHandlerExtension<TA, TB>(handler);
        //    provider.WithNotificationHandler<IRequestHandler<TA,TB>>(typeof(IRequestHandler<TA, TB>), h);
        //}

        public static IServiceCollection WithHandler<TA>(this IServiceCollection services, Action<TA> handler) 
            where TA : INotification {
            var assTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(TA).IsAssignableFrom(p) && p.IsConcrete());

            foreach (var t in assTypes) {
                var genericBase = typeof(INotificationHandler<>);
                var combinedType = genericBase.MakeGenericType(t);
                var h = new DelegatNotificationHandlerExtension<TA>(handler);
                services.AddSingleton(combinedType, h);
            }

            return services;
        }
    }
}