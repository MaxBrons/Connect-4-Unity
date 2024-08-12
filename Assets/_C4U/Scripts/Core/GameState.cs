using C4U.Game;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace C4U.Core
{
    /// <summary>
    /// A Class that represents the state of the game. <br/>
    /// It keeps track of the players in the game and has logic to interface with the players.
    /// </summary>
    public static class GameState
    {
        public static int CurrentPlayerIndex => s_currentPlayerIndex;

        private readonly static List<IPlayer> s_players = new();
        private static int s_currentPlayerIndex = -1;

        /// <summary>
        /// Add a player to the game.
        /// </summary>
        /// <param name="player"></param>
        public static void AddPlayer(IPlayer player)
        {
            if (player != null)
            {
                s_players.Add(player);

                if (s_currentPlayerIndex < 0)
                {
                    s_currentPlayerIndex = player.PlayerIndex;
                }
            }
        }

        /// <summary>
        /// Removes a player from the game. <br/>
        /// Asserts if index is invalid.
        /// </summary>
        /// <param name="index"></param>
        public static void RemovePlayer(int index)
        {
            if (index >= 0 && index < s_players.Count)
            {
                s_players.RemoveAt(index);
            }

            Debug.Assert(index >= 0 && index < s_players.Count,
                "Error while trying to remove a Player: Invalid Player Index");
        }

        /// <summary>
        /// Set the new active player of the game. <br/>
        /// The given index will be wrapped using the modulo operator for ease.
        /// </summary>
        /// <param name="index">The index of the new active player.</param>
        public static void SetCurrentPlayer(int index)
        {
            var player = s_players.FirstOrDefault(player => player.PlayerIndex == index);

            if (player != null)
            {
                s_currentPlayerIndex = player.PlayerIndex % (s_players.Count - 1);
            }
        }

        /// <summary>
        /// Get the active player and cast it to the given player type.
        /// </summary>
        /// <typeparam name="T">Type of the class that implements <see cref="IPlayer"/>.</typeparam>
        /// <returns></returns>
        public static T GetCurrentPlayer<T>() where T : class, IPlayer
        {
            return s_players[s_currentPlayerIndex] as T;
        }

        /// <summary>
        /// Get a player based on a given player index and cast it to the given player type.
        /// </summary>
        /// <typeparam name="T">Type of the class that implements <see cref="IPlayer"/>.</typeparam>
        /// <param name="index">The resulting player's index.</param>
        /// <returns></returns>
        public static T GetPlayerByIndex<T>(int index) where T : class, IPlayer
        {
            return s_players.FirstOrDefault(player => player.PlayerIndex == index) as T;
        }
    }
}