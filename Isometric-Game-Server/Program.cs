using Isometric_Game_Server;
using Isometric_Game_Server.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;


IServiceProvider serviceProvider = Container.Configure();
NetworkServer server = serviceProvider.GetRequiredService<NetworkServer>();
server.Start();


while (true)
{
    server.PollEvents();
    Thread.Sleep(16); // Approximate 60 updates per second
}
