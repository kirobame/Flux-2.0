using System;
using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Example06
{
    [Serializable, Path("Samples/05")]
    public class Animation : Effect
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string triggerName;
        [SerializeField] private string outTag;

        private bool triggerActivated;

        public override void Ready()
        {
            base.Ready();
            triggerActivated = false;
        }

        protected override void OnUpdate(EventArgs args)
        {
            if (!triggerActivated)
            {
                animator.SetTrigger(triggerName);
                triggerActivated = true;

                return;
            }

            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag(outTag)) IsDone = true;
        }
    }

    [Serializable, Path("Samples/05")]
    public class ChannelFill : Effect
    {
        [SerializeField] private bool r;
        [SerializeField] private bool g;
        [SerializeField] private bool b;
        
        protected override void OnUpdate(EventArgs args) => IsDone = true;

        protected override EventArgs OnTraversed(EventArgs args)
        {
            if (args is IWrapper<float> wrapper)
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

        public override void Ready()
        {
            base.Ready();

            time = duration;
            startColor = renderer.material.GetColor("_Color");
        }

        protected override void OnUpdate(EventArgs args)
        {
            if (args is WrapperArgs<Color> castedArgs)
            {
                time -= Time.deltaTime;

                var endColor = castedArgs.ArgOne;
                var ratio = 1.0f - time / duration;
                renderer.material.SetColor("_Color", Color.Lerp(startColor, endColor, ratio));

                if (time < 0) IsDone = true;
            }
            else IsDone = true;

        }
    }
}