using C4U.Core.SceneManagement;
using UnityEngine;

namespace C4U
{
    // Setup the main menu UI and load in the main scene when the main menu button is pressed.
    public class MainMenuSceneBootstrap : MonoBehaviour
    {
        private IMainMenuUI _mainMenuUI;

        async void Start()
        {
            _mainMenuUI = await ICanvas.FetchCanvas<IMainMenuUI>();
            _mainMenuUI.SetPlayButtonAction(OnPlayButtonPressed);
            _mainMenuUI.SetQuitButtonAction(OnQuitButtonPressed);
        }

        // Open main level on play button press.
        private async void OnPlayButtonPressed()
        {
            await SceneLoader.UnloadSceneAsync("MainMenu");
            await SceneLoader.LoadSceneAsync("Main");
        }

        // Quit the game on quit button press.
        private void OnQuitButtonPressed()
        {
            Application.Quit();
        }
    }
}
