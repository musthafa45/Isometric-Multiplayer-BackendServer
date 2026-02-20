using Isometric_Game_Server.Data;
using Isometric_Game_Server.Extensions;
using Isometric_Game_Server.Games;
using NetworkShared.Registries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Isometric_Game_Server.Matchmaking;

namespace Isometric_Game_Server.Infrastructure {
    public static class Container {
        public static IServiceProvider Configure() {
            var services = new ServiceCollection();

            ConfigureServices(services);

            return services.BuildServiceProvider();

        }

        private static void ConfigureServices(IServiceCollection services) {
            services.AddLogging(c => c.AddSimpleConsole());
            services.AddSingleton<NetworkServer>();
            services.AddSingleton<PacketRegistry>();
            services.AddSingleton<HandlerRegistry>();
            services.AddSingleton<IUserRepository,InMemoryUserRepository>();
            services.AddSingleton<UsersManager>();
            services.AddSingleton<Matchmaker>();
            services.AddSingleton<GameManager>();
            services.AddPacketHandlers();
        }
    }
}
