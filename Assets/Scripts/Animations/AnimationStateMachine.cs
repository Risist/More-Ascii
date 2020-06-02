using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateMachine : MonoBehaviour
{
    //public AnimationStateMachine(Animator animator) { this.animator = animator; }
    public Animator animator { get; private set; }
    public int stateMachineLayer = 0;
    public AnimationScript animationScript;

    public float animationTime { get => animator.GetCurrentAnimatorStateInfo(stateMachineLayer).normalizedTime; }
    public bool inTransition { get => animator.IsInTransition(stateMachineLayer); }
    public bool hasFinishedAnimation { get => animator.GetCurrentAnimatorStateInfo(stateMachineLayer).normalizedTime >= 1; }

    #region States
    // list of states character can be in
    readonly List<AnimationState> _states = new List<AnimationState>();
    public AnimationState currentState { get; private set;}

    public bool AnimatorHasState(string animationName)
    {
        return AnimatorHasState(Animator.StringToHash(animationName));
    }
    public bool AnimatorHasState(int animationHash)
    {
        return animator.HasState(stateMachineLayer, animationHash);
    }

    public AnimationState AddNewState(string animationName)
    {
        if (!animator.HasState(stateMachineLayer, Animator.StringToHash(animationName)))
        {
            // no such state 
            return null;
        }

        AnimationState state = new AnimationState(animationName);
        _states.Add(state);
        return state;
    }
    public AnimationState AddNewStateAsCurrent(string animationName)
    {
        if (!AnimatorHasState(animationName))
        {
            // no such state 
            return null;
        }

        AnimationState state = new AnimationState(animationName);
        _states.Add(state);
        SetCurrentState(state);
        return state;
    }

    public void SetCurrentState(AnimationState target, AnimationBlendData blendData)
    {
        Debug.Assert(target != null);

        if (currentState != null)
            currentState.End();

        currentState = target;
        currentState.ResetBuffer(); 
        currentState.Begin();

        animator.CrossFade(target.animationHash, blendData.transitionDuration, stateMachineLayer, blendData.normalizedOffset);
    }
    public void SetCurrentState(AnimationState target)
    {
        Debug.Assert(target != null);
        if (currentState != null)
            currentState.End();

        currentState = target;
        currentState.ResetBuffer();
        currentState.Begin();

        animator.Play(target.animationHash);
    }


    public void ClearStates()
    {
        SetCurrentState(null);
        _states.Clear();
    }
    public void IterateOverStates(Action<AnimationState> action)
    {
        foreach (var it in _states)
            action(it);
    }
    public void ResetInputBuffer()
    {
        foreach (var it in _states)
            it.ResetBuffer();
    }
    #endregion States

    #region 
    public void Start()
    {
        animator = GetComponent<Animator>();
        if(animationScript)
        {
            animationScript.InitAnimation(this);
        }
    }
    public void Update()
    {
        float animationTime = this.animationTime;

        Debug.Assert(currentState != null, "AnimationStateMachine: currentState has not been initialized");
        currentState.Update(animationTime);
        
        // transition only when all current transitions are finished
        if (!inTransition)
        {
            var transitionData = currentState.GetTransition(animationTime);
            if (transitionData == null)
                return;

            SetCurrentState(transitionData.target, transitionData.blendData);
        }
    }
    public void FixedUpdate()
    {
        float animationTime = this.animationTime;

        Debug.Assert(currentState != null);
        currentState.FixedUpdate(animationTime);
    }
    #endregion
}

public class AnimationTransition
{
    public AnimationState target;
    /// transition can only occur in specified part of animation
    public RangedFloat transitionRange;
    public bool bufferInput;

    /// additional requirements to use this transition
    public BoolMethod canEnter = () => true;

    public AnimationBlendData blendData;
}
public struct AnimationBlendData
{
    public AnimationBlendData(float transitionDuration, float normalizedOffset = 0f)
    {
        this.transitionDuration = transitionDuration;
        this.normalizedOffset = normalizedOffset;
    }
    public float transitionDuration;
    public float normalizedOffset;
}

public class AnimationState
{
    public AnimationState(string animationName)
    {
        this.animationName = animationName;
    }

    #region Animation
    public override string ToString()
    {
        // for debug purposes
        // animation state is closely related to animation from animator
        // states will be identified by their hash
        return animationHash.ToString();
    }
    public int animationHash;
    public string animationName { set { animationHash = Animator.StringToHash(value); } }
    #endregion Animation

    /// for more intuitive usage transitions will not require holding still action key
    /// if requirements are met for transition but it's too early for animation to transition
    ///     will for transition will be saved
    public bool bufferedInput { get; private set; }
    public void ResetBuffer() { bufferedInput = false; }


    #region Callbacks
    Action _onBegin = () => { };
    Action _onEnd = () => { };
    Action<float> _onUpdate = (animationTime) => { };
    Action<float> _onFixedUpdate = (animationTime) => { };
    readonly List<BoolMethod> _canEnter = new List<BoolMethod>();
    readonly List<BoolMethod> _isPressed = new List<BoolMethod>();

    public AnimationState AddOnBegin(Action s)
    {
        _onBegin += s;
        return this;
    }
    public AnimationState AddOnEnd(Action s)
    {
        _onEnd += s;
        return this;
    }

    public AnimationState AddUpdate(Action<float> s)
    {
        _onUpdate += s;
        return this;
    }
    public AnimationState AddFixedUpdate(Action<float> s)
    {
        _onFixedUpdate += s;
        return this;
    }
    public AnimationState AddCanEnter(BoolMethod s)
    {
        _canEnter.Add(s);
        return this;
    }
    public AnimationState AddIsPressed(BoolMethod s)
    {
        _isPressed.Add(s);
        return this;
    }

    public void Begin()
    {
        _onBegin();
    }
    public void End()
    {
        _onEnd();
    }
    public void Update(float animationTime)
    {
        _onUpdate(animationTime);
    }
    public void FixedUpdate(float animationTime)
    {
        _onFixedUpdate(animationTime);
    }
    #endregion Callbacks


    #region Transitions
    readonly List<AnimationTransition> _transitions = new List<AnimationTransition>();

    public AnimationState AddTransition(AnimationState target, bool bufferInput = true, BoolMethod canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition();
        transition.target = target;
        transition.transitionRange = new RangedFloat();
        transition.bufferInput = bufferInput;
        if(canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }
    public AnimationState AddTransition(AnimationState target, RangedFloat transitionRange, bool bufferInput = true, BoolMethod canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition();
        transition.target = target;
        transition.transitionRange = transitionRange;
        transition.bufferInput = bufferInput;
        if (canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }
    public AnimationState AddTransition(AnimationState target, AnimationBlendData transitionData, bool bufferInput = true, BoolMethod canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition();
        transition.target = target;
        transition.transitionRange = new RangedFloat();
        transition.blendData = transitionData;
        transition.bufferInput = bufferInput;
        if (canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }
    public AnimationState AddTransition(AnimationState target, RangedFloat transitionRange, AnimationBlendData transitionData, bool bufferInput = true, BoolMethod canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition();
        transition.target = target;
        transition.transitionRange = transitionRange;
        transition.blendData = transitionData;
        transition.bufferInput = bufferInput;
        if (canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }


    bool CanEnter()
    {
        foreach(var it in _canEnter)
            if( !it() )
                return false;
        return true;
    }
    bool IsPressed()
    {
        foreach (var it in _isPressed)
            if (!it())
                return false;
        return true;
    }
    public AnimationTransition GetTransition(float animationTime)
    {
        // sequential transition prioritetization
        // transitions at front of the list are more preffered
        // other ones will pop up only if for any reason it was not siutable to use previous 
        foreach (var it in _transitions)
        {
            if (!it.canEnter())
                continue;

            var target = it.target;
            var p = it.transitionRange;
            bool pInRange = p.InRange(animationTime);
            if (!pInRange)
                return null;

            // buffer input if there was will to be in this state
            // and transition uses input buffer
            if (target.IsPressed())
                target.bufferedInput = it.bufferInput;

            // use transition if input was buffered since last state change
            // and it is allowed to be in this state atm
            if (target.bufferedInput && target.CanEnter())
                return it;
        }

        /// null means there was no transition, continue whatever you've been doing
        return null;
    }
    #endregion Transitions

}
