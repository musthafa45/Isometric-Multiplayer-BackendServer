using Isometric_Game_Server.Games;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isometric_Game_Server.Matchmaking {
    public class Matchmaker {
        private readonly ILogger<Matchmaker> logger;
        List<MM_Request> matchMakingPool = new List<MM_Request>();

        public Matchmaker(ILogger<Matchmaker> logger) {
            this.logger = logger;
        }

        public void RegisterPlayerToPool(ServerConection serverConection) {
            if(!matchMakingPool.Exists(x => x.ServerConection.User.Id == serverConection.User.Id)) {
                matchMakingPool.Add(new MM_Request {
                    ServerConection = serverConection,
                    RequestTime = DateTime.UtcNow,
                    IsMatchFound = false
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

            int requesterScore = serverConection.User.Score;

            // Find best possible match
            MM_Request bestMatch = null;
            int bestScoreDiff = int.MaxValue;

            foreach (var candidate in matchMakingPool) {
                if (candidate.IsMatchFound)
                    continue;

                if (candidate.ServerConection.ConnectionId == serverConection.ConnectionId)
                    continue;

                int candidateScore = candidate.ServerConection.User.Score;
                int diff = Math.Abs(candidateScore - requesterScore);

                if (diff <= 10 && diff < bestScoreDiff) {
                    bestScoreDiff = diff;
                    bestMatch = candidate;
                }
            }

            if (bestMatch == null) {
                logger.LogInformation($"No suitable match found for {serverConection.User.Id} (Score {requesterScore})");
                return;
            }

            // 🎮 Match found
            requester.IsMatchFound = true;
            bestMatch.IsMatchFound = true;

            Guid gameId = Guid.NewGuid();
            requester.ServerConection.GameId = gameId;
            bestMatch.ServerConection.GameId = gameId;

            logger.LogInformation(
                "Match found: {Player1} ({Score1}) vs {Player2} ({Score2})",
                requester.ServerConection.User.Id,
                requesterScore,
                bestMatch.ServerConection.User.Id,
                bestMatch.ServerConection.User.Score
            );

            //StartGame(requester.ServerConection, bestMatch.ServerConection);
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
