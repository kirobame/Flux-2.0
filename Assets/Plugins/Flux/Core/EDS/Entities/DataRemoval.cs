using System;

namespace Flux.EDS
{
    internal class DataRemoval : Command
    {
        public DataRemoval(Entity target, Type dataType)
        {
            this.target = target;
            this.dataType = dataType;
        }
        
        private Entity target;
        private Type dataType;

        public override void Execute()
        {
            Entities.RemoveData(target, dataType);
            target.Cleanup(dataType);
        }

        public override int GetHashCode() => target.GetInstanceID() * dataType.GetHashCode() / 2;
    }
}