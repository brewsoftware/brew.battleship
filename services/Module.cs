using System;
using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using services;
using Services;
using Services.Game;
using Services.Player;

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
                            x.FullName.Contains("Services")
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
            services.RegisterGenericHandlers(typeof(IEvent<Game>), typeof(GameReducers));
            //services.AddTransient<IRequestHandler<IRequest<GetGameState>>, GameReducers>();
            return services;
        }
    }
}