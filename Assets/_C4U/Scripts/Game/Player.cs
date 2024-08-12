namespace C4U.Core
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