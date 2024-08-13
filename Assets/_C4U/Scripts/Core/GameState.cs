using C4U.Game;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace C4U.Core
{
    /// <summary>
    /// An interface for handling general Game State functionality.
    /// </summary>
    public interface IGameState
    {
        // Used for the active state of the game.
        public enum GameState
        {
            None, Active, GameOver
        }

        public int CurrentPlayerIndex { get; }
        public int PlayerCount { get; }

        public void AddPlayer(IPlayer player);
        public void RemovePlayer(int index);
        public void SetCurrentPlayer(int index);
        public T GetCurrentPlayer<T>() where T : class, IPlayer;
        public T GetPlayerByIndex<T>(int index) where T : class, IPlayer;

        public void SetGameState(GameState state);
        public GameState GetGameState();
    }

    /// <summary>
    /// A Class that represents the state of the game. <br/>
    /// It keeps track of the players in the game and has logic to interface with the players.
    /// </summary>
    public class GameState : IGameState
    {
        public int CurrentPlayerIndex => _currentPlayerIndex;
        public int PlayerCount => _players.Count;

        private readonly List<IPlayer> _players = new();
        private int _currentPlayerIndex = -1;

        private IGameState.GameState _state;

        /// <summary>
        /// Add a player to the game.
        /// </summary>
        /// <param name="player">The player to add.</param>
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
        /// <param name="index">The player to remove with the given player index.</param>
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
                return;
            }

            _currentPlayerIndex = -1;
        }

        /// <summary>
        /// Get the active player and cast it to the given player type.
        /// </summary>
        /// <typeparam name="T">Type of the class that implements <see cref="IPlayer"/>.</typeparam>
        /// <returns></returns>
        public T GetCurrentPlayer<T>() where T : class, IPlayer
        {
            return _players.FirstOrDefault(player => player.PlayerIndex == _currentPlayerIndex) as T;
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

        /// <summary>
        /// Set the current game state to the given value.
        /// </summary>
        /// <param name="state">The new state of the game.</param>
        public void SetGameState(IGameState.GameState state)
        {
            _state = state;
        }

        /// <summary>
        /// Return the current state of the game.
        /// </summary>
        /// <returns></returns>
        public IGameState.GameState GetGameState()
        {
            return _state;
        }
    }
}