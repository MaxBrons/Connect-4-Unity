using TMPro;
using UnityEngine;

namespace C4U
{
    /// <summary>
    /// An interface used to setup the Main Scene UI.
    /// </summary>
    public interface IMainSceneUI : ICanvas
    {
        public void SetActivePlayer(int index);
        public void SetPlayerColor(int index, Color color);
    }

    /// <summary>
    /// Implementation of <see cref="IMainMenuUI"/>. <br/>
    /// Used for interfacing with the game overlay (i.e. which player's turn it is).
    /// </summary>
    public class MainSceneUI : MonoBehaviour, IMainSceneUI
    {
        [SerializeField] private TMP_Text _player1Text;
        [SerializeField] private TMP_Text _player2Text;

        [Range(0.1f, 1)]
        [SerializeField] private float _inactiveValue = 0.1f;

        /// <summary>
        /// Highlight the current player's text to visualise which player's turn it is.
        /// </summary>
        /// <param name="index"></param>
        public void SetActivePlayer(int index)
        {
            ChangeTextColor(_player1Text, index == 0);
            ChangeTextColor(_player2Text, index == 1);
        }

        /// <summary>
        /// Set the color of the displayed text that corresponds to the player's index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void SetPlayerColor(int index, Color color)
        {
            TMP_Text text = index == 0 ? _player1Text : _player2Text;
            SetTextColor(text, color);
        }

        // Only set the Alpha value, don't change the RGB values.
        private void ChangeTextColor(TMP_Text text, bool active)
        {
            Color color = text.color;
            color.a = active ? 1.0f : _inactiveValue;
            text.color = color;
        }

        // Only set the RGB values, don't change the Alpha value.
        private void SetTextColor(TMP_Text text, Color color)
        {
            color.a = text.color.a;
            text.color = color;
        }
    }
}
