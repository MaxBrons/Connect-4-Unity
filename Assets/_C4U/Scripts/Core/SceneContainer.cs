using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace C4U.Core
{
    public interface ISceneContainer
    {
        public Scene GetSceneByIndex(int index);
        public Scene GetSceneByName(string name);
    }

    [CreateAssetMenu(fileName = "Scene Container", menuName = "C4U/Create Scene Container")]
    public class SceneContainer : ScriptableObject, ISceneContainer
    {
        [SerializeField]
        private List<Scene> _scenes = new();

        public Scene GetSceneByIndex(int index)
        {
            return _scenes.FirstOrDefault(scene => scene.Index == index);
        }

        public Scene GetSceneByName(string name)
        {
            return _scenes.FirstOrDefault(scene => scene.Name == name);
        }

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