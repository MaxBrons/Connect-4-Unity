using C4U.Core;
using C4U.Core.SceneManagement;
using C4U.Game;
using C4U.Input;
using C4U.Utilities;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C4U
{
    // Bootstrap for the main game. Handles most of the game.
    public class MainSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private GridGenerator _gridGenerator;
        [SerializeField] private Camera _rayCamera;

        // Input values.
        private BaseControls _controls;
        private IInputEvent _fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent;

        private Vector3 _currentScreenPos;

        // Grid value.
        private Connect4Grid<Connect4GridCell> _grid;
        private Transform _currentHighlightedCell;
        private IConnect4GridCell _currentHighlightedCellData;

        // Core values / UI.
        private IGameState _gameState;
        private IMainSceneUI _mainSceneUI;
        private IEndSceneUI _endSceneUI;

        private async void Awake()
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

            await Task.Delay(100);

            // Wait a bit, because Unity's execution order messes with the validity.
            _gameState = await ICore.Container.Get<IGameState>();
            _gameState.SetGameState(IGameState.GameState.Active);

            _mainSceneUI = await SceneLoader.LoadCanvasSceneAsync<IMainSceneUI>("MainSceneUI");
            _mainSceneUI.SetActivePlayer(_gameState.CurrentPlayerIndex);
        }

        private void OnEnable()
        {
            _grid.OnCellOccupied += OnGridCellOccupation;
            _grid.OnActiveColumnChanged += OnActiveColumnChanged;
            _grid.OnConnectionFound += OnConnectionFound;
            _grid.OnGridOccupied += OnGridOccupied;

            IInputEvent.EnableMultiple(_fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent);

            _mainSceneUI?.SetActivePlayer(_gameState.CurrentPlayerIndex);
        }

        private void OnDisable()
        {
            _grid.OnCellOccupied -= OnGridCellOccupation;
            _grid.OnActiveColumnChanged -= OnActiveColumnChanged;
            _grid.OnConnectionFound -= OnConnectionFound;
            _grid.OnGridOccupied -= OnGridOccupied;

            IInputEvent.DisableMultiple(_fireEvent, _pointerPositionChangedEvent, _moveInDirectionEvent, _confirmEvent);

            _mainSceneUI?.SetActivePlayer(-1);
        }

        // Try to confirm the current player's choice when pressing the pointer's button.
        private void OnFire(InputAction.CallbackContext ctx)
        {
            IPlayer player = _gameState.GetCurrentPlayer<IPlayer>();

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
            IPlayer player = _gameState.GetCurrentPlayer<IPlayer>();

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

            int nexPlayerIndex = (_gameState.CurrentPlayerIndex + 1) % _gameState.PlayerCount;

            // Begin the next player's turn.
            _gameState.SetCurrentPlayer(nexPlayerIndex);

            _mainSceneUI.SetActivePlayer(_gameState.CurrentPlayerIndex);
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

        // GAME OVER!
        // Open the end screen and disable main game input.
        private async void OnConnectionFound(IPlayer victor)
        {
            if (_gameState.GetGameState() != IGameState.GameState.Active)
                return;

            OnDisable();

            _gameState.SetGameState(IGameState.GameState.GameOver);

            await SceneLoader.UnloadSceneAsync("MainSceneUI");
            ShowEndScreen();
        }

        // GAME OVER!
        // Open the end screen and disable main game input.
        private async void OnGridOccupied()
        {
            if (_gameState.GetGameState() != IGameState.GameState.Active)
                return;

            // Nobody won, so set the index to -1 to show the 'tie' text.
            _gameState.SetCurrentPlayer(-1);
            _gameState.SetGameState(IGameState.GameState.GameOver);

            OnDisable();

            await SceneLoader.UnloadSceneAsync("MainSceneUI");
            ShowEndScreen();
        }

        // Show the end screen with the corresponding title text.
        // Load the main menu when the continue button is pressed.
        private async void ShowEndScreen()
        {
            // Wait a bit to unregister pointer click.
            await Task.Delay(250);

            _endSceneUI = await SceneLoader.LoadCanvasSceneAsync<IEndSceneUI>("EndSceneUI");

            // Set title text.
            int playerIndex = _gameState.GetCurrentPlayer<IPlayer>().PlayerIndex;
            string winnerText = playerIndex >= 0
                ? "Result: PLAYER " + playerIndex + " won!"
                : "Result: Tie";

            _endSceneUI.SetWinnerText(winnerText);

            // Go to main menu on continue button pressed.
            _endSceneUI.SetButtonAction(async () =>
            {
                _endSceneUI.SetButtonAction(null);
                await SceneLoader.UnloadSceneAsync("Main");
                await SceneLoader.UnloadSceneAsync("EndSceneUI");
                await SceneLoader.LoadSceneAsync("MainMenu");
            });
        }
    }
}
