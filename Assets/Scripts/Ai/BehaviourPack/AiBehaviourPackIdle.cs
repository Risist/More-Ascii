using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using Pathfinding;

[CreateAssetMenu(fileName = "idleBehaviourPack", menuName = "Ris/Ai/Pack/idleBehaviourPack")]
class AiBehaviourPackIdle : AiBehaviourPack
{
    public override void InitBehaviours(AiBehaviourController controller)
    {
        var stateMachine = controller.stateMachine;
        var inputHolder = controller.GetComponentInParent<InputHolder>();
        var transform = controller.transform;
        
        FocusPriority priorityFocus = controller.focusPriority;
        AttentionMode idleAttention = AiBehaviourPackUtility.GetAttentionIdle(controller);

        var tExecute = new Timer();
        var lookAroundMethod = new LookAround();
        
        // TODO if seeker is not defined create MoveToDestination instead
        var moveToDestination = new MoveToDestinationNavigation(controller.GetComponent<Seeker>());

        var destination = controller.InitBlackboardValue<Vector2>("destination");

        ////////////////
        /// idle
        var idle = idleAttention.AddNewState(true)
                .AddOnBegin(() => tExecute.RestartRandom(0.15f, 0.3f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddShallReturn(tExecute.IsReady)
                .SetUtility(20);
            ;
        stateMachine.ChangeState(idle);

        /// lookAround
        var lookAround = idleAttention.AddNewState(true)
            .AddOnBegin(() => lookAroundMethod.Begin(transform) )
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(0.25f, 0.4f))
            .AddOnUpdate(() =>
                {
                    inputHolder.rotationInput = lookAroundMethod.GetRotationInput(transform);
                })
            .AddShallReturn(() => tExecute.IsReady())
            .SetUtility(6)
        ;

        /// randomMovement
        var randomMovement = idleAttention.AddNewState(true)
            .AddOnBegin(() => moveToDestination.SetDestination_Search(priorityFocus, 20))
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(0.75f, 1.5f))
            .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestination();
                    //inputHolder.positionInput = Vector2.Lerp(inputHolder.positionInput, moveToDestination.ToDestination(), 0.985f );
                })
            .AddShallReturn(() => /*focusPain.HasTarget() ||*/ tExecute.IsReady())
            .SetUtility(2f)
            ;


    }
}
