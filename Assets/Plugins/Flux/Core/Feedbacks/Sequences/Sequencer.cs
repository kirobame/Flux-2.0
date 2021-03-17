using System.Collections.Generic;
using UnityEngine;

namespace Flux.Feedbacks
{
    public class Sequencer : MonoBehaviour
    {
        private List<Sequence> sequences = new List<Sequence>();

        void Update()
        {
            var index = 0;
            while (index < sequences.Count)
            {
                if (!UpdateAt(index)) index++;
            }
        }
        
        public void Add(Sequence sequence)
        {
            if (sequences.Contains(sequence)) return;
            
            sequences.Add(sequence);
            UpdateAt(sequences.Count - 1);
        }
        public void Remove(Sequence sequence) => sequences.Remove(sequence);

        private bool UpdateAt(int index)
        {
            if (sequences[index].Update()) return false;
            
            sequences[index].Stop(false);
            sequences.RemoveAt(index);
            
            return true;
        }
    }
}