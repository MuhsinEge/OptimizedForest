using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceLocator
{
    public class Locator
    {
        private Locator() { }

        private readonly Dictionary<string, IService> services = new Dictionary<string, IService>();
        public static Locator Instance { get; private set; }

        public static void Initialize()
        {
            Instance = new Locator();
        }

        public T Get<T>() where T : IService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                Debug.LogError($"{key} not registered with {GetType().Name}");
                throw new InvalidOperationException();
            }

            return (T)services[key];
        }

        public void Register<T>(T service) where T : IService
        {
            string key = typeof(T).Name;
            if (services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to register service of type {key} which is already registered with the {GetType().Name}.");
                return;
            }

            services.Add(key, service);
        }

        public void Unregister<T>() where T : IService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered with the {GetType().Name}.");
                return;
            }

            services.Remove(key);
        }
    }
}