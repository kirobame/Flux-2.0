using System;
using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example06
{
    [Serializable, Path("Samples/05")] // Specifies the creation path like a MenuItem once in the SequenceEditor
    public class Animation : Effect
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string triggerName;
        [SerializeField] private string outTag;

        private bool triggerActivated;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        public override void Ready()
        {
            base.Ready();
            triggerActivated = false; // Before each sequence execution, reset this state to execute the trigger only once
        }

        
        // Update is the first flow method to be called on an effect
        // As long as IsDone is true, OnUpdate will be called each tick and all linked effects will not receive any flow
        protected override void OnUpdate(EventArgs args)
        {
            if (!triggerActivated) // Execute the trigger
            {
                animator.SetTrigger(triggerName);
                triggerActivated = true;

                return;
            }

            var stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Check if we have reached the given Out state
            if (stateInfo.IsTag(outTag)) IsDone = true; // If so, release control from this effect & let the flow trickle down deeper
        }
    }

    [Serializable, Path("Samples/05")]
    public class ChannelFill : Effect
    {
        [SerializeField] private bool r;
        [SerializeField] private bool g;
        [SerializeField] private bool b;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/
        
        protected override void OnUpdate(EventArgs args) => IsDone = true; // There is no need to block the flow with this effect

        // OnTraversed is called on an effect for every tick after its IsDone switches to true
        protected override EventArgs OnTraversed(EventArgs args) // It only needs to modify the passed EventArgs
        {
            if (args is IWrapper<float> wrapper) // If possible convert a float into a color
            {
                var i = wrapper.Value;
                var color = new Color(r ? i : 0.0f, g ? i : 0.0f, b ? i : 0.0f, 1.0f);

                return new WrapperArgs<Color>(color);
            }
            else return args;
        }
    }

    [Serializable, Path("Samples/05")]
    public class ChangeColor : Effect
    {
        [SerializeField] private float duration;
        [SerializeField] private MeshRenderer renderer;

        private float time;
        private Color startColor;
        
        //---[Core]-----------------------------------------------------------------------------------------------------/

        public override void Ready()
        {
            base.Ready();

            time = duration; // Reset countdown & fetch the starting color for Color.Lerp();
            startColor = renderer.material.GetColor("_Color");
        }

        protected override void OnUpdate(EventArgs args)
        {
            if (args is IWrapper<Color> castedArgs) // If possible lerp the color of the targeted renderer
            {
                time -= Time.deltaTime;

                var endColor = castedArgs.Value;
                var ratio = 1.0f - time / duration;
                renderer.material.SetColor("_Color", Color.Lerp(startColor, endColor, ratio));

                if (time < 0) IsDone = true;
            }
            else IsDone = true;
        }
    }
}