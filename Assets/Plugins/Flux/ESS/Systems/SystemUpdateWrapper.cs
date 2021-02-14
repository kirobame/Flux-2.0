using System;
using System.Collections.Generic;

namespace Flux
{
    internal abstract class SystemUpdateWrapper : IComparable<SystemUpdateWrapper>
    {
        public string Name { get; protected set; }
        public string After { get; protected set; }
        public string Before { get; protected set; }

        public IReadOnlyList<SystemUpdateWrapper> Childs => childs;
        protected List<SystemUpdateWrapper> childs;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        public abstract void Initialize();
        public abstract void Update();
        
        //---[Utilities]------------------------------------------------------------------------------------------------/
        
        public int CompareTo(SystemUpdateWrapper updateWrapper)
        {
            if (After != "Any" && After == updateWrapper.Name) return 1;
            else if (Before != "Any" && Before == updateWrapper.Name) return -1;
            else return 0;
        }

        public void Add(SystemUpdateWrapper updateWrapper) => childs.Add(updateWrapper);
        public void Sort()
        {
            childs.Sort();
            foreach (var child in childs) child.Sort();
        }
    }
    
    //---[Implementations]----------------------------------------------------------------------------------------------/
    
    internal class DummySystemUpdateWrapper : SystemUpdateWrapper
    {
        public DummySystemUpdateWrapper()
        {
            Name = "Root";
            After = "Any";
            Before = "Any";
            
            childs = new List<SystemUpdateWrapper>();
        }

        public override void Initialize() { foreach (var child in childs) child.Initialize(); }
        public override void Update() { foreach (var child in childs) child.Update(); }
    }
    internal class ConcreteSystemUpdateWrapper : SystemUpdateWrapper
    {
        public ConcreteSystemUpdateWrapper(Type systemType, string after, string before)
        {
            Name = systemType.Name;
            After = after;
            Before = before;
            
            value = (System)Activator.CreateInstance(systemType);
            childs = new List<SystemUpdateWrapper>();
        }

        private System value;

        public override void Initialize()
        {
            value.Initialize();
            foreach (var child in childs) child.Initialize();
        } 
        public override void Update()
        {
            value.Update();
            foreach (var child in childs) child.Update();
        } 
    }
}