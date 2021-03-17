using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Flux.EDS
{
    public static class Entities
    {
        private static bool Insert(this UpdateRelay parent, out UpdateRelay output, string[] chain, int index = 0)
        {
            if (index == chain.Length)
            {
                output = null;
                return false;
            }

            output = parent.Childs.FirstOrDefault(child => child.Name == chain[index]);
            if (output == null)
            {
                output = new UpdateRelay(chain[index]);
                parent.Add(output);
            }

            if (Insert(output, out var subOutput, chain, index + 1)) output = subOutput;
            return true;
        }
        private static bool TryFind(this UpdateRelay value, string name, out UpdateRelay result)
        {
            if (value.Name == name)
            {
                result = value;
                return true;
            }

            foreach (var child in value.Childs)
            {
                if (child.TryFind(name, out result)) return true;
            }

            result = null;
            return false;
        }
        private static string Print(this UpdateRelay value, string output = "", int depth = 0)
        {
            for (var i = 0; i < depth; i++)
            {
                for (var j = 0; j < 3; j++) output += "-";
            }
            output += $"|{value.Name}\n";

            foreach (var child in value.Childs) output = child.Print(output, depth + 1);

            return output;
        }
        
        #region Nested Types

        public struct UpdateECS { }

        public delegate void P<T1>(Entity entity, T1 argOne) where T1 : IData;
        public delegate void PP<T1,T2>(Entity entity, T1 argOne, T2 argTwo) where T1 : IData where T2 : IData;
        public delegate void PPP<T1,T2,T3>(Entity entity, T1 argOne, T2 argTwo, T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void PPPP<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, T3 argThree, T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;

        public delegate void W<T1>(Entity entity, ref T1 argOne) where T1 : IData;
        public delegate void WW<T1,T2>(Entity entity, ref T1 argOne, ref T2 argTwo) where T1 : IData where T2 : IData;
        public delegate void WWW<T1,T2,T3>(Entity entity, ref T1 argOne, ref T2 argTwo, ref T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void WWWW<T1,T2,T3,T4>(Entity entity, ref T1 argOne, ref T2 argTwo, ref T3 argThree, ref T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        
        public delegate void R<T1>(Entity entity, in T1 argOne) where T1 : IData;
        public delegate void RR<T1,T2>(Entity entity, in T1 argOne, in T2 argTwo) where T1 : IData where T2 : IData;
        public delegate void RRR<T1,T2,T3>(Entity entity, in T1 argOne, in T2 argTwo, in T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void RRRR<T1,T2,T3,T4>(Entity entity, in T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;

        public delegate void PW<T1,T2>(Entity entity, T1 argOne, ref T2 argTwo) where T1 : IData where T2 : IData;
        public delegate void PR<T1,T2>(Entity entity, T1 argOne, in T2 argTwo) where T1 : IData where T2 : IData;
        public delegate void WR<T1,T2>(Entity entity, ref T1 argOne, in T2 argTwo) where T1 : IData where T2 : IData;
        
        public delegate void PPW<T1,T2,T3>(Entity entity, T1 argOne, T2 argTwo, ref T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void PWW<T1,T2,T3>(Entity entity, T1 argOne, ref T2 argTwo, ref T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void PPR<T1,T2,T3>(Entity entity, T1 argOne, T2 argTwo, in T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void PRR<T1,T2,T3>(Entity entity, T1 argOne, in T2 argTwo, in T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void PWR<T1,T2,T3>(Entity entity, T1 argOne, ref T2 argTwo, in T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void WWR<T1,T2,T3>(Entity entity, ref T1 argOne, ref T2 argTwo, in T3 argThree) where T1 : IData where T2 : IData where T3 : IData;
        public delegate void WRR<T1,T2,T3>(Entity entity, ref T1 argOne, in T2 argTwo, in T3 argThree) where T1 : IData where T2 : IData where T3 : IData;

        public delegate void PPPW<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, T3 argThree, ref T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PPWW<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, ref T3 argThree, ref T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PWWW<T1,T2,T3,T4>(Entity entity, T1 argOne, ref T2 argTwo, ref T3 argThree, ref T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PPPR<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PPRR<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, in T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PRRR<T1,T2,T3,T4>(Entity entity, T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PPWR<T1,T2,T3,T4>(Entity entity, T1 argOne, T2 argTwo, ref T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PWWR<T1,T2,T3,T4>(Entity entity, T1 argOne, ref T2 argTwo, ref T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void PWRR<T1,T2,T3,T4>(Entity entity, T1 argOne, ref T2 argTwo, in T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void WWWR<T1,T2,T3,T4>(Entity entity, ref T1 argOne, ref T2 argTwo, ref T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void WWRR<T1,T2,T3,T4>(Entity entity, ref T1 argOne, ref T2 argTwo, in T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        public delegate void WRRR<T1,T2,T3,T4>(Entity entity, ref T1 argOne, in T2 argTwo, in T3 argThree, in T4 argFour) where T1 : IData where T2 : IData where T3 : IData where T4 : IData;
        
        #endregion
        
        private static Dictionary<Type, Link> bridgeLinks;
        private static Dictionary<Type, Type[]> bridgeLookups;
        private static Dictionary<Entity, Dictionary<Type, HashSet<Type>>> dirtiedBridges;

        private static UpdateRelay root;
        
        private static Dictionary<Type, HashSet<Entity>> values;
        private static Dictionary<int, HashSet<Entity>> flaggedValues;

        private static FlagTranslator flagTranslator;

        private static HashSet<Command> commands;
        
        //---[Initialization methods]-----------------------------------------------------------------------------------/
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootup()
        {
            bridgeLinks = new Dictionary<Type, Link>();
            bridgeLookups = new Dictionary<Type, Type[]>();
            dirtiedBridges = new Dictionary<Entity, Dictionary<Type, HashSet<Type>>>();
            
            root = new UpdateRelay("Root");

            values = new Dictionary<Type, HashSet<Entity>>();
            flaggedValues = new Dictionary<int, HashSet<Entity>>();
            
            flagTranslator = new FlagTranslator();
            
            commands= new HashSet<Command>();

            var queuedRelays = new Dictionary<string, List<UpdateRelay>>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(ILink).IsAssignableFrom(type) && !type.IsInterface) RegisterBridgeType(type);
                    else if (typeof(System).IsAssignableFrom(type) && !type.IsAbstract) RegisterSystemType(type, queuedRelays);
                }
            }
            
            root.Sort();

            var loop = PlayerLoop.GetDefaultPlayerLoop();
            loop = loop.InsertAt<Update>(new PlayerLoopSystem {updateDelegate = Update, type = typeof(UpdateECS)});
            
            PlayerLoop.SetPlayerLoop(loop);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize() => root.Initialize();
        
        private static void RegisterBridgeType(Type type)
        {
            var matches = new List<Type>();
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!typeof(ILink).IsAssignableFrom(interfaceType) || interfaceType == typeof(ILink)) continue;
                
                var bridgedType = interfaceType.GetGenericArguments()[0];
                matches.Add(bridgedType);

                if (bridgeLinks.ContainsKey(bridgedType)) continue;

                var linkType = typeof(Link<>).MakeGenericType(bridgedType);
                bridgeLinks.Add(bridgedType, (Link)Activator.CreateInstance(linkType));
            }

            if (matches.Any()) bridgeLookups.Add(type, matches.ToArray());
        }

        private static void RegisterSystemType(Type type, Dictionary<string, List<UpdateRelay>> queuedRelays)
        {
            string[] chain;
            
            var updateGroup = type.GetCustomAttribute<GroupAttribute>();
            if (updateGroup != null)
            {
                chain = updateGroup.Path.Split('/');
                root.Insert(out var group, chain);
                
                group.SetOrder(updateGroup.After, updateGroup.Before);
            }
            
            var updateOrder = type.GetCustomAttribute<OrderAttribute>();
            if (updateOrder == null)
            {
                var defaultRelay = new UpdateRelay(type.Name);
                
                defaultRelay.SetOrder("Any", "Any");
                defaultRelay.Inject((System)Activator.CreateInstance(type));
                
                root.Add(defaultRelay);
                return;
            }

            chain = updateOrder.Path.Split('/');
            root.Insert(out var relay, chain);

            relay.SetOrder(updateOrder.After, updateOrder.Before);
            relay.Inject((System)Activator.CreateInstance(type));
        }

        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        static void Update()
        {
            root.Update();
            
            #if UNITY_EDITOR
            if (!Application.isPlaying) return;
            #endif
            
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
            
            Sync();
        }

        public static void Sync() => ExecuteCommands();

        #region ForEach

        public static void ForEach<T1>(P<T1> method, params Enum[] flags) where T1 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(PP<T1,T2> method, params Enum[] flags) where T1 : IData where T2 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PPP<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPPP<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        public static void ForEach<T1>(W<T1> method, params Enum[] flags) where T1 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(WW<T1,T2> method, params Enum[] flags) where T1 : IData where T2 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(WWW<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WWWW<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        public static void ForEach<T1>(R<T1> method, params Enum[] flags) where T1 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(RR<T1,T2> method, params Enum[] flags) where T1 : IData where T2 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(RRR<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(RRRR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        public static void ForEach<T1,T2>(PW<T1,T2> method, params Enum[] flags) where T1 : IData where T2 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(PR<T1,T2> method, params Enum[] flags) where T1 : IData where T2 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        public static void ForEach<T1,T2>(WR<T1,T2> method, params Enum[] flags) where T1 : IData where T2 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2))) entity.Relay(method);
        }
        
        public static void ForEach<T1,T2,T3>(PPW<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PWW<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PPR<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PRR<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(PWR<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(WWR<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3>(WRR<T1,T2,T3> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3))) entity.Relay(method);
        }
        
        public static void ForEach<T1,T2,T3,T4>(PPPW<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPWW<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PWWW<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPPR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPRR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PRRR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PPWR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PWWR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(PWRR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WWWR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WWRR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        public static void ForEach<T1,T2,T3,T4>(WRRR<T1,T2,T3,T4> method, params Enum[] flags) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            foreach (var entity in Fetch(flags, typeof(T1), typeof(T2), typeof(T3), typeof(T4))) entity.Relay(method);
        }
        
        #endregion

        public static int TranslateFlag(Enum flag) => flagTranslator.Translate(flag);
        public static IEnumerable<Entity> Fetch(IEnumerable<Enum> flags, params Type[] types)
        {
            if (!values.TryGetValue(types[0], out var output))
            {
                output = new HashSet<Entity>();
                return output;
            }
            output = new HashSet<Entity>(output);

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
                if (!flaggedValues.TryGetValue(flagTranslator.Translate(flag), out var hashSet))
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
            foreach (var section in entity.Table) OnDataAddition(entity, section);
            foreach (var flag in entity.Flags) OnFlagAddition(entity, flag.Value);
            
            entity.onDataAddition += OnDataAddition;
            entity.onDataRemoval += OnDataRemoval;
            entity.onFlagAddition += OnFlagAddition;
            entity.onFlagRemoval += OnFlagRemoval;
        }
        internal static void Unregister(Entity entity)
        {
            foreach (var section in entity.Table) OnDataRemoval(entity, section);
            foreach (var flag in entity.Flags) OnFlagRemoval(entity, flag.Value);
            
            entity.onDataAddition -= OnDataAddition;
            entity.onDataRemoval -= OnDataRemoval;
            entity.onFlagAddition -= OnFlagAddition;
            entity.onFlagRemoval -= OnFlagRemoval;
        }
        
        private static void OnDataAddition(Entity entity, IData data)
        {
            var key = data.GetType();
            if (!values.TryGetValue(key, out var hashSet))
            {
                hashSet = new HashSet<Entity>();
                values.Add(key, hashSet);
            }

            hashSet.Add(entity);
        }
        private static void OnDataRemoval(Entity entity, IData data) => commands.Add(new DataRemoval(entity, data.GetType()));
        internal static void RemoveData(Entity entity, Type dataType) => values[dataType].Remove(entity);
        
        private static void OnFlagAddition(Entity entity, Enum flag)
        {
            var key = flagTranslator.Translate(flag);

            if (!flaggedValues.TryGetValue(key, out var hashSet))
            {
                hashSet = new HashSet<Entity>();
                flaggedValues.Add(key, hashSet);
            }

            hashSet.Add(entity);
        }
        private static void OnFlagRemoval(Entity entity, Enum flag) => flaggedValues[flagTranslator.Translate(flag)].Remove(entity);
        
        //---[Bridges handling]-----------------------------------------------------------------------------------------/
        
        internal static void TryInjectComponentData(Entity source, IData data)
        {
            var key = data.GetType();
            if (!bridgeLookups.TryGetValue(key, out var bridgedTypes)) return;

            foreach (var bridgedType in bridgedTypes)
            {
                var component = source.AddBridgeTo(bridgedType);
                bridgeLinks[bridgedType].ReceiveData(data, component);
            }
        }

        public static void MarkDirty<T1>(Entity entity, ILink bridge)
            where T1 : Component
        {
            MarkDirty(entity, bridge, typeof(T1));
        }
    
        public static void MarkDirty<T1,T2>(Entity entity, ILink bridge)
            where T1 : Component
            where T2 : Component
        {
            MarkDirty(entity, bridge, typeof(T1), typeof(T2));
        }
    
        public static void MarkDirty<T1,T2 ,T3>(Entity entity, ILink bridge)
            where T1 : Component
            where T2 : Component
            where T3 : Component
        {
            MarkDirty(entity, bridge, typeof(T1), typeof(T2), typeof(T3));
        }

        private static void MarkDirty(Entity entity, ILink bridge, params Type[] bridgedTypes)
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
        
        //---[Commands]-------------------------------------------------------------------------------------------------/
        
        private static void ExecuteCommands()
        {
            if (!commands.Any()) return;
            
            foreach(var command in commands) command.Execute();
            commands.Clear();
        }
    }
}