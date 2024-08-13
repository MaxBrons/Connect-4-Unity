using C4U.Utilities;
using System;
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

        public delegate void ConnectionEvent(IPlayer victor);
        public event ConnectionEvent OnConnectionFound;

        public event Action OnGridOccupied;

        private readonly List<Transform> _cells = new(); // Grid cells in the scene. 
        private readonly Grid<IConnect4GridCell> _grid;
        private readonly int _width, _height;
        private int _activeGridColumnIndex = -1;

        public Connect4Grid(int width, int height, List<Transform> cells)
        {
            _cells = cells;
            _grid = new(width, height, index => new T());
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

                CheckForConnections(cellIndex, source);
                CheckForFullGrid();
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

        // Go through the surrounding 8 cells and check if they have the same occupant. <br/>
        // If the occupant is the same and the maxDepth has been reached, we have a valid Connect 4 match.
        private void CheckForConnections(int startIndex, IPlayer source)
        {
            // Loop from top left to bottom right, but skip the middle.
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    // Get the XY coordinates and check if the XY + offset is within the bounds.
                    int xPos = startIndex % _width;
                    int yPos = startIndex / _width;

                    if (xPos + x < 0 || xPos + x >= _width)
                        continue;

                    if (yPos + y < 0 || yPos + y >= _height)
                        continue;

                    // Start searching from the placed down piece.
                    var success = CheckConnectionDirection(xPos, yPos, x, y, 0, 3, source);

                    // Invoke the event when a valid Connect 4 match has been found.
                    if (success)
                    {
                        OnConnectionFound?.Invoke(source);
                    }
                }
            }
        }

        // Compare a given cell's occupant to the source.
        // Then go to the next cell in the same direction.
        // Repeat this untill the maxDepth has been reached to see if there is a Connect 4 match.
        private bool CheckConnectionDirection(int x, int y, int xOffset, int yOffset, int depth, int maxDepth, IPlayer source)
        {
            // Return if 4 pieces with the same occupant allign next to each other.
            if (depth >= maxDepth)
                return true;

            // Get the neighbouring cell in the same given direction, based on the offsets.
            var cell = _grid.Get(x + xOffset, y + yOffset);

            if (cell == null)
                return false;

            // Repeat this search untill the maxDepth has been reached or if the next cell is invalid.
            if (cell.Occupant == source)
            {
                return CheckConnectionDirection(x + xOffset, y + yOffset, xOffset, yOffset, depth + 1, maxDepth, source);
            }

            return false;
        }


        private void CheckForFullGrid()
        {
            foreach (var cell in _grid.Values)
            {
                if (!cell.IsOccupied())
                    return;
            }

            OnGridOccupied?.Invoke();
        }
    }
}