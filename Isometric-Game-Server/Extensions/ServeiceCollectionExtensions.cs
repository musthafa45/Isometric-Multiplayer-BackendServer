using Isometric_Game_Server.NetworkShared;
using Microsoft.Extensions.DependencyInjection;
using NetworkShared.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Isometric_Game_Server.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddPacketHandlers(this IServiceCollection services) {
            var handlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.DefinedTypes)
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(IPacketHandler).IsAssignableFrom(t))
                .Select(t => new {
                    Type = t.AsType(),
                    Attr = t.GetCustomAttribute<HandlerRegisterAttribute>()
                })
                .Where(x => x.Attr != null);

            foreach (var h in handlers) {
                services.AddScoped(h.Type);
            }

            return services;
        }
    }

}
