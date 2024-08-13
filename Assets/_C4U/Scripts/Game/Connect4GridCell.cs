namespace C4U.Game
{
    /// <summary>
    /// An interface used to store base data for the grid's cells.
    /// </summary>
    public interface IConnect4GridCell
    {
        public IPlayer Occupant { get; }

        public bool Occupy(IPlayer occupant);
        public bool IsOccupied();
    }

    /// <summary>
    /// An implementation of <see cref="IConnect4GridCell"/> to store 
    /// which player occupied the cell.
    /// </summary>
    public class Connect4GridCell : IConnect4GridCell
    {
        public IPlayer Occupant { get; private set; }

        /// <summary>
        /// Occupy this cell with the given <see cref="IPlayer"/>.
        /// </summary>
        /// <param name="occupant">The player that should occupy the cell.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Occupy(IPlayer occupant)
        {
            Occupant = occupant;
            return IsOccupied();
        }

        /// <summary>
        /// Return if the slot is occupied by a valid <see cref="IPlayer"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsOccupied()
        {
            return Occupant != null;
        }
    }
}