using System;
using UnityEngine;

namespace Example05
{
    public class Printer : MonoBehaviour
    {
        [SerializeField] private string message;

        // Callback to be called from a Listener
        public void Execute(EventArgs args) => Debug.Log(message);
    }
}