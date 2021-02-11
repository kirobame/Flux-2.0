namespace Flux
{
    public static class Access
    {
        public static void ForEach<T>(WriteTo<T> method) where T : IComponent
        {
            var entities = Hub.Request(typeof(T));
            foreach (var entity in entities) entity.Receive(method);
        }
        public static void ForEach<T1,T2>(WW<T1,T2> method) where T1 : IComponent where T2 : IComponent
        {
            var entities = Hub.Request(typeof(T1), typeof(T2));
            foreach (var entity in entities) entity.Receive(method);
        }
    }
}