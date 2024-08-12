using C4U.Core;
using C4U.Game;
using C4U.Input;
using C4U.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C4U
{
    public class Connect4TestSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private GridGenerator _gridGenerator;
        [SerializeField] private Camera _rayCamera;

        private BaseControls _controls;
        private IInputEvent _fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent;

        private Vector3 _currentScreenPos;

        private Connect4Grid<Connect4GridCell> _grid;
        private Transform _currentHighlightedCell;
        private IConnect4GridCell _currentHighlightedCellData;

        private void Awake()
        {
            _grid = new(_gridGenerator.Width, _gridGenerator.Height, _gridGenerator.GridCells);

            // Fallback for camera assignment.
            if (_rayCamera == null)
            {
                _rayCamera = Camera.main;
            }

            _controls = new();
            _controls.Player.Enable();

            // TODO: Move this to the game manager.
            GameState.AddPlayer(new Player(0));
        }

        private void OnEnable()
        {
            _grid.OnCellOccupied += OnGridCellOccupation;
            _grid.OnActiveColumnChanged += OnActiveColumnChanged;

            _fireEvent = new InputEvent(_controls.Player.Fire, OnFire);
            _pointerPositionChangedEvent = new InputEvent(_controls.Player.PointerPosition, OnPointerPositionChanged);
            _moveInDirectionEvent = new InputEvent(_controls.Player.Move, OnMoveInDirection);
            _confirmEvent = new InputEvent(_controls.Player.Confirm, OnConfirmChoice);
        }

        private void OnDisable()
        {
            _grid.OnCellOccupied -= OnGridCellOccupation;
            _grid.OnActiveColumnChanged -= OnActiveColumnChanged;

            _fireEvent = null;
            _pointerPositionChangedEvent = null;
            _moveInDirectionEvent = null;
            _confirmEvent = null;
        }

        private void OnFire(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            IPlayer player = GameState.GetCurrentPlayer<IPlayer>();

            _grid.ConfirmChoice(player);
        }

        private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            _currentScreenPos = ctx.ReadValue<Vector2>();
            _currentScreenPos.z = _rayCamera.nearClipPlane;

            RaycastFromScreen(out RaycastHit hit);
            _grid.SetActiveColumn(hit.transform);
        }

        private void OnMoveInDirection(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            _grid.MoveInDirection(ctx.ReadValue<Vector2>().x > 0 ? 1 : -1);
        }

        private void OnConfirmChoice(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            IPlayer player = GameState.GetCurrentPlayer<IPlayer>();

            _grid.ConfirmChoice(player);
        }

        private void OnGridCellOccupation(Transform cell, IConnect4GridCell data)
        {
            var cellMat = cell.GetComponent<MeshRenderer>().material;

            switch (data.Occupant.PlayerIndex)
            {
                case 0:
                    cellMat.color = Color.red;
                    break;
                case 1:
                    cellMat.color = Color.blue;
                    break;
            };
        }

        private void OnActiveColumnChanged(Transform cell, IConnect4GridCell data)
        {
            if (_currentHighlightedCell != null)
            {
                if (!_currentHighlightedCellData.IsOccupied())
                {
                    _currentHighlightedCell.GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }

            if (cell != null)
            {
                _currentHighlightedCellData = data;
                _currentHighlightedCell = cell;
                _currentHighlightedCell.GetComponent<MeshRenderer>().material.color = Color.green;

                return;
            }

            _currentHighlightedCell = null;
            _currentHighlightedCellData = null;
        }

        private bool RaycastFromScreen(out RaycastHit hit)
        {
            Ray ray = _rayCamera.ScreenPointToRay(_currentScreenPos);

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
            {
                if (!hit.transform)
                    return false;
            }

            return false;
        }
    }
}
