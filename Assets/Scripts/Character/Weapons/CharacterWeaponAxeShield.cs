using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
[CreateAssetMenu(fileName = "AxeShieldWeapon", menuName = "Character/Weapon/AxeShield")]
public class CharacterWeaponAxeShield : CharacterWeaponBase
{
    public override void InitCharacterStateController(CharacterStateController stateController)
    {
        var idle = stateController.AddNewState("idle");     // 0
        var slash = stateController.AddNewState("slash");   // 1
        var push = stateController.AddNewState("push");     // 2
        var block = stateController.AddNewState("block");   // 3
        var stagger = stateController.AddNewState("pain1"); // 4
        var stagger2 = stateController.AddNewState("pain2");// 5
        var blockWallDash = stateController.AddNewState("block wall dash"); // 6

        stateController.AddTransitionToAllStates(stagger,  (CharacterState st) => st == stagger2, new AnimationTransitionData(0.1f));
        stateController.AddTransitionToAllStates(stagger2, (CharacterState st) => st == stagger,  new AnimationTransitionData(0.1f));
        stateController.SetCurrentState(idle);

        idle
            .AddTransition(block, new AnimationTransitionData(0))
            .AddTransition(push, new AnimationTransitionData(0))
            .AddTransition(slash, new AnimationTransitionData(0))
            //.AddTransition(blockWallDash, new AnimationTransitionData(0))
            ;
        DamagedRecorder damaged = new DamagedRecorder(stateController.health);

        stagger
            .AddComponent(new CState_RandomEnter(0.5f))
            .AddComponent(new CState_StaggerCondition(damaged))
            .AddComponent(new CState_AutoTransition(idle, 1f))
        ;

        stagger2
            .AddComponent(new CState_StaggerCondition(damaged))
            .AddComponent(new CState_AutoTransition(idle, 1f))
        ;

        int cdBlock = stateController.CreateCd(0.75f);
        int cdSlash = stateController.CreateCd(0.9f);
        int cdPush = stateController.CreateCd(0.9f);

        
        slash
            .AddComponent(new CState_Input(0))
            .AddComponent(new CState_Cd(cdSlash))
            //.AddComponent(new CState_Cd(cdBlock, CState_Cd.EMode.ERestartOnly))
            .AddComponent(new CState_RotationToDirection( new RangedFloat(0, 0.6f), 0.1f, 0.45f))
            //.AddComponent(new CState_JumpMotor(new float[] { 300, -100 }, new RangedFloat(0.325f, 0.425f), -100.0f).SetDefaultDirection(0))
            .AddComponent(new CState_Motor(new Vector2(0, 325.0f), new RangedFloat(0.325f, 0.4f)))
            .AddComponent(new CState_AutoTransition(idle, 1f))

            .AddTransition(block, new RangedFloat(0.6f), new AnimationTransitionData(0.125f, 0f))
            .AddTransition(push, new RangedFloat(0.65f), new AnimationTransitionData(0.15f, 0.0f))
        ;
        push
            .AddComponent(new CState_Input(1))
            .AddComponent(new CState_Cd(cdPush))
            //.AddComponent(new CState_Cd(cdBlock, CState_Cd.EMode.ERestartOnly))
            .AddComponent(new CState_RotationToDirection(new RangedFloat(0, 0.7f), 0.15f, 1.0f))
            .AddComponent(new CState_JumpMotor(new float[] { 300, 125, 200, 200 }, new RangedFloat(0.050f, 0.1250f), -100.0f).SetDefaultDirection(0))
            //.AddComponent(new CState_Motor(new Vector2(0, 250.0f), new RangedFloat(0.050f, 0.1250f)))
            .AddComponent(new CState_Motor(new Vector2(0, -325.0f), new RangedFloat(0.325f, 0.425f)))
            .AddComponent(new CState_AutoTransition(idle, 1f))

            .AddTransition(block, new RangedFloat(0.6f), new AnimationTransitionData(0.125f, 0f))
            .AddTransition(slash, new RangedFloat(0.6f), new AnimationTransitionData(0.15f, 0.1f))
        ;

        var jumpBlock = new CState_JumpMotor(new float[] { 300, 300, 375, 375 }, new RangedFloat(0.11f, 0.275f))
                .SetDefaultDirection(0);
        block
            .AddComponent(new CState_Input(2))
            .AddComponent(new CState_Cd(cdBlock, CState_Cd.EMode.EConditionOnly))
            .AddComponent(new CState_RotationToDirection(new RangedFloat(0f, 0.6f), 0.25f, 1.0f))
            .AddComponent(jumpBlock)
            .AddComponent(new CState_AutoTransition(idle, 1f))

            .AddTransition(push, new RangedFloat(0.35f), new AnimationTransitionData(0.2f, 0.0f))
            .AddTransition(slash, new RangedFloat(0.35f), new AnimationTransitionData(0.2f, 0.1f))
            //.AddTransition(blockWallDash, new RangedFloat(0.3f, 0.4f), new AnimationTransitionData(0f, 0.0f), false, (CharacterState target, CharacterStateController controller) => jumpBlock.currentDirectionId == 0)
            
            .AddTransition(blockWallDash, new RangedFloat(0.25f,0.45f), new AnimationTransitionData(0.1f, 0.25f), false, (CharacterState target, CharacterStateController controller) => jumpBlock.currentDirectionId == 0)
        ;

        blockWallDash
            .AddComponent(new CState_Input(2))
            .AddComponent(new CStateJumpOverWall(0.4F, 0.03f, 0.25f, 0.75f))
            .AddComponent(new CState_AutoTransition(idle, 1f))//,0.2f))
        ;
    }
}*/

