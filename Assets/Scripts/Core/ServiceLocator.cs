using System;
using System.Collections.Generic;

namespace Labyrinth.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Clear()
        {
            Services.Clear();
        }

        public static void Register<T>(T service)
        {
            Services[typeof(T)] = service;
        }

        public static T Get<T>()
        {
            if (Services.TryGetValue(typeof(T), out object service))
                return (T)service;

            throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
        }

        public static bool TryGet<T>(out T service)
        {
            if (Services.TryGetValue(typeof(T), out object raw))
            {
                service = (T)raw;
                return true;
            }

            service = default;
            return false;
        }
    }
}
