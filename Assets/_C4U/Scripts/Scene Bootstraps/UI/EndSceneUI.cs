using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace C4U
{
    /// <summary>
    /// An interface used to setup the End Scene UI.
    /// </summary>
    public interface IEndSceneUI : ICanvas
    {
        public void SetWinnerText(string text);
        public void SetButtonAction(UnityAction buttonPressedEvent);
    }

    /// <summary>
    /// Implementation of <see cref="IEndSceneUI"/>. <br/>
    /// Used for navigating back to the main menu.
    /// </summary>
    public class EndSceneUI : MonoBehaviour, IEndSceneUI
    {
        [SerializeField] private TMP_Text _winnerText;
        [SerializeField] private Button _button;

        /// <summary>
        /// Set what should happen when the end scene UI button is pressed.
        /// </summary>
        /// <param name="buttonPressedEvent">The method that will be invoked.</param>
        public void SetButtonAction(UnityAction buttonPressedEvent)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(buttonPressedEvent);
        }

        /// <summary>
        /// Set the content of the <see cref="_winnerText"/> text.
        /// </summary>
        /// <param name="text"></param>
        public void SetWinnerText(string text)
        {
            _winnerText.text = text;
        }
    }
}
