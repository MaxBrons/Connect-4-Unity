using C4U.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace C4U.Game
{
    /// <summary>
    /// A class for handling the state of the Connect 4 game's grid.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IConnect4GridCell"/> to store.</typeparam>
    public class Connect4Grid<T> where T : IConnect4GridCell, new()
    {
        // Events for outputting the internal state of the Connect 4 grid.
        public delegate void CellEvent(Transform cell, IConnect4GridCell data);
        public event CellEvent OnCellOccupied;
        public event CellEvent OnActiveColumnChanged;

        private readonly List<Transform> _cells = new(); // Grid cells in the scene. 
        private readonly Grid<IConnect4GridCell> _grid;
        private readonly int _width, _height;
        private int _activeGridColumnIndex = -1;

        public Connect4Grid(int width, int height, List<Transform> cells)
        {
            _cells = cells;
            _grid = new(width, height, () => new T());
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Check if the given Transform is a valid grid cell.
        /// </summary>
        /// <param name="cell">A grid cell in the scene.</param>
        /// <returns></returns>
        public bool Contains(Transform cell)
        {
            return _cells.Contains(cell);
        }

        /// <summary>
        /// Get the index of the valid grid cell.
        /// </summary>
        /// <param name="cell">A grid cell in the scene.</param>
        /// <returns></returns>
        public int IndexOf(Transform cell)
        {
            if (!Contains(cell))
                return -1;

            return _cells.IndexOf(cell);
        }

        /// <summary>
        /// Move the active column in the given direction.
        /// </summary>
        /// <param name="direction">Used to check for a positive or negative horizontal direction.</param>
        public void MoveInDirection(int direction)
        {
            if (direction == 0)
                return;

            _activeGridColumnIndex += direction;

            // Wrap the index when it gets below 0.
            if (_activeGridColumnIndex < 0)
            {
                _activeGridColumnIndex = _width - 1;
            }

            // Wrap the index when it gets past the width of the grid.
            _activeGridColumnIndex %= _width;

            CallOnActiveColumnChanged();
        }

        /// <summary>
        /// Set the current active column based on a grid cell in the scene.
        /// </summary>
        /// <param name="cell">A grid cell in the scene.</param>
        public void SetActiveColumn(Transform cell)
        {
            _activeGridColumnIndex = IndexOf(cell) % _width;

            CallOnActiveColumnChanged();
        }

        /// <summary>
        /// Try to occupy a grid cell in the currently selected grid column.
        /// </summary>
        /// <param name="source">The player that wants to confirm it's choise.</param>
        public void ConfirmChoice(IPlayer source)
        {
            // Return if nothing is selected.
            if (_activeGridColumnIndex < 0)
                return;

            var gridCell = GetUnoccupiedColumnCell(_activeGridColumnIndex);

            // If valid cell, occupy the cell.
            if (gridCell == null)
                return;

            // Occupy the grid cell and call the corresponding event.
            if (gridCell.Occupy(source))
            {
                int cellIndex = _grid.Values.IndexOf(gridCell);
                OnCellOccupied?.Invoke(_cells[cellIndex], gridCell);
            }
        }

        /// <summary>
        /// Get an unoccupied grid cell based on a given grid cell index.
        /// </summary>
        /// <param name="index">The index of the grid cell in the column.</param>
        /// <returns>An unoccupied grid cell or null.</returns>
        public IConnect4GridCell GetUnoccupiedColumnCell(int index)
        {
            if (index < 0)
                return null;

            // Loop to the top to find an unoccupied grid cell.
            for (int y = 0; y < _height; y++)
            {
                var gridCell = _grid.Get(index % _width, y);

                if (!gridCell.IsOccupied())
                    return gridCell;
            }

            // Otherwise return null.
            return null;
        }

        // Find an unoccupied grid cell in the active column and
        // dispatch it via the corresponding event when the active column changes.
        private void CallOnActiveColumnChanged()
        {
            var cell = GetUnoccupiedColumnCell(_activeGridColumnIndex);

            // Retrieve the corresponding grid cell in the scene and dispatch it.
            if (cell != null)
            {
                int cellIndex = _grid.Values.IndexOf(cell);
                Transform cellTransform = _cells[cellIndex];

                OnActiveColumnChanged?.Invoke(cellTransform, cell);

                return;
            }

            // If there's no active column, dispatch null to clear the grid.
            OnActiveColumnChanged?.Invoke(null, null);
        }
    }
}