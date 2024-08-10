using C4U.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C4U
{
    public class InputTestSceneBootstrap : MonoBehaviour
    {
        private BaseControls _controls;

        // NOTE: could be removed if decided not to disable input during gameplay.
        private IInputEvent _mousePositionEvent,
            _fireEvent, _moveInDirectionEvent, _confirmEvent;

        private void Awake()
        {
            _controls = new();
        }

        // Link the input actions to their corresponding input event method.
        private void Start()
        {
            _mousePositionEvent = new InputEvent(_controls.Player.PointerPosition, OnPointerPositionChanged);
            _fireEvent = new InputEvent(_controls.Player.Fire, OnFire);
            _moveInDirectionEvent = new InputEvent(_controls.Player.Move, OnMoveInDirection);
            _confirmEvent = new InputEvent(_controls.Player.Confirm, OnConfirm);
        }

        private void OnEnable()
        {
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
        {
            print("Pointer position changed: " + ctx.ReadValue<Vector2>());
        }

        private void OnFire(InputAction.CallbackContext ctx)
        {
            print("Pressed pointer: " + ctx.ReadValueAsButton());
        }

        private void OnMoveInDirection(InputAction.CallbackContext ctx)
        {
            print("Direction to move in: " + ctx.ReadValue<Vector2>());
        }

        private void OnConfirm(InputAction.CallbackContext ctx)
        {
            print("Confirmed: " + ctx.ReadValueAsButton());
        }
    }
}
