using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace C4U.Core
{
    /// <summary>
    /// An interface used for retrieving a <see cref="Scene"/> from the scenes in this container.
    /// </summary>
    public interface ISceneContainer
    {
        public Scene GetSceneByIndex(int index);
        public Scene GetSceneByName(string name);
    }

    /// <summary>
    /// A scriptable object used for storing scene data to be able to load scenes by name.
    /// </summary>
    [CreateAssetMenu(fileName = "Scene Container", menuName = "C4U/Create Scene Container")]
    public class SceneContainer : ScriptableObject, ISceneContainer
    {
        [SerializeField]
        private List<Scene> _scenes = new();

        /// <summary>
        /// Get a <see cref="Scene"/> by name.
        /// </summary>
        /// <param name="index">The build index of the scene.</param>
        /// <returns>A valid <see cref="Scene"/> or null.</returns>
        public Scene GetSceneByIndex(int index)
        {
            return _scenes.FirstOrDefault(scene => scene.Index == index);
        }

        /// <summary>
        /// Get a <see cref="Scene"/> by name.
        /// </summary>
        /// <param name="name">The name of the scene in the container.</param>
        /// <returns>A valid <see cref="Scene"/> or null.</returns>
        public Scene GetSceneByName(string name)
        {
            return _scenes.FirstOrDefault(scene => scene.Name == name);
        }

        /// <summary>
        /// Create a copy of this Scriptable Object
        /// to prevent altering the data from the Scriptable Object file itself.
        /// </summary>
        /// <returns></returns>
        public SceneContainer CreateInstance()
        {
            var container = Instantiate(this);

            foreach (var scene in _scenes)
            {
                var newScene = new Scene(scene.Name, scene.Index);

                container._scenes.Add(newScene);
            }

            return container;
        }
    }
}