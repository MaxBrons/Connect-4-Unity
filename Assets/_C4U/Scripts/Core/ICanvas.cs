using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace C4U
{
    public interface ICanvas
    {
        public static async Task<T> FetchCanvas<T>() where T : class, ICanvas
        {
            return await Task.FromResult(Object.FindObjectsOfType<MonoBehaviour>().FirstOrDefault(x => x is T) as T);
        }
    }
}