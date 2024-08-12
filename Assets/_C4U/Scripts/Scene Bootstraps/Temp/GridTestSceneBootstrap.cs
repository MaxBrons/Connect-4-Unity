using C4U.Utilities;
using UnityEngine;

namespace C4U
{
    public class GridTestSceneBootstrap : MonoBehaviour
    {
        private Grid<int> _grid;

        void Start()
        {
            _grid = new(10, 5, () => 0);
            _grid.Set(0, 0, 1);
            _grid.Set(1, 1, 1);
            _grid.Set(2, 2, 1);
        }

#if UNITY_EDITOR
        // Draw a grid of wire cubes to test if the grid is functional (using test data).
        private void OnDrawGizmos()
        {
            if (_grid == null)
                return;

            const float gizmoScale = 0.5f;

            for (int i = 0; i < _grid.Size; i++)
            {
                int x = i % _grid.Width;
                int y = i / _grid.Width;

                Gizmos.color = _grid.Get(x, y) == 0 ? Color.red : Color.green;
                Gizmos.DrawWireCube(new Vector3(x, y, 0) * gizmoScale, Vector3.one * gizmoScale);
            }
        }
#endif
    }
}
