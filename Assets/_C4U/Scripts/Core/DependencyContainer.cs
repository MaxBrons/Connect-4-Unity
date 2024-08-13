using System;
using System.Collections.Generic;
using System.Linq;

namespace C4U.Core
{
    public interface IDependencyContainer
    {
        public T Get<T>();
        public void Add<T>(object value);
    }

    public class DependencyContainer : IDependencyContainer
    {
        private Dictionary<Type, object> _container = new();

        public T Get<T>()
        {
            return (T)_container.FirstOrDefault(x => x.Key == typeof(T)).Value;
        }

        public void Add<T>(object value)
        {
            if (value != null)
            {
                _container.Add(typeof(T), value);
            }
        }
    }
}