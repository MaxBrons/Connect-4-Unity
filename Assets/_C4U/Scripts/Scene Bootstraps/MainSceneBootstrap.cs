using C4U.Core;
using C4U.Game;
using C4U.Input;
using C4U.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C4U
{
    public class MainSceneBootstrap : MonoBehaviour
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
            // Create a Connect 4 Grid and initialize it with the GridGenerator's cells.
            _grid = new(_gridGenerator.Width, _gridGenerator.Height, _gridGenerator.GridCells);
            _controls = new();

            var playerInput = _controls.Player;
            playerInput.Enable();

            // Create and bind the input events for the game.
            _fireEvent = new InputEventSimple(playerInput.Fire, OnFire);
            _pointerPositionChangedEvent = new InputEventSimple(playerInput.PointerPosition, OnPointerPositionChanged);
            _moveInDirectionEvent = new InputEventSimple(playerInput.Move, OnMoveInDirection);
            _confirmEvent = new InputEventSimple(playerInput.Confirm, OnConfirmChoice);

            // Fallback for camera assignment.
            if (_rayCamera == null)
            {
                _rayCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            _grid.OnCellOccupied += OnGridCellOccupation;
            _grid.OnActiveColumnChanged += OnActiveColumnChanged;

            IInputEvent.EnableMultiple(_fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent);
        }

        private void OnDisable()
        {
            _grid.OnCellOccupied -= OnGridCellOccupation;
            _grid.OnActiveColumnChanged -= OnActiveColumnChanged;

            IInputEvent.DisableMultiple(_fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent);
        }

        // Try to confirm the current player's choice when pressing the pointer's button.
        private void OnFire(InputAction.CallbackContext ctx)
        {
            IPlayer player = GameState.GetCurrentPlayer<IPlayer>();

            _grid.ConfirmChoice(player);
        }

        // Update the grid's active column when the pointer moves.
        private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
        {
            _currentScreenPos = ctx.ReadValue<Vector2>();
            _currentScreenPos.z = _rayCamera.nearClipPlane;

            _rayCamera.RaycastFromScreen(_currentScreenPos, out RaycastHit hit);
            _grid.SetActiveColumn(hit.transform);
        }

        // Update the current highlighted grid cell when the player wants to move in a given direction.
        private void OnMoveInDirection(InputAction.CallbackContext ctx)
        {
            int dir = ctx.ReadValue<Vector2>().x > 0 ? 1 : -1;

            _grid.MoveInDirection(dir);
        }

        // Try to confirm the player's choice when pressing the confirm button.
        private void OnConfirmChoice(InputAction.CallbackContext ctx)
        {
            IPlayer player = GameState.GetCurrentPlayer<IPlayer>();

            _grid.ConfirmChoice(player);
        }

        // When the grid cell is succesfully occupied, update the color to the player's matching color.
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

            int nexPlayerIndex = (GameState.CurrentPlayerIndex + 1) % GameState.PlayerCount;

            // Begin the next player's turn.
            GameState.SetCurrentPlayer(nexPlayerIndex);
        }

        // Update the current highlighted grid cell.
        private void OnActiveColumnChanged(Transform cell, IConnect4GridCell data)
        {
            // Change the color back to white when the unoccupied grid cell was previously highlighted.
            if (_currentHighlightedCell != null)
            {
                if (!_currentHighlightedCellData.IsOccupied())
                {
                    _currentHighlightedCell.GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }

            // Change the unoccupied grid cells's color to green when there is an active column.
            if (cell != null)
            {
                _currentHighlightedCellData = data;
                _currentHighlightedCell = cell;
                _currentHighlightedCell.GetComponent<MeshRenderer>().material.color = Color.green;

                return;
            }

            // Clear all stored values when no column is active.
            _currentHighlightedCell = null;
            _currentHighlightedCellData = null;
        }
    }
}
