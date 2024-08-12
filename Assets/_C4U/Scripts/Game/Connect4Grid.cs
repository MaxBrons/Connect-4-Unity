using C4U.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace C4U.Game
{
    public class Connect4GridCell
    {
        public IPlayer Occupant => _occupant;
        public bool Occupied => _occupant != null;

        private IPlayer _occupant;

        public bool Occupy(IPlayer occupant)
        {
            if (occupant == null)
                return false;

            _occupant = occupant;

            return true;
        }
    }

    public class Connect4Grid
    {
        public delegate void CellEvent(Transform cell, Connect4GridCell data);
        public event CellEvent OnCellOccupied;
        public event CellEvent OnActiveColumnChanged;
        public Vector2Int Size => new(_width, _height);

        private readonly List<Transform> _cells = new();
        private readonly Grid<Connect4GridCell> _grid;
        private readonly int _width, _height;
        private int _activeGridColumnIndex = -1;

        public Connect4Grid(int width, int height, List<Transform> cells)
        {
            _cells = cells;
            _grid = new(width, height, () => new());
            _width = width;
            _height = height;
        }

        public bool Contains(Transform cell)
        {
            return _cells.Contains(cell);
        }

        public int IndexOf(Transform cell)
        {
            if (!Contains(cell))
                return -1;

            return _cells.IndexOf(cell);
        }

        public void MoveInDirection(int direction)
        {
            if (direction == 0)
                return;

            _activeGridColumnIndex += direction;

            if (_activeGridColumnIndex < 0)
            {
                _activeGridColumnIndex = _width - 1;
            }

            _activeGridColumnIndex %= _width;

            CallOnActiveColumnChanged();
        }

        public void SetActiveColumn(Transform cell)
        {
            bool shouldFireEvent = _activeGridColumnIndex >= 0;

            _activeGridColumnIndex = IndexOf(cell) % _width;

            if (shouldFireEvent)
            {
                CallOnActiveColumnChanged();
            }
        }

        public void ConfirmChoice(IPlayer source)
        {
            if (_activeGridColumnIndex < 0)
                return;

            var gridCell = GetUnoccupiedColumnCell(_activeGridColumnIndex);

            // If valid cell, occupy the cell.
            if (gridCell == null)
                return;

            if (gridCell.Occupy(source))
            {
                int cellIndex = _grid.Values.IndexOf(gridCell);
                OnCellOccupied?.Invoke(_cells[cellIndex], gridCell);
            }
        }

        public Connect4GridCell GetUnoccupiedColumnCell(int index)
        {
            if (index < 0)
                return null;

            for (int y = 0; y < _height; y++)
            {
                var gridCell = _grid.Get(index % _width, y);

                if (!gridCell.Occupied)
                    return gridCell;
            }

            return null;
        }

        private void CallOnActiveColumnChanged()
        {
            var cell = GetUnoccupiedColumnCell(_activeGridColumnIndex);
            int cellIndex = _grid.Values.IndexOf(cell);
            Transform cellTransform = cellIndex >= 0 ? _cells[cellIndex] : null;

            OnActiveColumnChanged?.Invoke(cellTransform, cell);
        }
    }
}