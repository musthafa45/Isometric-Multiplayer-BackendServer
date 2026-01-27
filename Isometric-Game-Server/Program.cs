using Isometric_Game_Server;
using Isometric_Game_Server.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

// See https://aka.ms/new-console-template for more information
//var server = new NetworkServer();
//server.Start();

var serviceProvider = Container.Configure();
var server = serviceProvider.GetRequiredService<NetworkServer>();


while (true)
{
    server.PollEvents();
    Thread.Sleep(16); // Approximate 60 updates per second
}
