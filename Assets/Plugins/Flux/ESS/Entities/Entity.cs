using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Flux
{
    public class Entity : MonoBehaviour
    {
        internal event Action<Entity, ISection> onSectionAddition;
        internal event Action<Entity, ISection> onSectionRemoval;
        
        internal event Action<Entity, IFlag> onFlagAddition;
        internal event Action<Entity, IFlag> onFlagRemoval;
        
        //---[Data]-----------------------------------------------------------------------------------------------------/

        public IReadOnlyList<IFlag> Flags => flags;
        [SerializeReference] private List<IFlag> flags;

        public IReadOnlyList<ISection> Sections => sections;
        [SerializeReference] private List<ISection> sections = new List<ISection>();

        private Dictionary<Type, int> lookups;
        
        internal IReadOnlyDictionary<Type, Component> BridgedComponents => bridgedComponents;
        private Dictionary<Type, Component> bridgedComponents;

        //---[Accessors]------------------------------------------------------------------------------------------------/
        
        public ISection this[Type type]
        {
            get => sections[lookups[type]];
            set => sections[lookups[type]] = value;
        }

        public T GetSection<T>() where T : ISection => (T)this[typeof(T)];
        public void SetSection(ISection section) => this[section.GetType()] = section;

        //---[Lifetime callbacks]---------------------------------------------------------------------------------------/
        
        void Awake()
        {
            lookups = new Dictionary<Type, int>();
            bridgedComponents = new Dictionary<Type, Component>();

            for (var i = 0; i < sections.Count; i++)
            {
                var key = sections[i].GetType();
                if (lookups.ContainsKey(key))
                {
                    sections.RemoveAt(i);
                    i--;

                    continue;
                }
                
                lookups.Add(key, i);
                Entities.TryInjectComponentData(this, sections[i]);
            }
        }

        void OnEnable() => Entities.Register(this);
        void OnDisable() => Entities.Unregister(this);

        #region Relays

        public void Relay<T1>(Entities.P<T1> method) where T1 : ISection
        {
            method(this, GetSection<T1>());
        }
        public void Relay<T1,T2>(Entities.PP<T1,T2> method) where T1 : ISection where T2 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>());
        }
        public void Relay<T1,T2,T3>(Entities.PPP<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPPP<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        
        public void Relay<T1>(Entities.W<T1> method) where T1 : ISection
        {
            var sectOne = GetSection<T1>();
            
            method(this, ref sectOne);
            
            SetSection(sectOne);
        }
        public void Relay<T1,T2>(Entities.WW<T1,T2> method) where T1 : ISection where T2 : ISection
        {
            var sectOne = GetSection<T1>();
            var sectTwo= GetSection<T2>();
            
            method(this, ref sectOne, ref sectTwo);
            
            SetSection(sectOne);
            SetSection(sectTwo);
        }
        public void Relay<T1,T2,T3>(Entities.WWW<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            var sectOne = GetSection<T1>();
            var sectTwo= GetSection<T2>();
            var sectThree = GetSection<T3>();

            method(this, ref sectOne, ref sectTwo, ref sectThree);
            
            SetSection(sectOne);
            SetSection(sectTwo);
            SetSection(sectThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WWWW<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectOne = GetSection<T1>();
            var sectTwo= GetSection<T2>();
            var sectThree = GetSection<T3>();
            var sectFour = GetSection<T4>();
            
            method(this, ref sectOne, ref sectTwo, ref sectThree, ref sectFour);
            
            SetSection(sectOne);
            SetSection(sectTwo);
            SetSection(sectThree);
            SetSection(sectFour);
        }

        public void Relay<T1>(Entities.R<T1> method) where T1 : ISection
        {
            method(this, GetSection<T1>());
        }
        public void Relay<T1,T2>(Entities.RR<T1,T2> method) where T1 : ISection where T2 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>());
        }
        public void Relay<T1,T2,T3>(Entities.RRR<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.RRRR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        
        public void Relay<T1,T2>(Entities.PW<T1,T2> method) where T1 : ISection where T2 : ISection
        {
            var sectTwo= GetSection<T2>();
            
            method(this, GetSection<T1>(), ref sectTwo);
            
            SetSection(sectTwo);
        }
        public void Relay<T1,T2>(Entities.PR<T1,T2> method) where T1 : ISection where T2 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>());
        }
        public void Relay<T1,T2>(Entities.WR<T1,T2> method) where T1 : ISection where T2 : ISection
        {
            var sectOne = GetSection<T1>();

            method(this, ref sectOne, GetSection<T2>());
            
            SetSection(sectOne);
        }

        public void Relay<T1,T2,T3>(Entities.PPW<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            var sectThree = GetSection<T3>();

            method(this, GetSection<T1>(), GetSection<T2>(), ref sectThree);
            
            SetSection(sectThree);
        }
        public void Relay<T1,T2,T3>(Entities.PWW<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            var sectTwo= GetSection<T2>();
            var sectThree = GetSection<T3>();

            method(this, GetSection<T1>(), ref sectTwo, ref sectThree);

            SetSection(sectTwo);
            SetSection(sectThree);
        }
        public void Relay<T1,T2,T3>(Entities.PPR<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            method(this, GetSection<T1>(),  GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3>(Entities.PRR<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>());
        }
        public void Relay<T1,T2,T3>(Entities.PWR<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            var sectTwo= GetSection<T2>();

            method(this,  GetSection<T1>(), ref sectTwo,  GetSection<T3>());
            
            SetSection(sectTwo);
        }
        public void Relay<T1,T2,T3>(Entities.WWR<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            var sectOne = GetSection<T1>();
            var sectTwo= GetSection<T2>();

            method(this, ref sectOne, ref sectTwo, GetSection<T3>());
            
            SetSection(sectOne);
            SetSection(sectTwo);
        }
        public void Relay<T1,T2,T3>(Entities.WRR<T1,T2,T3> method) where T1 : ISection where T2 : ISection where T3 : ISection
        {
            var sectOne = GetSection<T1>();

            method(this, ref sectOne, GetSection<T2>(), GetSection<T3>());
            
            SetSection(sectOne);
        }
        
        public void Relay<T1,T2,T3,T4>(Entities.PPPW<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectFour = GetSection<T4>();
            
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), ref sectFour);
            
            SetSection(sectFour);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPWW<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectThree = GetSection<T3>();
            var sectFour = GetSection<T4>();
            
            method(this, GetSection<T1>(), GetSection<T2>(), ref sectThree, ref sectFour);
            
            SetSection(sectThree);
            SetSection(sectFour);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PWWW<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectTwo= GetSection<T2>();
            var sectThree = GetSection<T3>();
            var sectFour = GetSection<T4>();
            
            method(this, GetSection<T1>(), ref sectTwo, ref sectThree, ref sectFour);
            
            SetSection(sectTwo);
            SetSection(sectThree);
            SetSection(sectFour);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPPR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPRR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PRRR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            method(this, GetSection<T1>(), GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
        }
        public void Relay<T1,T2,T3,T4>(Entities.PPWR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectThree = GetSection<T3>();

            method(this, GetSection<T1>(), GetSection<T2>(), ref sectThree, GetSection<T4>());

            SetSection(sectThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PWWR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectTwo= GetSection<T2>();
            var sectThree = GetSection<T3>();

            method(this, GetSection<T1>(), ref sectTwo, ref sectThree,  GetSection<T4>());
            
            SetSection(sectTwo);
            SetSection(sectThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.PWRR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectTwo= GetSection<T2>();

            method(this, GetSection<T1>(), ref sectTwo, GetSection<T3>(), GetSection<T4>());
            
            SetSection(sectTwo);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WWWR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectOne = GetSection<T1>();
            var sectTwo= GetSection<T2>();
            var sectThree = GetSection<T3>();

            method(this, ref sectOne, ref sectTwo, ref sectThree, GetSection<T4>());
            
            SetSection(sectOne);
            SetSection(sectTwo);
            SetSection(sectThree);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WWRR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectOne = GetSection<T1>();
            var sectTwo= GetSection<T2>();

            method(this, ref sectOne, ref sectTwo, GetSection<T3>(), GetSection<T4>());
            
            SetSection(sectOne);
            SetSection(sectTwo);
        }
        public void Relay<T1,T2,T3,T4>(Entities.WRRR<T1,T2,T3,T4> method) where T1 : ISection where T2 : ISection where T3 : ISection where T4 : ISection
        {
            var sectOne = GetSection<T1>();

            method(this, ref sectOne, GetSection<T2>(), GetSection<T3>(), GetSection<T4>());
            
            SetSection(sectOne);
        }
        #endregion

        //---[Flag & Section handling]----------------------------------------------------------------------------------/
        
        public void AddFlag(IFlag flag)
        {
            flags.Add(flag);
            onFlagAddition?.Invoke(this, flag);
        }
        public void RemoveFlag(IFlag flag)
        {
            flags.RemoveAll(item => item.GetCode() == flag.GetCode());
            onFlagRemoval?.Invoke(this, flag);
        }

        public void AddSection(ISection section)
        {
            var key = section.GetType();
            if (lookups.ContainsKey(key)) return;

            var count = sections.Count;
            lookups.Add(key, count);
            sections.Add(section);
            
            Entities.TryInjectComponentData(this, section);
            onSectionAddition?.Invoke(this, section);

            for (var i = 0; i < count; i++)
            {
                key = sections[i].GetType();
                lookups[key] = i;
            }
        }
        public void RemoveSection<T>() where T : ISection
        {
            var key = typeof(T);
            if (!lookups.ContainsKey(key)) return;

            var index = lookups[key];
            onSectionRemoval?.Invoke(this, sections[index]);
            
            sections.RemoveAt(index);
            lookups.Remove(key);
            
            for (var i = index; i < sections.Count; i++)
            {
                key = sections[i].GetType();
                lookups[key] = i;
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
            var output = $"Entity : {GetHashCode()}\n";
            foreach (var section in sections) output += $"---|{section}\n";

            return output;
        }
    }
}