using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Flux
{
    public static class Hub
    {
        public static IReadOnlyDictionary<int, LinkFactory> LinkFactories => linkFactories;
        private static Dictionary<int, LinkFactory> linkFactories;

        public static IReadOnlyDictionary<int, int[]> LinkLookups => linkLookups;
        private static Dictionary<int, int[]> linkLookups;

        private static Dictionary<int, HashSet<int>> entitiesLookups;
        private static Dictionary<int, Entity> entities;

        private static List<SystemWrapper> systemWrappers;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootup()
        {
            linkFactories = new Dictionary<int, LinkFactory>();
            linkLookups = new Dictionary<int, int[]>();
            
            systemWrappers = new List<SystemWrapper>();
        
            var matches = new List<int>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IBridge).IsAssignableFrom(type))
                    {
                        foreach (var interfaceType in type.GetInterfaces())
                        {
                            if (!typeof(IBridge).IsAssignableFrom(interfaceType) || interfaceType == typeof(IBridge)) continue;

                            var componentType = interfaceType.GetGenericArguments()[0];
                            var componentKey = componentType.GetHashCode();
                    
                            matches.Add(componentKey);
                            if (!linkFactories.ContainsKey(componentKey))
                            {
                                var factoryType = typeof(LinkFactory<>).MakeGenericType(componentType);
                                linkFactories.Add(componentKey, (LinkFactory)Activator.CreateInstance(factoryType));
                            }
                        }

                        if (matches.Any())
                        {
                            linkLookups.Add(type.GetHashCode(), matches.ToArray());
                            matches.Clear();
                        }
                    }

                    if (typeof(System).IsAssignableFrom(type) && type != typeof(System)) systemWrappers.Add(new SystemWrapper(type));
                }
            }
            systemWrappers.Sort();
            
            entitiesLookups = new Dictionary<int, HashSet<int>>();
            entities = new Dictionary<int, Entity>();

            var loop = PlayerLoop.GetDefaultPlayerLoop();
            loop = loop.InsertAt<Update>(new PlayerLoopSystem {updateDelegate = Update, type = typeof(UpdateECS)});
            
            PlayerLoop.SetPlayerLoop(loop);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Start()
        {
            foreach (var systemWrapper in systemWrappers) systemWrapper.Value.Initialize();
        }

        public static bool TryGetLinkLookupsFor(int key, out IEnumerable<int> linkLookups)
        {
            if (Hub.linkLookups.TryGetValue(key, out var output))
            {
                linkLookups = output;
                return true;
            }

            linkLookups = null;
            return false;
        }
        
        public static IEnumerable<Entity> Request(params Type[] types)
        {
            var output = new HashSet<int>(entitiesLookups[types[0].GetHashCode()]);
            for (var i = 1; i < types.Length; i++) output.IntersectWith(entitiesLookups[types[i].GetHashCode()]);

            return output.Select(item => entities[item]);
        }
        
        public static void Register(Entity entity)
        {
            var id = entity.GetInstanceID();
            entities.Add(id, entity);

            foreach (var key in entity.Keys)
            {
                if (!entitiesLookups.TryGetValue(key, out var entityLookups))
                {
                    entityLookups = new HashSet<int>();
                    entitiesLookups.Add(key, entityLookups);
                }
                
                entityLookups.Add(id);
            }
        }
        public static void Unregister(Entity entity)
        {
            var id = entity.GetInstanceID();
            foreach (var key in entity.Keys) entitiesLookups[key].Remove(id);

            entities.Remove(id);
        }
        
        private static void Update()
        {
            foreach (var systemWrapper in systemWrappers)
            {
                if (!systemWrapper.Value.IsActive) continue;
                systemWrapper.Value.Update();
            }
        }
    }
}