using UnityEngine;

namespace Flux.Feedbacks
{
    public static class SequenceHandler
    {
        private static Sequencer sequencer;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootup()
        {
            var sequencerObj = new GameObject("Sequencer");
            Object.DontDestroyOnLoad(sequencerObj);

            sequencer = sequencerObj.AddComponent<Sequencer>();
        }

        public static void Add(Sequence sequence) => sequencer.Add(sequence);
        public static void Remove(Sequence sequence) => sequencer.Remove(sequence);
    }
}