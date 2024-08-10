using System;
using UnityEngine.InputSystem;

namespace C4U.Input
{
    using InputEventAction = Action<InputAction.CallbackContext>;

    /// <summary>
    /// Interface to bind to a Unity input action.
    /// </summary>
    public interface IInputEvent
    {
        public void Bind(InputEventAction onAction);
        public void Unbind(InputEventAction onAction);
    }

    /// <summary>
    /// Implementation of <see cref="IInputEvent"/> to abstract out the Unity Input System bindings.
    /// </summary>
    public class InputEvent : IInputEvent
    {
        private readonly InputAction _action;
        private readonly InputEventAction _onAction;

        /// <summary>
        /// Bind / unbind a method to a given Unity <see cref="InputAction"/>.
        /// </summary>
        /// <param name="action">The <see cref="InputAction"/> to (un)bind to.</param>
        /// <param name="onAction">The method to (un)bind to the given <see cref="InputAction"/>.</param>
        public InputEvent(InputAction action, InputEventAction onAction)
        {
            _action = action;
            _onAction = onAction;

            // Auto bind the action to the input action for redundancy.
            if (_onAction != null)
                Bind(onAction);
        }

        public void Bind(InputEventAction onAction)
        {
            // Unbind the action first to stop double bindings from occuring.
            Unbind(onAction);

            _action.started += onAction;
            _action.performed += onAction;
            _action.canceled += onAction;
        }

        public void Unbind(InputEventAction onAction)
        {
            _action.started -= onAction;
            _action.performed -= onAction;
            _action.canceled -= onAction;
        }
    }
}
