using Flux;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSomeData", menuName = "Flux/Some Data")]
public class SomeData : ScriptableObject, IWrapper<Vector3>, IWrapper<Color>
{
    public Vector3 Step => step;
    [SerializeField] private Vector3 step;

    public Color ColorStep => colorStep;
    [SerializeField] private Color colorStep;

    Vector3 IWrapper<Vector3>.Value => step;
    Color IWrapper<Color>.Value => colorStep;
}