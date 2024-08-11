using System.Collections;
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
        /// <returns></returns>
        public static async Task LoadSceneAsync(int index)
        {
            // TODO: Change the indexing to a scene container scriptable object.
            if (SceneManager.GetActiveScene().buildIndex == index)
            {
                Debug.LogWarning(INDEX_WARNING);
                return;
            }

            // Using a TaskCompletionSource to be able to await the Task and use a coroutine without to much hassle.
            var source = new TaskCompletionSource<bool>();
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);

            var coroutine = ICore.StartCoroutine(AwaitSceneLoad(operation, source));

            if (coroutine != null)
                await source.Task;
        }

        /// <summary>
        /// Unload the scene with the given build index and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to unload.</param>
        /// <returns></returns>
        public static async Task UnloadSceneAsync(int index)
        {
            // Using a TaskCompletionSource to be able to await the Task and use a coroutine without to much hassle.
            var source = new TaskCompletionSource<bool>();
            AsyncOperation operation = SceneManager.UnloadSceneAsync(index);

            var coroutine = ICore.StartCoroutine(AwaitSceneLoad(operation, source));

            if (coroutine != null)
                await source.Task;
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
