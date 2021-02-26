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
        
        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var data = Repository.Get<SomeData>(Name.Link); // None Unity.Object data can be fetched
                data.message = message;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Group of data can be fetched as long as the underlying collection is an implementation of IList<T>
                foreach (var meshRenderer in Repository.GetAll<MeshRenderer>(Name.Spheres)) 
                {
                    meshRenderer.material.SetColor("_Color", color);
                }
            }
        }
        
        //---[Callbacks]------------------------------------------------------------------------------------------------/

        public void OnUserInput(string input)
        {
            var title = Repository.Get<Text>(Name.Title);
            title.text = input;
        }
    }
}