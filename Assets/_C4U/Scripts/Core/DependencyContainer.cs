using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace C4U.Core
{
    /// <summary>
    /// An interface to implement a generic async 'scratchpad'.
    /// </summary>
    public interface IDependencyContainer
    {
        public Task<T> Get<T>();
        public Task Add<T>(object value);
    }

    /// <summary>
    /// Implementation of <see cref="IDependencyContainer"/> to store the game's dependencies.
    /// </summary>
    public class DependencyContainer : IDependencyContainer
    {
        private Dictionary<Type, object> _container = new();

        /// <summary>
        /// Get a value from the container based on the given Type.
        /// </summary>
        /// <typeparam name="T">The type of the value you want to retrieve.</typeparam>
        /// <returns></returns>
        public async Task<T> Get<T>()
        {
            return await Task.FromResult((T)_container.FirstOrDefault(x => x.Key == typeof(T)).Value);
        }

        /// <summary>
        /// Add a value to the container based on the given Type. <br/>
        /// NOTE: There can only be one dependency per Type.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="value">The corresponding value of type <typeparamref name="T"/>.</param>
        /// <returns></returns>
        public async Task Add<T>(object value)
        {
            await Task.Run(() =>
            {
                if (value != null)
                {
                    _container.Add(typeof(T), value);
                }
            });
        }
    }
}