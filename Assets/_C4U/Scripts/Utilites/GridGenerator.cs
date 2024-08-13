using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace C4U.Utilities
{
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
            _gridCells = transform.GetComponentsInChildren<Transform>()
                .Where(cell => cell != transform)
                .ToList();
        }

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
                    float xPos = x * _cellMargin.x + x * _gridCellPrefab.transform.localScale.x;
                    float yPos = y * _cellMargin.y + y * _gridCellPrefab.transform.localScale.y;

                    var gridCell = Instantiate(_gridCellPrefab, transform);
                    gridCell.transform.localPosition = new Vector3(xPos, yPos);


                    _gridCells.Add(gridCell.transform);
                }
            }
        }
    }
}
