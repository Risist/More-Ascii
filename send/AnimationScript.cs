using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ris.Animation
{
    public abstract class AnimationScript : ScriptableObject
    {
        public abstract void InitAnimation(AnimationStateMachine animationStateMachine);

        public static void AutoTransition(AnimationStateMachine animationStateMachine, AnimationState targetState, float transitionDuration = 0.0f, float transitionOffset = 0.0f, float transitionTime = 1.0f)
        {
            float animationTime = animationStateMachine.animationTime;
            if (!animationStateMachine.inTransition && animationTime > transitionTime)
            {
                var blendData = new AnimationBlendData(transitionDuration, transitionOffset);
                animationStateMachine.SetCurrentState(targetState, blendData);
            }
        }
        public static void AutoTransition(AnimationStateMachine animationStateMachine, AnimationState targetState, AnimationBlendData blendData, float transitionTime = 1.0f)
        {
            float animationTime = animationStateMachine.animationTime;
            if (!animationStateMachine.inTransition && animationTime > transitionTime)
            {
                animationStateMachine.SetCurrentState(targetState, blendData);
            }
        }

        public static void SetCd(AnimationState state, Timer cd)
        {
            state.AddCanEnter(cd.IsReady);
            state.AddOnEnd(cd.Restart);
        }
    }
}
