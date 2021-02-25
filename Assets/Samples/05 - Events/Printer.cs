using System;
using UnityEngine;

namespace Example05
{
    public class Printer : MonoBehaviour
    {
        [SerializeField] private string message;

        public void Execute(EventArgs args) => Debug.Log(message);
    }
}