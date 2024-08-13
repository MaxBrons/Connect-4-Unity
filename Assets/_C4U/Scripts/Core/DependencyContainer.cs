using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace C4U.Core
{
    public interface IDependencyContainer
    {
        public Task<T> Get<T>();
        public Task Add<T>(object value);
    }

    public class DependencyContainer : IDependencyContainer
    {
        private Dictionary<Type, object> _container = new();

        public async Task<T> Get<T>()
        {
            return await Task.FromResult((T)_container.FirstOrDefault(x => x.Key == typeof(T)).Value);
        }

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