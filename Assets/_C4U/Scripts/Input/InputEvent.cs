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

        /// <summary>
        /// Enable mutiple <see cref="IInputEvent"/>s at the same time.
        /// </summary>
        /// <param name="events">The event(s) to enable.</param>
        public static void EnableMultiple(params IInputEvent[] events)
        {
            foreach (var @event in events)
            {
                @event.Enable();
            }
        }

        /// <summary>
        /// Disable mutiple <see cref="IInputEvent"/>s at the same time.
        /// </summary>
        /// <param name="events">The event(s) to disable.</param>
        public static void DisableMultiple(params IInputEvent[] events)
        {
            foreach (var @event in events)
            {
                @event.Disable();
            }
        }
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
        }

        ~InputEvent()
        {
            Disable();
        }

        /// <summary>
        /// Bind to the started, performed and canceled actions of the stored <see cref="InputAction"/>.
        /// </summary>
        public void Enable()
        {
            // Unbind the action first to stop double bindings from occuring.
            Disable();

            _action.started += _onAction;
            _action.performed += _onAction;
            _action.canceled += _onAction;
        }

        /// <summary>
        /// Unbind from the started, performed and canceled actions of the stored <see cref="InputAction"/>.
        /// </summary>
        public void Disable()
        {
            _action.started -= _onAction;
            _action.performed -= _onAction;
            _action.canceled -= _onAction;
        }
    }
}
