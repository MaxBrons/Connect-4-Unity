namespace C4U.Game
{
    public interface IPlayer
    {
        public int PlayerIndex { get; }
    }

    public class Player : IPlayer
    {
        public int PlayerIndex { get; }

        public Player(int index)
        {
            PlayerIndex = index;
        }
    }
}