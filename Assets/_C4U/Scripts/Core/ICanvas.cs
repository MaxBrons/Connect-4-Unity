using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace C4U
{
    /// <summary>
    /// An interface used for retrieving a UI script when loading in a UI scene.
    /// </summary>
    public interface ICanvas
    {
        public static async Task<T> FetchCanvas<T>() where T : class, ICanvas
        {
            return await Task.FromResult(Object.FindObjectsOfType<MonoBehaviour>().FirstOrDefault(x => x is T) as T);
        }
    }
}