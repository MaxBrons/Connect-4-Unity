using System;
using UnityEngine.InputSystem;

namespace C4U.Input
{
    using InputActionEvent = Action<InputAction.CallbackContext>;

    /// <summary>
    /// Interface used for easy binding to a Unity input action.
    /// </summary>
    public interface IInputEvent
    {
        public void Enable();
        public void Disable();
    }

    /// <summary>
    /// Implementation of <see cref="IInputEvent"/> to abstract out the Unity Input System bindings. <br/>
    /// Auto unbinds when the object is destroyed.
    /// </summary>
    public class InputEvent : IInputEvent
    {
        private readonly InputAction _action;
        private readonly InputActionEvent _onAction;

        /// <summary>
        /// Bind / unbind a method to a given Unity <see cref="InputAction"/>.
        /// </summary>
        /// <param name="action">The <see cref="InputAction"/> to (un)bind to.</param>
        /// <param name="onAction">The method to bind to/unbind from the given <see cref="InputAction"/>.</param>
        public InputEvent(InputAction action, InputActionEvent onAction)
        {
            _action = action;
            _onAction = onAction;

            Enable();
        }

        ~InputEvent()
        {
            Disable();
        }

        public void Enable()
        {
            // Unbind the action first to stop double bindings from occuring.
            Disable();

            _action.started += _onAction;
            _action.performed += _onAction;
            _action.canceled += _onAction;
        }

        public void Disable()
        {
            _action.started -= _onAction;
            _action.performed -= _onAction;
            _action.canceled -= _onAction;
        }
    }
}
