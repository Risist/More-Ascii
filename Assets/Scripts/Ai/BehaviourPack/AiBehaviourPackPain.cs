using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using Pathfinding;

/*
 * When enemy spotted character will look at it
 */
[CreateAssetMenu(fileName = "painBehaviourPack", menuName = "Ris/Ai/Pack/painBehaviourPack")]
class AiBehaviourPackPain : AiBehaviourPack
{
    public override void InitBehaviours(AiBehaviourController controller)
    {
        var stateMachine = controller.stateMachine;
        var inputHolder = controller.GetComponentInParent<InputHolder>();
        var transform = controller.transform;

        FocusPriority priorityFocus = controller.focusPriority;
        AttentionMode painAttention = AiBehaviourPackUtility.GetAttentionPain(controller);

        var tExecute = new Timer();
        var lookAroundMethod = new LookAround(new RangedFloat(0.5f, 0.8f), new RangedFloat(25, 50));

        // TODO if seeker is not defined create MoveToDestination instead
        var moveToDestination = new MoveToDestinationNavigation(controller.GetComponent<Seeker>());
        var velocityAccumulator = new VelocityAccumulator(0.89f, 0.075f);

        var destination = controller.InitBlackboardValue<Vector2>("destination");


        ////////////////
        var idle = painAttention.AddNewState(true)
                .AddOnBegin(() => tExecute.RestartRandom(0.15f, 0.3f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddShallReturn(tExecute.IsReady)
                .SetUtility(0.1f);
        ;

        var moveAway = painAttention.AddNewState(true)
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(0.25f, 0.325f))
            .AddOnUpdate(() =>
            {
                Vector2 toTarget = painAttention.focus.ToTarget();
                inputHolder.positionInput = -toTarget;
                inputHolder.rotationInput = toTarget;
                inputHolder.directionInput = toTarget;

                //Debug.Log(painAttention.activeTime);
            })
            .AddShallReturn(() => tExecute.IsReady())
            .SetUtility(() => 10)
            .AddCanEnter(() => painAttention.activeTime < 0.1f)
            ;


        BoxValue<Vector2> initialDirection = new BoxValue<Vector2>(Vector2.zero);

        var hit = painAttention.AddNewState(true)
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(1.0f, 1.5f))
            .AddOnBegin(() => inputHolder.directionInput = transform.up)
            .AddOnUpdate(() =>
            {
                Vector2 toTarget = painAttention.focus.ToTarget();
                inputHolder.positionInput = tExecute.IsReady(0.2f) ? Vector2.zero : -toTarget;
                if (tExecute.IsReady(0.2f))
                {
                    inputHolder.directionInput = PolarVector2.MoveTowards(
                        inputHolder.directionInput.GetPolarVector(), toTarget.GetPolarVector(), 3.5f, 0.25f);
                    inputHolder.keys[0] = true;
                }
                //Debug.Log(painAttention.activeTime);
            })
            .AddShallReturn(() => tExecute.IsReady())
            .SetUtility(() => 15)
            .AddCanEnter(() => painAttention.activeTime < 0.1f)
            ;

        /// TODO:
        /// later (if have not switched to another event) ai will have search behaviours 
        ///     look around ( standard 360 degrees with agressive switches)
        ///     move randomly around 
        ///     search manager (which will somehow generate points to check)
        ///     once in a while look at random point close to the hit position
    }
}
