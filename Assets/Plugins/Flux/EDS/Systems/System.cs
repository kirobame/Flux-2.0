namespace Flux
{
    public abstract class System
    {
        public System() { }

        public bool IsActive { get; protected set; } = true;
        
        public virtual void Initialize() { }
        public abstract void Update();
    }
}