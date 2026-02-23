using Isometric_Game_Server.Games;
using Isometric_Game_Server.NetworkShared.Packets.ServerClient;
using Microsoft.Extensions.Logging;
using NetworkShared.Packets.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isometric_Game_Server.Matchmaking {
    public class Matchmaker {
        private readonly ILogger<Matchmaker> logger;
        private readonly GameManager gameManager;
        private readonly NetworkServer networkServer;
        private readonly ProblemCreator problemCreator;
        private List<MM_Request> matchMakingPool = new List<MM_Request>();

        public Matchmaker(ILogger<Matchmaker> logger,
                             GameManager gameManager,
                             NetworkServer networkServer,
                             ProblemCreator problemCreator) {
            this.logger = logger;
            this.gameManager = gameManager;
            this.networkServer = networkServer;
            this.problemCreator = problemCreator;
        }

        public void RegisterPlayerToPool(ServerConection serverConection, ushort playersCount) {
            if(!matchMakingPool.Exists(x => x.ServerConection.User.Id == serverConection.User.Id)) {
                matchMakingPool.Add(new MM_Request {
                    ServerConection = serverConection,
                    RequestTime = DateTime.UtcNow,
                    IsMatchFound = false,
                    PlayersCount = playersCount
                });
                logger.LogInformation($"Player {serverConection.User.Id} added to matchmaking pool.");

                SearchMatchWithStats(serverConection);
            }
            else {
                // Player is already in the matchmaking pool
                logger.LogInformation($"Player {serverConection.User.Id} is already in the matchmaking pool.");
            }
        }

        private void SearchMatchWithStats(ServerConection serverConection) {
            MM_Request requester = matchMakingPool.FirstOrDefault(x =>
                x.ServerConection.ConnectionId == serverConection.ConnectionId &&
                !x.IsMatchFound);

            if (requester == null)
                return;

            int targetCount = requester.PlayersCount;
            int requesterScore = requester.ServerConection.User.Score;

            // Find candidates with same requested player count
            var candidates = matchMakingPool
                .Where(x =>
                    !x.IsMatchFound &&
                    x.PlayersCount == targetCount &&
                    x.ServerConection.ConnectionId != serverConection.ConnectionId &&
                    Math.Abs(x.ServerConection.User.Score - requesterScore) <= 10
                )
                .OrderBy(x => Math.Abs(x.ServerConection.User.Score - requesterScore))
                .ToList();

            // Include requester
            List<MM_Request> matchGroup = new List<MM_Request> { 
                requester 
            };

            foreach (MM_Request candidate in candidates) {
                matchGroup.Add(candidate);

                if (matchGroup.Count == targetCount)
                    break;
            }

            if (matchGroup.Count < targetCount) {
                logger.LogInformation(
                    $"Waiting for more players ({matchGroup.Count}/{targetCount}) for player {requester.ServerConection.User.Id}"
                );
                return;
            }

            // 🎮 Match Found
            Guid gameId = Guid.NewGuid();

            foreach (var player in matchGroup) {
                player.IsMatchFound = true;
                player.ServerConection.GameId = gameId;
            }

            logger.LogInformation(
                $"Match found! Game {gameId} with {targetCount} players: {string.Join(", ", matchGroup.Select(p => p.ServerConection.User.Id))}"
            );

            //remove matched players from pool
            matchMakingPool.RemoveAll(x => matchGroup.Contains(x));

            // Start the game with the matched players
            StartGame(matchGroup.Select(p => p.ServerConection).ToList());

            INetPacket msg = new Net_OnStartGame {
                Players = matchGroup.Select(p => p.ServerConection.User.Id).ToArray(),
                GameId = gameId
            };

            Problem problem = problemCreator.CreateProblem(Complexity.Low);

            INetPacket msgProblem = new Net_OnGameQuestion {
                Id = (ushort)problem.Id,
                Complexity = problem.Complexity,
                Question = problem.Question,
                AnswerA = problem.AnswerA,
                AnswerB = problem.AnswerB,
                AnswerC = problem.AnswerC,
                AnswerD = problem.AnswerD,
                CorrectIndex = problem.CorrectIndex
            };

            //Send match found message to players
            foreach (MM_Request player in matchGroup) {
                // Here you would send a message to the player using their ServerConection.Peer
                networkServer.SendPacketToClient(msg, player.ServerConection.ConnectionId);
                networkServer.SendPacketToClient(msgProblem, player.ServerConection.ConnectionId);
            }
        }

        private void StartGame(List<ServerConection> serverConections) {
            string[] players = serverConections.Select(x => x.User.Id).ToArray();
            gameManager.RegisterGame(players);
        }

        public void UnregisterPlayer(string username) {
            var request = matchMakingPool.Find(x => x.ServerConection.User.Id == username);
            if (request != null) {
                matchMakingPool.Remove(request);
                logger.LogInformation($"Player {username} removed from matchmaking pool.");
            }
            else {
                logger.LogInformation($"Player {username} not found in matchmaking pool.");
            }
        }
    }
}
