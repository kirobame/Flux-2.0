using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.UI;

namespace Example04
{
    public class Handler : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private string message;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var data = Repository.Get<SomeData>(Name.Link);
                data.message = message;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var meshRenderer in Repository.GetAll<MeshRenderer>(Name.Spheres))
                {
                    meshRenderer.material.SetColor("_Color", color);
                }
            }
        }

        public void OnUserInput(string input)
        {
            var title = Repository.Get<Text>(Name.Title);
            title.text = input;
        }
    }
}