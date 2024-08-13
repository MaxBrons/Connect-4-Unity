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
        // TODO:
        // 1. Move Core to the services scene.
        // 2. Create scene loading container.
        // 3. Load game via services scene.

        private void Awake()
        {
            ICore._coroutineHelper = this;

            GameState.AddPlayer(new Player(0));
            GameState.AddPlayer(new Player(1));
        }
    }
}
