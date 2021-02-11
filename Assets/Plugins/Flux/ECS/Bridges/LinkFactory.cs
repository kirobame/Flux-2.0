using UnityEngine;

namespace Flux
{
    public abstract class LinkFactory
    {
        public abstract bool TryCreateFor(Entity entity, out Link link);
    }

    public class LinkFactory<T> : LinkFactory where T : Component
    {
        public override bool TryCreateFor(Entity entity, out Link link)
        {
            if (entity.gameObject.TryGetComponent<T>(out var component))
            {
                link = new Link<T>(component);
                return true;
            }

            link = null;
            return false;
        }
    }
}