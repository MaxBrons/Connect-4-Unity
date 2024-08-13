using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace C4U
{
    public interface IEndSceneUI : ICanvas
    {
        public void SetWinnerText(string text);
        public void SetButtonAction(UnityAction buttonPressedEvent);
    }

    public class EndSceneUI : MonoBehaviour, IEndSceneUI
    {
        [SerializeField] private TMP_Text _winnerText;
        [SerializeField] private Button _button;

        public void SetButtonAction(UnityAction buttonPressedEvent)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(buttonPressedEvent);
        }

        public void SetWinnerText(string text)
        {
            _winnerText.text = text;
        }
    }
}
