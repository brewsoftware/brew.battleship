using System;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace services
{
    public static class TypeHelpers {

        public static bool IsConcrete(this Type type) {
            if (!type.GetTypeInfo().IsAbstract)
                return !type.GetTypeInfo().IsInterface;
            return false;
        }

        public static void RegisterGenericHandlers(this IServiceCollection services, Type type, Type handler) {
            var assTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsConcrete());

            foreach (var t in assTypes) {
                var genericBase = typeof(INotificationHandler<>);
                var combinedType = genericBase.MakeGenericType(t);
                services.AddTransient(combinedType, handler);
            }
        }
    }
}