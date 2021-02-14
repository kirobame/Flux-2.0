using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Flux
{
    public static class Entities
    {
        #region Nested Types

        public struct UpdateECS { }

        public delegate void P<T1>(Entity entity, T1 argOne) where T1 : ISection;
        public delegate void PP<T1,T2>(Entity entity, T1 argOne, T2 argTwo) where T1 : ISection where T2 : ISection;
        public delegate void PPP<T1,T2,T3>(Entity entity, T1 argOne, T2 argTwo, T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void PPPP<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, T3 argThree, T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;

        public delegate void W<T1>(Entity entity, ref T1 argOne) where T1 : ISection;
        public delegate void WW<T1,T2>(Entity entity, ref T1 argOne, ref T2 argTwo) where T1 : ISection where T2 : ISection;
        public delegate void WWW<T1,T2,T3>(Entity entity, ref T1 argOne, ref T2 argTwo, ref T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void WWWW<T1,T2,T3,T4>(Entity entity, ref T1 argOne, ref T2 argTwo, ref T3 argThree, ref T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        
        public delegate void R<T1>(Entity entity, in T1 argOne) where T1 : ISection;
        public delegate void RR<T1,T2>(Entity entity, in T1 argOne, in T2 argTwo) where T1 : ISection where T2 : ISection;
        public delegate void RRR<T1,T2,T3>(Entity entity, in T1 argOne, in T2 argTwo, in T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void RRRR<T1,T2,T3,T4>(Entity entity, in T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;

        public delegate void PW<T1,T2>(Entity entity, T1 argOne, ref T2 argTwo) where T1 : ISection where T2 : ISection;
        public delegate void PR<T1,T2>(Entity entity, T1 argOne, in T2 argTwo) where T1 : ISection where T2 : ISection;
        public delegate void WR<T1,T2>(Entity entity, ref T1 argOne, in T2 argTwo) where T1 : ISection where T2 : ISection;
        
        public delegate void PPW<T1,T2,T3>(Entity entity, T1 argOne, T2 argTwo, ref T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void PWW<T1,T2,T3>(Entity entity, T1 argOne, ref T2 argTwo, ref T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void PPR<T1,T2,T3>(Entity entity, T1 argOne, T2 argTwo, in T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void PRR<T1,T2,T3>(Entity entity, T1 argOne, in T2 argTwo, in T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void PWR<T1,T2,T3>(Entity entity, T1 argOne, ref T2 argTwo, in T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void WWR<T1,T2,T3>(Entity entity, ref T1 argOne, ref T2 argTwo, in T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;
        public delegate void WRR<T1,T2,T3>(Entity entity, ref T1 argOne, in T2 argTwo, in T3 argThree) where T1 : ISection where T2 : ISection where T3 : ISection;

        public delegate void PPPW<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, T3 argThree, ref T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PPWW<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, ref T3 argThree, ref T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PWWW<T1,T2,T3,T4>(Entity entity, T1 argOne, ref T2 argTwo, ref T3 argThree, ref T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PPPR<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PPRR<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, in T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PRRR<T1,T2,T3,T4>(Entity entity, T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PPWR<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, ref T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PWWR<T1,T2,T3,T4>(Entity entity, T1 argOne, ref T2 argTwo, ref T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void PWRR<T1,T2,T3,T4>(Entity entity, T1 argOne, ref T2 argTwo, in T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void WWWR<T1,T2,T3,T4>(Entity entity, ref T1 argOne, ref T2 argTwo, ref T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void WWRR<T1,T2,T3,T4>(Entity entity, ref T1 argOne, ref T2 argTwo, in T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        public delegate void WRRR<T1,T2,T3,T4>(Entity entity, ref T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection;
        
        #endregion
        
        private static Dictionary<Type, Link> bridgeLinks;
        private static Dictionary<Type, Type[]> bridgeLookups;
        private static Dictionary<Entity, Dictionary<Type, HashSet<Type>>> dirtiedBridges;

        private static SystemUpdateWrapper systemRoot;
        
        private static Dictionary<Type, HashSet<Entity>> values;
        private static Dictionary<int, HashSet<Entity>> valuesByFlag;
        
        //---[Initialization methods]-----------------------------------------------------------------------------------/
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootup()
        {
            bridgeLinks = new Dictionary<Type, Link>();
            bridgeLookups = new Dictionary<Type, Type[]>();
            dirtiedBridges = new Dictionary<Entity, Dictionary<Type, HashSet<Type>>>();
            
            systemRoot = new DummySystemUpdateWrapper();
            
            values = new Dictionary<Type, HashSet<Entity>>();
            valuesByFlag = new Dictionary<int, HashSet<Entity>>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IBridge).IsAssignableFrom(type) && !type.IsInterface) RegisterBridgeType(type);
                    else if (typeof(System).IsAssignableFrom(type) && !type.IsAbstract) RegisterSystemType(type);
                }
            }
            
            systemRoot.Sort();
            
            var loop = PlayerLoop.GetDefaultPlayerLoop();
            loop = loop.InsertAt<Update>(new PlayerLoopSystem {updateDelegate = Update, type = typeof(UpdateECS)});
            
            PlayerLoop.SetPlayerLoop(loop);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize() => systemRoot.Initialize();
        
        private static void RegisterBridgeType(Type type)
        {
            var matches = new List<Type>();
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!typeof(IBridge).IsAssignableFrom(interfaceType) || interfaceType == typeof(IBridge)) continue;
                
                var bridgedType = interfaceType.GetGenericArguments()[0];
                matches.Add(bridgedType);

                if (bridgeLinks.ContainsKey(bridgedType)) continue;

                var linkType = typeof(Link<>).MakeGenericType(bridgedType);
                bridgeLinks.Add(bridgedType, (Link)Activator.CreateInstance(linkType));
            }

            if (matches.Any()) bridgeLookups.Add(type, matches.ToArray());
        }
        private static void RegisterSystemType(Type type)
        {
            var updateOrder = type.GetCustomAttribute<UpdateOrder>();
            if (updateOrder == null) updateOrder = new UpdateOrder("Root", "Any/Any");

            if (!systemRoot.TryFind(updateOrder.Parent, out var result)) return;
            result.Add(new ConcreteSystemUpdateWrapper(type, updateOrder.After, updateOrder.Before));
        }
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        static void Update()
        {
            systemRoot.Update();
            
            foreach (var kvp in dirtiedBridges)
            {
                foreach (var subKvp in kvp.Value)
                {
                    var bridge = kvp.Key[subKvp.Key];
                    foreach (var bridgedType in subKvp.Value)
                    {
                        var component = kvp.Key.BridgedComponents[bridgedType];
                        bridgeLinks[bridgedType].SendData(bridge, component);
                    }
                    
                    subKvp.Value.Clear();
                }
            }
        }

        #region ForEach

        public static void ForEach<T1>(P<T1> method, params IFlag[] flags) where T1 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(PP<T1,T2> method, params IFlag[] flags) where T1 : ISection where T2 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PPP<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPPP<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        public static void ForEach<T1>(W<T1> method, params IFlag[] flags) where T1 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(WW<T1,T2> method, params IFlag[] flags) where T1 : ISection where T2 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(WWW<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WWWW<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        public static void ForEach<T1>(R<T1> method, params IFlag[] flags) where T1 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(RR<T1,T2> method, params IFlag[] flags) where T1 : ISection where T2 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(RRR<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(RRRR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        public static void ForEach<T1,T2>(PW<T1,T2> method, params IFlag[] flags) where T1 : ISection where T2 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(PR<T1,T2> method, params IFlag[] flags) where T1 : ISection where T2 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(WR<T1,T2> method, params IFlag[] flags) where T1 : ISection where T2 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        
        public static void ForEach<T1,T2,T3>(PPW<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PWW<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PPR<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PRR<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PWR<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(WWR<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(WRR<T1,T2,T3> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        
        public static void ForEach<T1,T2,T3,T4>(PPPW<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPWW<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PWWW<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPPR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPRR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PRRR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPWR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PWWR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PWRR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WWWR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WWRR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WRRR<T1,T2,T3,T4> method, params IFlag[] flags) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        #endregion
        
        public static IEnumerable<Entity> Fetch(IEnumerable<IFlag> flags, params Type[] types)
        {
            var output = new HashSet<Entity>(values[types[0]]);

            for (var i = 1; i < types.Length; i++)
            {
                if (!values.TryGetValue(types[i], out var hashSet))
                {
                    output.Clear();
                    return output;
                }
                output.IntersectWith(hashSet);
            }
            foreach (var flag in flags)
            {
                if (!valuesByFlag.TryGetValue(flag.GetCode(), out var hashSet))
                {
                    output.Clear();
                    return output;
                }
                output.IntersectWith(hashSet);
            }

            return output;
        }
        
        //---[Entities lifetime hooks]----------------------------------------------------------------------------------/

        internal static void Register(Entity entity)
        {
            foreach (var section in entity.Sections) OnSectionAddition(entity, section);
            foreach (var flag in entity.Flags) OnFlagAddition(entity, flag);
            
            entity.onSectionAddition += OnSectionAddition;
            entity.onSectionRemoval += OnSectionRemoval;
            entity.onFlagAddition += OnFlagAddition;
            entity.onFlagRemoval += OnFlagRemoval;
        }
        internal static void Unregister(Entity entity)
        {
            foreach (var section in entity.Sections) OnSectionRemoval(entity, section);
            foreach (var flag in entity.Flags) OnFlagRemoval(entity, flag);
            
            entity.onSectionAddition -= OnSectionAddition;
            entity.onSectionRemoval -= OnSectionRemoval;
            entity.onFlagAddition -= OnFlagAddition;
            entity.onFlagRemoval -= OnFlagRemoval;
        }
        
        private static void OnSectionAddition(Entity entity, ISection section)
        {
            var key = section.GetType();
            if (!values.TryGetValue(key, out var hashSet))
            {
                hashSet = new HashSet<Entity>();
                values.Add(key, hashSet);
            }

            hashSet.Add(entity);
        }
        private static void OnSectionRemoval(Entity entity, ISection section) => values[section.GetType()].Remove(entity);
        
        private static void OnFlagAddition(Entity entity, IFlag flag)
        {
            var key = flag.GetCode();
            if (!valuesByFlag.TryGetValue(key, out var hashSet))
            {
                hashSet = new HashSet<Entity>();
                valuesByFlag.Add(key, hashSet);
            }

            hashSet.Add(entity);
        }
        private static void OnFlagRemoval(Entity entity, IFlag flag) => valuesByFlag[flag.GetCode()].Remove(entity);
        
        //---[Bridges handling]-----------------------------------------------------------------------------------------/
        
        internal static void TryInjectComponentData(Entity source, ISection section)
        {
            var key = section.GetType();
            if (!bridgeLookups.TryGetValue(key, out var bridgedTypes)) return;

            foreach (var bridgedType in bridgedTypes)
            {
                var component = source.AddBridgeTo(bridgedType);
                bridgeLinks[bridgedType].ReceiveData(section, component);
            }
        }

        public static void MarkDirty<T1>(Entity entity, IBridge bridge)
            where T1 : Component
        {
            MarkDirty(entity, bridge, typeof(T1));
        }
    
        public static void MarkDirty<T1,T2>(Entity entity, IBridge bridge)
            where T1 : Component
            where T2 : Component
        {
            MarkDirty(entity, bridge, typeof(T1), typeof(T2));
        }
    
        public static void MarkDirty<T1,T2 ,T3>(Entity entity, IBridge bridge)
            where T1 : Component
            where T2 : Component
            where T3 : Component
        {
            MarkDirty(entity, bridge, typeof(T1), typeof(T2), typeof(T3));
        }

        private static void MarkDirty(Entity entity, IBridge bridge, params Type[] bridgedTypes)
        {
            if (!dirtiedBridges.TryGetValue(entity, out var subDictionary))
            {
                subDictionary = new Dictionary<Type, HashSet<Type>>();
                dirtiedBridges.Add(entity, subDictionary);
            }
            var bridgeType = bridge.GetType();

            if (!subDictionary.TryGetValue(bridgeType, out var hashSet))
            {
                hashSet = new HashSet<Type>();
                subDictionary.Add(bridgeType, hashSet);
            }

            foreach (var bridgedType in bridgedTypes) hashSet.Add(bridgedType);
        }
    }
}