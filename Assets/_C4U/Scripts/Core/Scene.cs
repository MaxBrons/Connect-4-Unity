using UnityEngine;

namespace C4U.Core
{
    /// <summary>
    /// Used to store a scene's name and build index in the <see cref="IDependencyContainer"/>.
    /// </summary>
    [System.Serializable]
    public class Scene
    {
        public string Name => _name;
        public int Index => _index;

        [SerializeField] private string _name;
        [SerializeField] private int _index;

        public Scene(string name, int index)
        {
            _name = name;
            _index = index;
        }
    }
}