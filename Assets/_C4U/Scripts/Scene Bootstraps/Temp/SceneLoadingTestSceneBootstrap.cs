using C4U.Core.SceneManagement;
using UnityEngine;

namespace C4U
{
    public class SceneLoadingTestSceneBootstrap : MonoBehaviour
    {
        // Load in the second scene and check if this bool is flipped.
        private static bool s_loaded = false;

        async void Start()
        {
            if (!s_loaded)
            {
                s_loaded = true;
                await SceneLoader.LoadSceneAsync(1);
                return;
            }

            print("LOADED!");
        }
    }
}
