using C4U.Core.SceneManagement;
using C4U.Game;
using System.Collections;
using UnityEngine;

namespace C4U.Core
{
    /// <summary>
    /// Interface for important/usefull methods. <br/>
    /// Make sure to have an ICore MonoBehaviour script in your scene to make use of some methods.
    /// </summary>
    public interface ICore
    {
        public static IDependencyContainer Container;

        protected static MonoBehaviour _coroutineHelper;

        /// <summary>
        /// Unity only allows Coroutines to be called from a MonoBehaviour. 
        /// To be able to run a Coroutine in other scripts, you reference a MonoBehaviour script that preferably
        /// lives for the entire duration of the game.
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            Debug.Assert(_coroutineHelper != null,
                "No coroutine helper set in ICore Interface but you are trying to access it! Try adding the 'Core' component to the scene.");

            if (_coroutineHelper != null)
                return _coroutineHelper.StartCoroutine(coroutine);

            return null;
        }
    }

    /// <summary>
    /// Implementation of ICore to be able to use the Coroutine functionality for now.
    /// </summary>
    public class Core : MonoBehaviour, ICore
    {
        [SerializeField] private SceneContainer _sceneContainer;

        private IGameState _gameState;

        private async void Awake()
        {
            _gameState = new GameState();

            ICore._coroutineHelper = this;
            ICore.Container = new DependencyContainer();

            await ICore.Container.Add<IGameState>(_gameState);
            await ICore.Container.Add<ISceneContainer>(_sceneContainer.CreateInstance());

            _gameState.AddPlayer(new Player(0));
            _gameState.AddPlayer(new Player(1));

            await SceneLoader.LoadSceneAsync("MainMenu");
        }
    }
}
