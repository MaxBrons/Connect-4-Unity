using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace C4U.Utilities
{
    /// <summary>
    /// Utility calss to generate the grid cells in the scene.
    /// </summary>
    public class GridGenerator : MonoBehaviour
    {
        public int Width => _columnCount;
        public int Height => _rowCount;
        public List<Transform> GridCells => _gridCells;

        [SerializeField, Min(1)] private int _columnCount;
        [SerializeField, Min(1)] private int _rowCount;
        [SerializeField] private GameObject _gridCellPrefab;
        [SerializeField] private Vector2 _cellMargin;

        private List<Transform> _gridCells = new();

        private void Awake()
        {
            // Store all the generated cells.
            _gridCells = transform.GetComponentsInChildren<Transform>()
                .Where(cell => cell != transform)
                .ToList();
        }

        /// <summary>
        /// Remove all previously generated cells and place new ones.
        /// </summary>
        [ContextMenu("Regenerate Grid")]
        private void GenerateGrid()
        {
            _gridCells = transform.GetComponentsInChildren<Transform>()
                .Where(cell => cell != transform)
                .ToList();
            _gridCells.ForEach(cell => DestroyImmediate(cell.gameObject));

            if (_gridCellPrefab == null)
                return;

            for (int y = 0; y < _rowCount; y++)
            {
                for (int x = 0; x < _columnCount; x++)
                {
                    // Offset their positions based on the set margins.
                    float xPos = x * _cellMargin.x + x * _gridCellPrefab.transform.localScale.x;
                    float yPos = y * _cellMargin.y + y * _gridCellPrefab.transform.localScale.y;

                    // Instantiate the set prefab of the grid cell and add it to the grid cell list.
                    var gridCell = Instantiate(_gridCellPrefab, transform);
                    gridCell.transform.localPosition = new Vector3(xPos, yPos);

                    _gridCells.Add(gridCell.transform);
                }
            }
        }
    }
}
