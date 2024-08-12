using C4U.Core;
using C4U.Input;
using C4U.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C4U
{
    public class Connect4GridCell
    {
        public bool Occupied = false;
        public IPlayer Occupant = null;

        public void Occupy(IPlayer occupantID)
        {
            if (occupantID == null)
                return;

            Occupant = occupantID;
            Occupied = true;
        }
    }

    public class Connect4TestSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private GridGenerator _gridGenerator;
        [SerializeField] private Camera _rayCamera;

        private Grid<Connect4GridCell> _grid;
        private BaseControls _controls;
        private IInputEvent _fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent;

        private int _width, _height;
        private List<Transform> _cells = new();

        private Vector3 _currentScreenPos;
        private int _activeGridCellIndex = -1;

        private void Awake()
        {
            _width = _gridGenerator.Width;
            _height = _gridGenerator.Height;
            _cells = _gridGenerator.GridCells;

            _grid = new(_width, _height, () => new());

            // Fallback for camera assignment.
            if (_rayCamera == null)
            {
                _rayCamera = Camera.main;
            }

            _controls = new();
            _controls.Player.Enable();

            _fireEvent = new InputEvent(_controls.Player.Fire, OnFire);
            _pointerPositionChangedEvent = new InputEvent(_controls.Player.PointerPosition, OnPointerPositionChanged);
            _moveInDirectionEvent = new InputEvent(_controls.Player.Move, OnMoveInDirection);
            _confirmEvent = new InputEvent(_controls.Player.Confirm, OnConfirmChoice);

            // TODO: Move this to the game manager.
            GameState.AddPlayer(new Player(0));
        }

        private void OnFire(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            Ray ray = _rayCamera.ScreenPointToRay(_currentScreenPos);

            if (Physics.Raycast(ray.origin, ray.direction * 10, out RaycastHit hit, 100f))
            {
                if (!hit.transform)
                    return;

                var cellIndex = _cells.FindIndex(cell => cell.Equals(hit.transform));

                TryChooseGridCell(cellIndex);
            }
        }

        private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            var screenPos = ctx.ReadValue<Vector2>();
            _currentScreenPos = new Vector3(screenPos.x, screenPos.y, _rayCamera.nearClipPlane);

            Ray ray = _rayCamera.ScreenPointToRay(_currentScreenPos);

            if (Physics.Raycast(ray.origin, ray.direction * 10, out RaycastHit hit, 100f))
            {
                if (!hit.transform)
                    return;

                var cell = _cells.Find(cell => cell == hit.transform);

                if (cell != null)
                {
                    _activeGridCellIndex = _cells.IndexOf(cell);
                }
            }
            else
            {
                _activeGridCellIndex = -1;
            }
        }

        private void OnMoveInDirection(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;
        }

        private void OnConfirmChoice(InputAction.CallbackContext ctx)
        {
            if (!ctx.ReadValueAsButton())
                return;

            if (_activeGridCellIndex >= 0)
            {
                TryChooseGridCell(_activeGridCellIndex);
            }
        }

        private Connect4GridCell GetChosenColumnCell(int index)
        {
            for (int y = 0; y < _height; y++)
            {
                int x = index % _grid.Width;
                var gridCell = _grid.Get(x, y);

                if (!gridCell.Occupied)
                    return gridCell;
            }

            return default;
        }

        private void UpdateGrid()
        {
            for (int i = 0; i < _cells.Count - 1; i++)
            {
                var cell = _grid.Get(i % _width, i / _width);

                if (!cell.Occupied)
                    continue;

                var cellMat = _cells[i].GetComponent<MeshRenderer>().material;

                if (cell.Occupant.PlayerIndex == 0)
                    cellMat.color = Color.red;
                if (cell.Occupant.PlayerIndex == 1)
                    cellMat.color = Color.blue;
            }
        }

        private bool TryChooseGridCell(int index)
        {
            var gridCell = GetChosenColumnCell(index);

            // If valid cell, occupy the cell and update the grid.
            if (gridCell != null)
            {
                IPlayer activePlayer = GameState.GetCurrentPlayer<IPlayer>();
                gridCell.Occupy(activePlayer);

                UpdateGrid();
                return true;
            }
            return false;
        }
    }
}
