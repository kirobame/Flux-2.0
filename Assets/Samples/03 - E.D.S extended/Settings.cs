using Flux;
using UnityEngine;

namespace Example03
{
    [CreateAssetMenu(fileName = "NewSettings", menuName = "Samples/03/Settings")]
    public class Settings : ScriptableObject, IWrapper<float>
    {
        // If this data is of no interest to the RaceSystem, the IWrapper interface is justified as it can be easier to deal
        // with direct data. Do mind that distance will become a copy which is then not linked anymore to the ScriptableObject
        public string AlternativeName => alternativeName;
        [SerializeField] private string alternativeName; 
        
        public float Distance => distance;
        [SerializeField] private float distance;
        
        //---[Wrapped values]-------------------------------------------------------------------------------------------/

        float IWrapper<float>.Value => distance;
    }
}