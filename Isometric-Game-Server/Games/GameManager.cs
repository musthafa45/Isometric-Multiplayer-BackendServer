using System;
using System.Collections.Generic;

namespace Isometric_Game_Server.Games {
    public class GameManager {
        private List<Game> games;

        public GameManager() {
            games = new List<Game>();

        }

        public Guid RegisterGame(string[] players) {
            Game game = new Game(players);
            games.Add(game);
            return game.Id;
        }

        public Game GetGame(string userName) {
            foreach (Game game in games) {
                if (game.PlayersInGameData.Exists(p => p.Player == userName))
                    return game;
            }
            return null;
        }


        public Game CloseGame(string userName) {
            Game game = GetGame(userName);
            if (game != null) {
                games.Remove(game);
                return game;
            }
            return null;
        }

        public bool IsGameExists(string userName) {
            return GetGame(userName) != null;
        }

        public int GetGamesCount() {
            return games.Count;
        }
    }
}