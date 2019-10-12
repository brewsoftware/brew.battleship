using System;
using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using services;
using services.player.Game;

namespace services.player
{
    public static class Module
    {
        public static IServiceCollection CanPerformBasicOperations(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddMediatR(
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x =>
                        (
                            x.FullName.Contains("services")
                        )
                    ).ToArray()
            );
            return services;
        }
        public static IServiceCollection CanProcessPlayers(this IServiceCollection services)
        {
            services.RegisterGenericHandlers(typeof(IEvent<Player>), typeof(PlayerReducer));
            return services;
        }

        public static IServiceCollection CanProcessGames(this IServiceCollection services)
        {
            services.RegisterGenericHandlers(typeof(IEvent<services.Game>), typeof(GameReducers));
            //services.AddTransient<IRequestHandler<IRequest<GetGameState>>, GameReducers>();
            return services;
        }
    }
}