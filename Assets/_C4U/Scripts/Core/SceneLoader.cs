using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C4U.Core.SceneManagement
{
    /// <summary>
    /// Class for asynchronously (un)loading a scene.
    /// </summary>
    public static class SceneLoader
    {
        private const string INDEX_WARNING = "Warning: You are trying to load a scene that is already active.";

        /// <summary>
        /// Load the scene with the given build index and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to load.</param>
        /// <param name="options">The load options to use when loading the scene.</param>
        /// <returns></returns>
        public static async Task LoadSceneAsync(int index, LoadSceneMode options = LoadSceneMode.Single)
        {
            // TODO: Change the indexing to a scene container scriptable object.
            if (SceneManager.GetActiveScene().buildIndex == index)
            {
                Debug.LogWarning(INDEX_WARNING);
                return;
            }

            // Using a TaskCompletionSource to be able to await the Task and use a coroutine without to much hassle.
            var source = new TaskCompletionSource<bool>();
            AsyncOperation operation = SceneManager.LoadSceneAsync(index, options);

            var coroutine = ICore.StartCoroutine(AwaitSceneLoad(operation, source));

            if (coroutine != null)
            {
                await source.Task;
            }
        }

        /// <summary>
        /// Load the scene with the given name and wait until the process is finished.
        /// </summary>
        /// <param name="name">The build index of the scene to load.</param>
        /// /// <param name="options">The load options to use when loading the scene.</param>
        /// <returns></returns>
        public static async Task LoadSceneAsync(string name, LoadSceneMode options = LoadSceneMode.Single)
        {
            var scene = SceneManager.GetSceneByName(name);

            if (scene.IsValid())
            {
                await LoadSceneAsync(scene.buildIndex, options);
            }
        }

        /// <summary>
        /// Load a Canvas scene in and return the ICanvas component that should be active in the scene.
        /// </summary>
        /// <typeparam name="T">Type of the ICanvas component to get in the scene.</typeparam>
        /// <param name="index">The index of the Canvas scene to load.</param>
        /// <returns></returns>
        public static async Task<T> LoadCanvasSceneAsync<T>(int index) where T : class, ICanvas
        {
            await LoadSceneAsync(index, LoadSceneMode.Additive);

            return Object.FindObjectsOfType<MonoBehaviour>().FirstOrDefault(x => x.GetType().Equals(typeof(T))) as T;
        }

        /// <summary>
        /// Load a Canvas scene in and return the ICanvas component that should be active in the scene.
        /// </summary>
        /// <typeparam name="T">Type of the ICanvas component to get in the scene.</typeparam>
        /// <param name="name">The name of the Canvas scene to load.</param>
        /// <returns></returns>
        public static async Task<T> LoadCanvasSceneAsync<T>(string name) where T : class, ICanvas
        {
            await LoadSceneAsync(name, LoadSceneMode.Additive);

            return Object.FindObjectsOfType<MonoBehaviour>().FirstOrDefault(x => x.GetType().Equals(typeof(T))) as T;
        }

        /// <summary>
        /// Unload the scene with the given build index and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to unload.</param>
        /// <returns></returns>
        public static async Task UnloadSceneAsync(int index, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            // Using a TaskCompletionSource to be able to await the Task and use a coroutine without to much hassle.
            var source = new TaskCompletionSource<bool>();
            AsyncOperation operation = SceneManager.UnloadSceneAsync(index, options);

            var coroutine = ICore.StartCoroutine(AwaitSceneLoad(operation, source));

            if (coroutine != null)
            {
                await source.Task;
            }
        }

        /// <summary>
        /// Unload the scene with the given name and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to unload.</param>
        /// <param name="options">The unload options to use when unloading the scen.</param>
        /// <returns></returns>
        public static async Task UnloadSceneAsync(string name, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            var scene = SceneManager.GetSceneByName(name);

            if (scene.IsValid())
            {
                await UnloadSceneAsync(scene.buildIndex, options);
            }
        }

        // Wait until Unity is done loading the scene.
        private static IEnumerator AwaitSceneLoad(AsyncOperation operation, TaskCompletionSource<bool> source)
        {
            while (!operation.isDone)
            {
                yield return null;
            }

            source.SetResult(true);
        }
    }
}
