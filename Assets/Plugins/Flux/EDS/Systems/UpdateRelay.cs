using System;
using System.Collections.Generic;

namespace Flux
{
    internal abstract class UpdateRelay : IComparable<UpdateRelay>
    {
        public string Name { get; protected set; }
        public string After { get; protected set; }
        public string Before { get; protected set; }

        public IReadOnlyList<UpdateRelay> Childs => childs;
        protected List<UpdateRelay> childs;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        public abstract void Initialize();
        public abstract void Update();
        
        //---[Utilities]------------------------------------------------------------------------------------------------/
        
        public int CompareTo(UpdateRelay wrapper)
        {
            if (After != "Any" && After == wrapper.Name) return 1;
            else if (Before != "Any" && Before == wrapper.Name) return -1;
            else return 0;
        }

        public void Add(UpdateRelay wrapper) => childs.Add(wrapper);
        public void Sort()
        {
            childs.Sort();
            foreach (var child in childs) child.Sort();
        }
    }
    
    //---[Implementations]----------------------------------------------------------------------------------------------/
    
    internal class SystemRouter : UpdateRelay
    {
        public SystemRouter()
        {
            Name = "Root";
            After = "Any";
            Before = "Any";
            
            childs = new List<UpdateRelay>();
        }

        public override void Initialize() { foreach (var child in childs) child.Initialize(); }
        public override void Update() { foreach (var child in childs) child.Update(); }
    }
    internal class SystemWrapper : UpdateRelay
    {
        public SystemWrapper(Type systemType, string after, string before)
        {
            Name = systemType.Name;
            After = after;
            Before = before;
            
            value = (System)Activator.CreateInstance(systemType);
            childs = new List<UpdateRelay>();
        }

        private System value;

        public override void Initialize()
        {
            value.Initialize();
            foreach (var child in childs) child.Initialize();
        } 
        public override void Update()
        {
            if (value.IsActive) value.Update();
            foreach (var child in childs) child.Update();
        } 
    }
}