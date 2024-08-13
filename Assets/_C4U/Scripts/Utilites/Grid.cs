using System;
using System.Collections.Generic;
using System.Linq;

namespace C4U.Utilities
{
    /// <summary>
    /// Generic class for representing a grid.
    /// </summary>
    /// <typeparam name="T">Type of the grid cells</typeparam>
    public class Grid<T>
    {
        // Ease of access variables to interface with the grid.
        public List<T> Values => _grid;
        public int Size => _grid.Count;
        public readonly int Width, Height;

        // The list full of grid cells.
        private readonly List<T> _grid;

        /// <summary>
        /// Create a grid with a given width and height. <br/>
        /// It's possible to initialize the entire grid using the intializer Func.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="initializer">The Func for initializing each grid cell.</param>
        public Grid(int width, int height, Func<int, T> initializer = null)
        {
            _grid = new(width * height);

            Width = width;
            Height = height;

            // Go through the entire grid and initialize each cell with
            // either the initializer Func value or their defaults.
            for (int i = 0; i < width * height; i++)
            {
                if (initializer != null)
                {
                    _grid.Add(initializer(i));
                    continue;
                }

                _grid.Add(default);
            }
        }

        /// <summary>
        /// Set a grid cell's value.
        /// </summary>
        /// <param name="x">X position of the grid cell.</param>
        /// <param name="y">Y position of the grid cell.</param>
        /// <param name="value">The new value of the grid cell.</param>
        public void Set(int x, int y, T value)
        {
            // Bounds-check the given position values and set the new value.
            if (IsValidIndex(x, y))
                _grid[x + y * Width] = value;
        }

        /// <summary>
        /// Return a grid cell's value at a given position or null. <br/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public T Get(int x, int y)
        {
            return _grid.ElementAtOrDefault(x + y * Width);
        }

        /// <summary>
        /// Return a grid cell's value at a given index or null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T Get(int index)
        {
            return _grid.ElementAtOrDefault(index);
        }

        // Check if the x/y coordinates are in within the grid's range.
        private bool IsValidIndex(int x, int y)
        {
            return x >= 0 && x < Width
                && y >= 0 && y < Height;
        }
    }
}