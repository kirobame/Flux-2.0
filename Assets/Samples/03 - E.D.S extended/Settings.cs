using Flux;
using UnityEngine;

namespace Example03
{
    [CreateAssetMenu(fileName = "NewSettings", menuName = "Samples/03/Settings")]
    public class Settings : ScriptableObject, IWrapper<float>
    {
        public string AlternativeName => alternativeName;
        [SerializeField] private string alternativeName;
        
        public float Distance => distance;
        [SerializeField] private float distance;

        float IWrapper<float>.Value => distance;
    }
}