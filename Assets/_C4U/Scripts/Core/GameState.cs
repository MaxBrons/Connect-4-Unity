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
    public class GameState
    {
        public int CurrentPlayerIndex => _currentPlayerIndex;
        public int PlayerCount => _players.Count;

        private readonly List<IPlayer> _players = new();
        private int _currentPlayerIndex = -1;

        /// <summary>
        /// Add a player to the game.
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(IPlayer player)
        {
            if (player != null)
            {
                _players.Add(player);

                if (_currentPlayerIndex < 0)
                {
                    _currentPlayerIndex = player.PlayerIndex;
                }
            }
        }

        /// <summary>
        /// Removes a player from the game. <br/>
        /// Asserts if index is invalid.
        /// </summary>
        /// <param name="index"></param>
        public void RemovePlayer(int index)
        {
            if (index >= 0 && index < _players.Count)
            {
                _players.RemoveAt(index);
            }

            Debug.Assert(index >= 0 && index < _players.Count,
                "Error while trying to remove a Player: Invalid Player Index");
        }

        /// <summary>
        /// Set the new active player of the game. <br/>
        /// The given index will be wrapped using the modulo operator for ease.
        /// </summary>
        /// <param name="index">The index of the new active player.</param>
        public void SetCurrentPlayer(int index)
        {
            var player = _players.FirstOrDefault(player => player.PlayerIndex == index);

            if (player != null)
            {
                _currentPlayerIndex = player.PlayerIndex;
            }
        }

        /// <summary>
        /// Get the active player and cast it to the given player type.
        /// </summary>
        /// <typeparam name="T">Type of the class that implements <see cref="IPlayer"/>.</typeparam>
        /// <returns></returns>
        public T GetCurrentPlayer<T>() where T : class, IPlayer
        {
            return _players[_currentPlayerIndex] as T;
        }

        /// <summary>
        /// Get a player based on a given player index and cast it to the given player type.
        /// </summary>
        /// <typeparam name="T">Type of the class that implements <see cref="IPlayer"/>.</typeparam>
        /// <param name="index">The resulting player's index.</param>
        /// <returns></returns>
        public T GetPlayerByIndex<T>(int index) where T : class, IPlayer
        {
            return _players.FirstOrDefault(player => player.PlayerIndex == index) as T;
        }
    }
}