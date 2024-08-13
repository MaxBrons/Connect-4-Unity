using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C4U.Core.SceneManagement
{
    /// <summary>
    /// Used to log error when failing to load scene.
    /// </summary>
    public struct SceneLoadResult
    {
        public bool Success;
        public string Message;

        public SceneLoadResult(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }
    }

    /// <summary>
    /// Class for asynchronously (un)loading a scene.
    /// </summary>
    public static class SceneLoader
    {
        private static List<Scene> _activeScenes = new();

        private const string ERROR_PREFIX = "Failed to load/unload scene: ";

        /// <summary>
        /// Load the scene with the given build index and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to load.</param>
        /// <param name="options">The load options to use when loading the scene.</param>
        /// <returns></returns>
        public static async Task LoadSceneAsync(int index, LoadSceneMode options = LoadSceneMode.Additive)
        {
            // Get the Scene Container.
            ISceneContainer container = await ICore.Container.Get<ISceneContainer>();

            // Get the scene by index and load the scene if it's valid.
            var scene = container.GetSceneByIndex(index);

            await LoadSceneAsync_Internal(scene, options);
        }

        /// <summary>
        /// Load the scene with the given name and wait until the process is finished.
        /// </summary>
        /// <param name="name">The build index of the scene to load.</param>
        /// /// <param name="options">The load options to use when loading the scene.</param>
        /// <returns></returns>
        public static async Task LoadSceneAsync(string name, LoadSceneMode options = LoadSceneMode.Additive)
        {
            // Get the Scene Container.
            ISceneContainer container = await ICore.Container.Get<ISceneContainer>();

            // Get the scene by name and load the scene if it's valid.
            var scene = container.GetSceneByName(name);

            await LoadSceneAsync_Internal(scene, options);
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

            return await ICanvas.FetchCanvas<T>();
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

            return await ICanvas.FetchCanvas<T>();
        }

        /// <summary>
        /// Unload the scene with the given build index and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to unload.</param>
        /// <returns></returns>
        public static async Task UnloadSceneAsync(int index, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            // Get the Scene Container.
            ISceneContainer container = await ICore.Container.Get<ISceneContainer>();

            // Check if the scene is valid.
            var scene = container.GetSceneByIndex(index);

            await UnloadSceneAsync_Internal(scene, options);
        }

        /// <summary>
        /// Unload the scene with the given name and wait until the process is finished.
        /// </summary>
        /// <param name="index">The build index of the scene to unload.</param>
        /// <param name="options">The unload options to use when unloading the scen.</param>
        /// <returns></returns>
        public static async Task UnloadSceneAsync(string name, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            // Get the Scene Container.
            ISceneContainer container = await ICore.Container.Get<ISceneContainer>();

            // Check if the scene is valid and unload it.
            var scene = container.GetSceneByName(name);

            await UnloadSceneAsync_Internal(scene, options);
        }

        // Do the scene loading and error handling here.
        private static async Task LoadSceneAsync_Internal(Scene scene, LoadSceneMode options)
        {
            // Check if the scene is not null.
            if (scene == null)
            {
                LogError("Scene was null.");
                return;
            }

            // Get the Scene Container.
            ISceneContainer container = await ICore.Container.Get<ISceneContainer>();

            // Check if the scene is not already loaded in.
            if (_activeScenes.Any(x => x.Index == scene.Index))
                return;

            // Using a TaskCompletionSource to be able to await the Task and use a coroutine without to much hassle.
            var source = new TaskCompletionSource<bool>();
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.Index, options);

            // Load in the scene.
            var coroutine = ICore.StartCoroutine(AwaitSceneLoad(operation, source));

            if (coroutine != null)
            {
                await source.Task;

                _activeScenes.Add(scene);
                return;
            }

            LogError("Scene loading coroutine failed.");
        }

        // Do the scene unloading and error handling here.
        private static async Task UnloadSceneAsync_Internal(Scene scene, UnloadSceneOptions options)
        {
            // Check if the scene is not null.
            if (scene == null)
            {
                LogError("Scene was null.");
                return;
            }

            // Using a TaskCompletionSource to be able to await the Task and use a coroutine without to much hassle.
            var source = new TaskCompletionSource<bool>();
            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene.Index, options);

            var coroutine = ICore.StartCoroutine(AwaitSceneLoad(operation, source));

            // Unload the valid scene.
            if (coroutine != null)
            {
                await source.Task;

                _activeScenes.Remove(scene);
                return;
            }

            LogError("Scene unloading coroutine failed.");
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

        private static void LogError(string message)
        {
            Debug.LogError(ERROR_PREFIX + message);
        }
    }
}
