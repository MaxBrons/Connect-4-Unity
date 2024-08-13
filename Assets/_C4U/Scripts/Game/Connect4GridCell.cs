namespace C4U.Game
{
    public interface IConnect4GridCell
    {
        public IPlayer Occupant { get; }

        public bool Occupy(IPlayer occupant);
        public bool IsOccupied();
    }

    public class Connect4GridCell : IConnect4GridCell
    {
        public IPlayer Occupant { get; private set; }

        public bool Occupy(IPlayer occupant)
        {
            Occupant = occupant;
            return IsOccupied();
        }

        public bool IsOccupied()
        {
            return Occupant != null;
        }
    }
}