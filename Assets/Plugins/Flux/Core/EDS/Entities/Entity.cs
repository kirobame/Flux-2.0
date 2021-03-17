using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Flux.EDS
{
    public class Entity : MonoBehaviour
    {
        internal event Action<Entity, IData> onDataAddition;
        internal event Action<Entity, IData> onDataRemoval;
        
        internal event Action<Entity, Enum> onFlagAddition;
        internal event Action<Entity, Enum> onFlagRemoval;
        
        //---[Data]-----------------------------------------------------------------------------------------------------/

        public IReadOnlyList<IFlag> Flags => flags;
        [Explicit, SerializeReference] private List<IFlag> flags = new List<IFlag>();

        public IReadOnlyList<IData> Table => table;
        [Explicit, SerializeReference] private List<IData> table = new List<IData>();

        private Dictionary<Type, int> lookups;
        
        internal IReadOnlyDictionary<Type, Component> BridgedComponents => bridgedComponents;
        private Dictionary<Type, Component> bridgedComponents;

        //---[Accessors]------------------------------------------------------------------------------------------------/
        
        public IData this[Type type]
        {
            get => table[lookups[type]];
            set => table[lookups[type]] = value;
        }

        public T GetSection<T>() where T : IData => (T)this[typeof(T)];
        public void SetSection(IData data) => this[data.GetType()] = data;

        //---[Lifetime callbacks]---------------------------------------------------------------------------------------/
        
        void Awake()
        {
            var query = flags.Where(flag => flag is IBootable).Cast<IBootable>()
                                             .Concat(table.Where(data => data is IBootable).Cast<IBootable>());
            
            foreach (var bootable in query) bootable.Bootup();
            
            lookups = new Dictionary<Type, int>();
            bridgedComponents = new Dictionary<Type, Component>();

            for (var i = 0; i < table.Count; i++)
            {
                var key = table[i].GetType();
                if (lookups.ContainsKey(key))
                {
                    table.RemoveAt(i);
                    i--;

                    continue;
                }
                
                lookups.Add(key, i);
                Entities.TryInjectComponentData(this, table[i]);
            }
        }

        void OnEnable() => Entities.Register(this);
        void OnDisable() => Entities.Unregister(this);
        
        #region Relays

        public void Relay<T1>(Entities.P<T1> method) where T1 : IData
        {
            method(this, GetSection<T1>());
        }
        public void Relay<T1,T2>(Entities.PP<T1,T2> method) where T1 : IData where T2 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>());
        }
        public void Relay<T1,T2,T3>(Entities.PPP<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPPP<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        
        public void Relay<T1>(Entities.W<T1> method) where T1 : IData
        {
            var argOne = GetSection<T1>();
            
            method(this, ref argOne);
            
            SetSection(argOne);
        }
        public void Relay<T1,T2>(Entities.WW<T1,T2> method) where T1 : IData where T2 : IData
        {
            var argOne = GetSection<T1>();
            var argTwo= GetSection<T2>();
            
            method(this, ref argOne, ref argTwo);
            
            SetSection(argOne);
            SetSection(argTwo);
        }
        public void Relay<T1,T2,T3>(Entities.WWW<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            var argOne = GetSection<T1>();
            var argTwo= GetSection<T2>();
            var argThree = GetSection<T3>();

            method(this, ref argOne, ref argTwo, ref argThree);
            
            SetSection(argOne);
            SetSection(argTwo);
            SetSection(argThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WWWW<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argOne = GetSection<T1>();
            var argTwo= GetSection<T2>();
            var argThree = GetSection<T3>();
            var argtFour = GetSection<T4>();
            
            method(this, ref argOne, ref argTwo, ref argThree, ref argtFour);
            
            SetSection(argOne);
            SetSection(argTwo);
            SetSection(argThree);
            SetSection(argtFour);
        }

        public void Relay<T1>(Entities.R<T1> method) where T1 : IData
        {
            method(this, GetSection<T1>());
        }
        public void Relay<T1,T2>(Entities.RR<T1,T2> method) where T1 : IData where T2 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>());
        }
        public void Relay<T1,T2,T3>(Entities.RRR<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.RRRR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        
        public void Relay<T1,T2>(Entities.PW<T1,T2> method) where T1 : IData where T2 : IData
        {
            var argTwo= GetSection<T2>();
            
            method(this, GetSection<T1>(), ref argTwo);
            
            SetSection(argTwo);
        }
        public void Relay<T1,T2>(Entities.PR<T1,T2> method) where T1 : IData where T2 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>());
        }
        public void Relay<T1,T2>(Entities.WR<T1,T2> method) where T1 : IData where T2 : IData
        {
            var argOne = GetSection<T1>();

            method(this, ref argOne, GetSection<T2>());
            
            SetSection(argOne);
        }

        public void Relay<T1,T2,T3>(Entities.PPW<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            var argThree = GetSection<T3>();

            method(this, GetSection<T1>(), GetSection<T2>(), ref argThree);
            
            SetSection(argThree);
        }
        public void Relay<T1,T2,T3>(Entities.PWW<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            var argTwo= GetSection<T2>();
            var argThree = GetSection<T3>();

            method(this, GetSection<T1>(), ref argTwo, ref argThree);

            SetSection(argTwo);
            SetSection(argThree);
        }
        public void Relay<T1,T2,T3>(Entities.PPR<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            method(this, GetSection<T1>(),  GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3>(Entities.PRR<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3>(Entities.PWR<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            var argTwo= GetSection<T2>();

            method(this,  GetSection<T1>(), ref argTwo,  GetSection<T3>());
            
            SetSection(argTwo);
        }
        public void Relay<T1,T2,T3>(Entities.WWR<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            var argOne = GetSection<T1>();
            var argTwo= GetSection<T2>();

            method(this, ref argOne, ref argTwo, GetSection<T3>());
            
            SetSection(argOne);
            SetSection(argTwo);
        }
        public void Relay<T1,T2,T3>(Entities.WRR<T1,T2,T3> method) where T1 : IData where T2 : IData where T3 : IData
        {
            var argOne = GetSection<T1>();

            method(this, ref argOne, GetSection<T2>(), GetSection<T3>());
            
            SetSection(argOne);
        }
        
        public void Relay<T1,T2,T3,T4>(Entities.PPPW<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argtFour = GetSection<T4>();
            
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), ref argtFour);
            
            SetSection(argtFour);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPWW<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argThree = GetSection<T3>();
            var argtFour = GetSection<T4>();
            
            method(this, GetSection<T1>(), GetSection<T2>(), ref argThree, ref argtFour);
            
            SetSection(argThree);
            SetSection(argtFour);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PWWW<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argTwo= GetSection<T2>();
            var argThree = GetSection<T3>();
            var argtFour = GetSection<T4>();
            
            method(this, GetSection<T1>(), ref argTwo, ref argThree, ref argtFour);
            
            SetSection(argTwo);
            SetSection(argThree);
            SetSection(argtFour);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPPR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPRR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PRRR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPWR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argThree = GetSection<T3>();

            method(this, GetSection<T1>(), GetSection<T2>(), ref argThree, GetSection<T4>());

            SetSection(argThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PWWR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argTwo= GetSection<T2>();
            var argThree = GetSection<T3>();

            method(this, GetSection<T1>(), ref argTwo, ref argThree,  GetSection<T4>());
            
            SetSection(argTwo);
            SetSection(argThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PWRR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argTwo= GetSection<T2>();

            method(this, GetSection<T1>(), ref argTwo, GetSection<T3>(), GetSection<T4>());
            
            SetSection(argTwo);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WWWR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argOne = GetSection<T1>();
            var argTwo= GetSection<T2>();
            var argThree = GetSection<T3>();

            method(this, ref argOne, ref argTwo, ref argThree, GetSection<T4>());
            
            SetSection(argOne);
            SetSection(argTwo);
            SetSection(argThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WWRR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argOne = GetSection<T1>();
            var argTwo= GetSection<T2>();

            method(this, ref argOne, ref argTwo, GetSection<T3>(), GetSection<T4>());
            
            SetSection(argOne);
            SetSection(argTwo);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WRRR<T1,T2,T3,T4> method) where T1 : IData where T2 : IData where T3 : IData where T4 : IData
        {
            var argOne = GetSection<T1>();

            method(this, ref argOne, GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
            
            SetSection(argOne);
        }
        #endregion

        //---[Flag & Section handling]----------------------------------------------------------------------------------/
        
        public void AddFlag(Enum flag)
        {
            flags.Add(new Flag(flag));
            onFlagAddition?.Invoke(this, flag);
        }
        public void RemoveFlag(Enum flag)
        {
            var value = Entities.TranslateFlag(flag);
            flags.RemoveAll(item => Entities.TranslateFlag(item.Value) == value);
            onFlagRemoval?.Invoke(this, flag);
        }

        public void AddData(IData data)
        {
            var key = data.GetType();
            if (lookups.ContainsKey(key)) return;

            var count = table.Count;
            lookups.Add(key, count);
            table.Add(data);
            
            Entities.TryInjectComponentData(this, data);
            onDataAddition?.Invoke(this, data);

            for (var i = 0; i < count; i++)
            {
                key = table[i].GetType();
                lookups[key] = i;
            }
        }
        public void RemoveData<T>() where T : IData
        {
            var key = typeof(T);
            if (!lookups.ContainsKey(key)) return;
            
            var index = lookups[key];
            onDataRemoval?.Invoke(this, table[index]);
        }
        internal void Cleanup(Type type)
        {
            var index = lookups[type];

            table.RemoveAt(index);
            lookups.Remove(type);
            
            for (var i = index; i < table.Count; i++)
            {
                type = table[i].GetType();
                lookups[type] = i;
            }
        }
        
        //---[Utilities]------------------------------------------------------------------------------------------------/
        
        internal Component AddBridgeTo(Type bridgedType)
        {
            if (bridgedComponents.TryGetValue(bridgedType, out var component)) return component;

            component = GetComponent(bridgedType);
            bridgedComponents.Add(bridgedType, component);

            return component;
        }
        
        public override int GetHashCode() => GetInstanceID();
        public override string ToString()
        {
            var output = $"Entity : {GetHashCode()}/{name}\n";
            foreach (var section in table) output += $"---|{section}\n";

            return output;
        }
    }
}