using System;
using System.Collections.Generic;

namespace Flux.EDS
{
    public class UpdateRelay : IComparable<UpdateRelay>
    {
        public UpdateRelay(string name)
        {
            Name = name;
            After = "Any";
            Before = "Any";
            
            childs = new List<UpdateRelay>();
        }
        
        //---[Data]-----------------------------------------------------------------------------------------------------/

        public string Name { get; protected set; }
        public string After { get; protected set; }
        public string Before { get; protected set; }

        public IReadOnlyList<UpdateRelay> Childs => childs;
        protected List<UpdateRelay> childs;

        private EDS.System system;
        private bool hasSystem;
        
        //---[Utilities]------------------------------------------------------------------------------------------------/

        public void SetOrder(string after, string before)
        {
            After = after;
            Before = before;
        }
        public void Inject(EDS.System system)
        {
            this.system = system;
            hasSystem = true;
        }
        public void Add(UpdateRelay relay) => childs.Add(relay);

        //---[Core]-----------------------------------------------------------------------------------------------------/

        public void Initialize()
        {
            if (hasSystem) system.Initialize();
            foreach (var child in childs) child.Initialize();
        }
        public void Update()
        {
            if (hasSystem && system.IsActive) system.Update();
            foreach (var child in childs) child.Update();
        }
        
        //---[Sorting]--------------------------------------------------------------------------------------------------/
        
        public void Sort()
        {
            childs.Sort();
            foreach (var child in childs) child.Sort();
        }
        
        int IComparable<UpdateRelay>.CompareTo(UpdateRelay relay)
        {
            if (After != "Any" && After == relay.Name) return 1;
            else if (Before != "Any" && Before == relay.Name) return -1;
            else return 0;
        }
    }
}