using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace C4U
{
    public interface IMainMenuUI : ICanvas
    {
        public void SetPlayButtonAction(UnityAction action);
        public void SetQuitButtonAction(UnityAction action);
    }

    public class MainMenuUI : MonoBehaviour, IMainMenuUI
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton;

        public void SetPlayButtonAction(UnityAction action)
        {
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(action);
        }

        public void SetQuitButtonAction(UnityAction action)
        {
            _quitButton.onClick.RemoveAllListeners();
            _quitButton.onClick.AddListener(action);
        }
    }
}
