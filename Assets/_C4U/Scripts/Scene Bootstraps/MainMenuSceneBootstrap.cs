using C4U.Core.SceneManagement;
using UnityEngine;

namespace C4U
{
    public class MainMenuSceneBootstrap : MonoBehaviour
    {
        private IMainMenuUI _mainMenuUI;

        async void Start()
        {
            _mainMenuUI = await ICanvas.FetchCanvas<IMainMenuUI>();
            _mainMenuUI.SetPlayButtonAction(OnPlayButtonPressed);
            _mainMenuUI.SetQuitButtonAction(OnQuitButtonPressed);
        }

        private async void OnPlayButtonPressed()
        {
            await SceneLoader.UnloadSceneAsync("MainMenu");
            await SceneLoader.LoadSceneAsync("Main");
        }

        private void OnQuitButtonPressed()
        {
            Application.Quit();
        }
    }
}
