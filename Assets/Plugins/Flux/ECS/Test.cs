using UnityEngine;

public class Test : MonoBehaviour
{
    public void Modify(ref Position position)
    {
        position.value += Vector3.one;
    }
}