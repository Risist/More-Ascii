using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;

public class AiBehaviourController : MonoBehaviour
{
    #region References
    [NonSerialized] public readonly StateMachine stateMachine = new StateMachine();
    public FocusPriority focusPriority { get; private set; }
    #endregion References


    #region Blackboard
    readonly GenericBlackboard _blackboard = new GenericBlackboard();

    public BoxValue<T> InitBlackboardValue<T>(string key, GenericBlackboard.InitializeMethod<T> initializeMethod)
    {
        return _blackboard.InitValue<T>(key, initializeMethod);
    }
    public BoxValue<T> InitBlackboardValue<T>(string key) where T: new()
    {
        return _blackboard.InitValue<T>(key);
    }
    public BoxValue<T> GetBlackboardValue<T>(string key)
    {
        return _blackboard.GetValue<T>(key);
    }
    #endregion Blackboard

    #region BehaviourPack
    [Tooltip("Behaviour packs are used to insert groups of behaviours to this agent")]
    [SerializeField] AiBehaviourPack[] _behaviourPacks = new AiBehaviourPack[0];
    void InitBehaviourPacks()
    {
        foreach (var it in _behaviourPacks)
            it.InitBehaviours(this);
    }
    #endregion BehaviourPack

    private void Start()
    {
        focusPriority = AiBehaviourPackUtility.GetFocusPriority(this);
        stateMachine.SetNextStateMethod(focusPriority.GetTransitionByRandomUtility);
        InitBehaviourPacks();
    }

    private void FixedUpdate()
    {
        focusPriority.UpdateCurrentMode();
        stateMachine.UpdateStates();
    }
}
