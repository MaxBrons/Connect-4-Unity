using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace C4U
{
    /// <summary>
    /// An interface used to setup the Main Menu UI.
    /// </summary>
    public interface IMainMenuUI : ICanvas
    {
        public void SetPlayButtonAction(UnityAction action);
        public void SetQuitButtonAction(UnityAction action);
    }

    /// <summary>
    /// Implementation of <see cref="IMainMenuUI"/>. <br/>
    /// Used for handling what should happen when the main menu buttons are pressed.
    /// </summary>
    public class MainMenuUI : MonoBehaviour, IMainMenuUI
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton;

        /// <summary>
        /// Set what should happen when the play button is pressed.
        /// </summary>
        /// <param name="action">The method that will be invoked.</param>
        public void SetPlayButtonAction(UnityAction action)
        {
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(action);
        }

        /// <summary>
        /// Set what should happen when the quit button is pressed.
        /// </summary>
        /// <param name="action">The method that will be invoked.</param>
        public void SetQuitButtonAction(UnityAction action)
        {
            _quitButton.onClick.RemoveAllListeners();
            _quitButton.onClick.AddListener(action);
        }
    }
}
