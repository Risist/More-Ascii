using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ris.Animation
{
    public class AnimationScriptCharacter : AnimationScript
    {
        public enum ECharacterState
        {
            Attack,
            Stagger,
            Size
        }

        // animation flags should be set by GA 
        bool[] animationFlags;
        public void SetAnimationFlag(int id)
        {
            animationFlags[id] = true;
        }


        public override void InitAnimation(AnimationStateMachine animationStateMachine)
        {
            var idle = controller.AddNewState(ECharacterState.Idle.ToString());

            animationFlags = new bool[(int)ECharacterState.Size];

            for(int i = 0; i < ECharacterState.Size; ++i)
            {
                var state = controller.AddNewState( ((ECharacterState)i).ToString() );
                // check if character contains such state
                if (!state)
                    continue;

                idle.AddTransition(state, new AnimationBlendData(0, 0));
                state
                    .AddUpdate((animationTime) => AutoTransition(controller, idle))
                    .AddCanEnter(() => animationFlags[i] )
                    .AddOnBegin(() => animationFlags[i] = false)
                ;
            }

        }
    }
}