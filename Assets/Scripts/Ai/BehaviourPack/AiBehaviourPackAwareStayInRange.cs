using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using Pathfinding;

/*
 * When enemy spotted character will look at it
 */
[CreateAssetMenu(fileName = "awareViewCorrectionBehaviourPack", menuName = "Ris/Ai/Pack/awareViewCorrectionBehaviourPack")]
class AiBehaviourPackAwareStayInRange : AiBehaviourPack
{
    public override void InitBehaviours(AiBehaviourController controller)
    {
        var stateMachine = controller.stateMachine;
        var inputHolder = controller.GetComponentInParent<InputHolder>();
        var transform = controller.transform;

        FocusPriority priorityFocus = controller.focusPriority;
        AttentionMode enemyAttention = AiBehaviourPackUtility.GetAttentionEnemy(controller);

        var tExecute = new Timer();
        var lookAroundMethod = new LookAround(new RangedFloat(0.5f, 0.8f), new RangedFloat(25, 50));

        // TODO if seeker is not defined create MoveToDestination instead
        var moveToDestination = new MoveToDestinationNavigation(controller.GetComponent<Seeker>());
        var velocityAccumulator = new VelocityAccumulator(0.89f, 0.125f);

        var destination = controller.InitBlackboardValue<Vector2>("destination");


        ////////////////
        /// idle
        var lookAtTarget = enemyAttention.AddNewState(true)
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(1f, 2f))
            .AddOnUpdate(() => {
                velocityAccumulator.Update(transform, priorityFocus.currentFocus.GetTarget(), 0.01225f);
                inputHolder.rotationInput = velocityAccumulator.position - (Vector2)transform.position;


                inputHolder.directionInput = velocityAccumulator.position - (Vector2)transform.position;
            })
            .AddShallReturn(() => tExecute.IsReady())
            .SetUtility(() => 10)
            ;

        var lookAround = enemyAttention.AddNewState(true)
            .AddOnBegin(() => lookAroundMethod.Begin(transform))
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(0.6f, 1.5f))
            .AddOnUpdate(() =>
            {
                velocityAccumulator.Update(transform, priorityFocus.currentFocus.GetTarget(), 0.01225f);
                Vector2 targetPosition = velocityAccumulator.position - (Vector2)transform.position;
                float angle = Vector2.SignedAngle(Vector2.up, targetPosition);
                inputHolder.rotationInput = lookAroundMethod.GetRotationInput(transform, angle, 0.1f);


                inputHolder.directionInput = velocityAccumulator.position - (Vector2)transform.position;
            })
            .AddShallReturn(() => tExecute.IsReady() || priorityFocus.currentFocus.GetTarget().velocity.sqrMagnitude > 20)
            .SetUtility(() => 15f - 0.225f * priorityFocus.currentFocus.GetTarget().velocity.sqrMagnitude)
        ;

        /// ideas:
        ///     looks a little bit to sides sometimes
        ///     tries to keep certain distance from enemy
        ///     Input Accumulation (aka velocity ?)

        /*/// lookAround
        var lookAround = enemyAttention.AddNewState(true)
            .AddOnBegin(() => lookAroundMethod.Begin(transform))
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
        var randomMovement = enemyAttention.AddNewState(true)
            .AddOnBegin(() => moveToDestination.SetDestination_Search(priorityFocus, 20))
            .AddOnBegin(inputHolder.ResetInput)
            .AddOnBegin(() => tExecute.RestartRandom(0.75f, 1.5f))
            .AddOnUpdate(() =>
            {
                inputHolder.positionInput = moveToDestination.ToDestination();
                //inputHolder.positionInput = Vector2.Lerp(inputHolder.positionInput, moveToDestination.ToDestination(), 0.985f );
            })
            .AddShallReturn(() =>  tExecute.IsReady())
            .SetUtility(2f)
            ;*/


    }
}
