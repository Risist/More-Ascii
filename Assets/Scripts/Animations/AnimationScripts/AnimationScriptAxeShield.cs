using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AxeShieldAnimationScript", menuName = "Ris/Animation/AxeShield")]
public class AnimationScriptAxeShield : AnimationScript
{

    public override void InitAnimation(AnimationStateMachine controller)
    {
        var idle = controller.AddNewState("idle");     // 0
        var slash = controller.AddNewState("slash");   // 1
        var push = controller.AddNewState("push");     // 2
        var block = controller.AddNewState("block");   // 3
        var stagger = controller.AddNewState("pain1"); // 4
        var stagger2 = controller.AddNewState("pain2");// 5
        var blockWallDash = controller.AddNewState("block wall dash"); // 6

        var health = controller.GetComponent<HealthController>();
        var inputHolder = controller.GetComponent<InputHolder>();
        var movement = controller.GetComponent<RigidbodyMovement>();
        var rigidbody = controller.GetComponent<Rigidbody2D>();

        controller.IterateOverStates(
            (state) =>
            {
                if (state != stagger2 && state != stagger)
                {
                    state.AddTransition(stagger, new AnimationBlendData(0.1f));
                    state.AddTransition(stagger2, new AnimationBlendData(0.1f));
                }
            }
        );
        controller.SetCurrentState(idle);

        idle
            .AddTransition(block, new AnimationBlendData(0))
            .AddTransition(push, new AnimationBlendData(0))
            .AddTransition(slash, new AnimationBlendData(0))
            //.AddTransition(blockWallDash, new AnimationBlendData(0))
            ;

        BoxValue<bool> damaged = new BoxValue<bool>(false);
        health.onStaggerCallback += (data) =>
        {
            if (data.staggerIncrease != 0)
                damaged.value = true;
        };


        stagger
            .AddCanEnter(() => damaged.value && Random.value < 0.5f)
            .AddOnEnd(() => damaged.value = false)
            .AddUpdate((animationTime) => AutoTransition(controller, idle))


            .AddTransition(push, new RangedFloat(0.6f), new AnimationBlendData(0.2f, 0.0f))
            .AddTransition(slash, new RangedFloat(0.6f), new AnimationBlendData(0.2f, 0.0f))
        ;

        stagger2
            .AddCanEnter(() => damaged.value)
            .AddOnEnd(() => damaged.value = false)
            .AddUpdate((animationTime) => AutoTransition(controller, idle))


            .AddTransition(push, new RangedFloat(0.6f), new AnimationBlendData(0.2f, 0.0f))
            .AddTransition(slash, new RangedFloat(0.6f), new AnimationBlendData(0.2f, 0.0f))
        ;

        Timer cdBlock = new Timer(0.0f);
        Timer cdSlash = new Timer(0.0f);
        Timer cdPush = new Timer(0.0f);



        BoxValue<Vector2> initialDirection = new BoxValue<Vector2>(Vector2.zero);
        BoxValue<Vector2> initialMovement = new BoxValue<Vector2>(Vector2.zero);
        

        SetCd(slash, cdSlash);
        SetInput(slash, inputHolder, 0);
        slash
            .AddFixedUpdate((animationTime) =>
            {
                if (RangedFloat.InRange(animationTime, 0, 0.55f))
                {
                    movement.atExternalRotation = true;
                    float desiredRotation = -Vector2.SignedAngle(inputHolder.directionInput, Vector2.up);

                    float t = 0.4f - animationTime;
                    float maxRotation = Mathf.Lerp(6f, 0, t*t);

                    movement.desiredRotation = rigidbody.rotation = Mathf.MoveTowardsAngle(rigidbody.rotation, desiredRotation, maxRotation);
                }

                if (RangedFloat.InRange(animationTime, 0.325f, 0.415f))
                    Motor(rigidbody, rigidbody.transform.up * 425.0f);
            })
            .AddUpdate((animationTime) => AutoTransition(controller, idle))
            
            .AddTransition(block, new RangedFloat(0.6f), new AnimationBlendData(0.125f, 0f))
            .AddTransition(push, new RangedFloat(0.65f), new AnimationBlendData(0.05f, 0.0f))
        ;


        SetCd(push, cdPush);
        SetInput(push, inputHolder, 1);
        push
            .AddOnBegin(() => initialDirection.value = inputHolder.directionInput.normalized)
            .AddOnBegin(() => initialMovement.value = inputHolder.GetAllignedDirection(new float[] { 500, 150/*, 450, 450*/ }, 0))
            .AddFixedUpdate((animationTime) =>
            {
                float rotationSpeed = 0.075f;
                if(RangedFloat.InRange(animationTime, 0, 0.2f) || RangedFloat.InRange(animationTime, 0.5f, 0.7f))
                {
                    movement.ApplyExternalRotation(initialDirection.value, rotationSpeed);
                }else if (RangedFloat.InRange(animationTime, 0.0f, 0.7f))
                {
                    movement.atExternalRotation = true;
                    float desiredRotation = -Vector2.SignedAngle(inputHolder.directionInput, Vector2.up);


                    float t = 0.4f - animationTime;
                    float maxRotation = Mathf.Lerp(3f, 0, t * t);

                    float currentRotation = rigidbody.rotation;
                    movement.desiredRotation = rigidbody.rotation = Mathf.MoveTowardsAngle(currentRotation, desiredRotation, maxRotation);
                    //movement.ApplyExternalRotation(initialDirection.value, rotationSpeed);
                    initialDirection.value = Quaternion.Euler(0, 0, movement.desiredRotation)*Vector2.up;
                }
                
                if (RangedFloat.InRange(animationTime, 0.325f, 0.425f))
                    Motor(rigidbody, initialDirection.value * -400.0f);

                if (RangedFloat.InRange(animationTime, 0.050f, 0.1250f))
                    Motor(rigidbody, initialMovement.value);
            })
            .AddUpdate((animationTime) => AutoTransition(controller, idle))

            .AddTransition(block, new RangedFloat(0.6f), new AnimationBlendData(0.125f, 0f))
            .AddTransition(slash, new RangedFloat(0.1f), new AnimationBlendData(0.15f, 0.1f))
        ;


        SetInput(block, inputHolder, 2);
        block
            .AddOnBegin(() => initialDirection.value = inputHolder.directionInput.normalized)
            .AddOnBegin(() => initialMovement.value = inputHolder.GetAllignedDirection(new float[] { 400, 350, 425, 425 }, 0))
            .AddFixedUpdate( (animationTime) =>
            {
                float rotationSpeed = 0.25f;
                if (RangedFloat.InRange(animationTime, 0, 0.6f))
                    movement.ApplyExternalRotation(initialDirection.value, rotationSpeed);

                if (RangedFloat.InRange(animationTime, 0.11f, 0.175f))
                    Motor(rigidbody, initialMovement.value*1.25f);
                else if (RangedFloat.InRange(animationTime, 0.11f, 0.275f))
                    Motor(rigidbody, initialMovement.value*0.75f);
                //else if (RangedFloat.InRange(animationTime, 0.11f, 0.5f))
                    //Motor(rigidbody, initialMovement.value * 0.075f);
            })
            .AddUpdate((animationTime) => AutoTransition(controller, idle))
            
            .AddTransition(push, new RangedFloat(0.45f),  new AnimationBlendData(0.1f,  0.0f))
            .AddTransition(slash, new RangedFloat(0.25f), new AnimationBlendData(0.2f, 0.05f))
            .AddTransition(block, new RangedFloat(0.8f), new AnimationBlendData(0.1f, -0.1f))
            //.AddTransition(blockWallDash, new RangedFloat(0.3f, 0.4f), new AnimationTransitionData(0f, 0.0f), false, (CharacterState target, CharacterStateController controller) => jumpBlock.currentDirectionId == 0)

            //.AddTransition(blockWallDash, new RangedFloat(0.25f, 0.45f), new AnimationBlendData(0.1f, 0.25f), false, (CharacterState target, CharacterStateController controller) => jumpBlock.currentDirectionId == 0)
        ;

        /*blockWallDash
            .AddComponent(new CState_Input(2))
            .AddComponent(new CStateJumpOverWall(0.4F, 0.03f, 0.25f, 0.75f))
            .AddComponent(new CState_AutoTransition(idle, 1f))//,0.2f))
        ;*/


    }
}
