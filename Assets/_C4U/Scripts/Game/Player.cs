namespace C4U.Game
{
    /// <summary>
    /// An interface that stores player data.
    /// </summary>
    public interface IPlayer
    {
        public int PlayerIndex { get; }
    }

    /// <summary>
    /// An implementation of <see cref="IPlayer"/> to use 
    /// for checking which player's turn it is.
    /// </summary>
    public class Player : IPlayer
    {
        public int PlayerIndex { get; }

        public Player(int index)
        {
            PlayerIndex = index;
        }
    }
}