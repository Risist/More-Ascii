using UnityEditorInternal;
using UnityEngine;

public class SequenceState : State
{
    public SequenceState()
    {
        _onBegin = OnBegin_def;
        _onEnd = onEnd_def;
        _shallReturn.Add(ShallReturn_def);
        _onUpdate = OnUpdate_def;
    }

    readonly StateSet _transitions = new StateSet();
    public int _currentId { get; private set; }
    bool _shallAbort;
    public State currentState
    {
        get => _currentId < _transitions.Count ? _transitions.GetState(_currentId) : null;
    }

    public State AddNewState()
    {
        return AddNewState<State>();
    }
    public T AddExistingState<T>(T state) where T : State, new()
    {
        return _transitions.AddExistingState(state);
    }
    public T AddNewState<T>() where T : State, new()
    {
        var state = new T();
        AddExistingState(state);
        return state;
    }    

    #region callbacks
    void OnBegin_def()
    {
        _shallAbort = false;
        _currentId = 0;
        if (_transitions.Count != 0)
            currentState.Begin();
    }
    bool ShallReturn_def()
    {
        return _shallAbort;
    }
    void OnUpdate_def()
    {
        currentState.Update();

        if (currentState.ShallReturn())
        {
            currentState.End();
            if (++_currentId >= _transitions.Count || !currentState.CanEnter())
                _shallAbort = true;
            else
                currentState.Begin();
        }
    }
    void onEnd_def()
    {
        if (!_shallAbort)
            currentState.End();
    }
    #endregion callbacks

}
