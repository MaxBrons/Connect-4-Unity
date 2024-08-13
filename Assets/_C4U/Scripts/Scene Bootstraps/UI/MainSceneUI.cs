using TMPro;
using UnityEngine;

namespace C4U
{
    public interface IMainSceneUI : ICanvas
    {
        public void SetActivePlayer(int index);
        public void SetPlayerColor(int index, Color color);
    }

    public class MainSceneUI : MonoBehaviour, IMainSceneUI
    {
        [SerializeField] private TMP_Text _player1Text;
        [SerializeField] private TMP_Text _player2Text;

        [Range(0.1f, 1)]
        [SerializeField] private float _inactiveValue = 0.1f;

        public void SetActivePlayer(int index)
        {
            ChangeTextColor(_player1Text, index == 0);
            ChangeTextColor(_player2Text, index == 1);
        }

        public void SetPlayerColor(int index, Color color)
        {
            TMP_Text text = index == 0 ? _player1Text : _player2Text;
            SetTextColor(text, color);
        }

        private void ChangeTextColor(TMP_Text text, bool active)
        {
            Color color = text.color;
            color.a = active ? 1.0f : _inactiveValue;
            text.color = color;
        }

        private void SetTextColor(TMP_Text text, Color color)
        {
            color.a = text.color.a;
            text.color = color;
        }
    }
}
