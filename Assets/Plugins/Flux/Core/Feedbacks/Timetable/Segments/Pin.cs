using System;

namespace Flux.Feedbacks
{
    public abstract class Pin
    {
        public abstract void Update(object value);
    }

    public class Pin<T> : Pin
    {
        public event Action<T> callback;

        public override void Update(object value) => callback?.Invoke((T)value);
        public void Update(T value) => callback?.Invoke(value);
    }
}