using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class State
{
    private State _nextState;

    protected GameStateMachineBehaviour OwningGameStateMachine { get; private set; }

    public Action OnStateEnter;
    public Action OnStateExit;

    protected State() => InitState();

    protected void InitState()
    {
        this.Subscribe();
        OwningGameStateMachine = GameStateMachine.OwningGameStateMachine;
    }
    
    protected void SetNextState<TState>() where TState: State, new() 
        => _nextState = GameStateMachine.GetState<TState>();

    protected void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName))
            return;
        
        SceneManager.LoadScene(sceneName);
        Debug.Log($"Loaded Scene ({sceneName})");
    }

    /// <summary>
    /// Executes code related to entering the state.
    /// </summary>
    public virtual void EnterState()
    {
        OnStateEnter?.Invoke();
        
        string stateName = ToString().Replace("TrollverseVR.GameStateMachine.", "");
        Debug.Log($"Entering - {stateName} : {GetHashCode()}");
    }

    /// <summary>
    /// Executes code related to leaving the state.
    /// </summary>
    public virtual void ExitState()
    {
        OnStateExit?.Invoke();
        
        string stateName = ToString().Replace("TrollverseVR.GameStateMachine.", "");
        Debug.Log($"Leaving - {stateName} : {GetHashCode()}");
    }

    /// <summary>
    /// Triggers the Owning StateMachine to move to the next state.
    /// </summary>
    public void MoveToNextState()
    {
        if (_nextState == null)
        {
            Debug.LogError("Next State variable was not set! Cancelling...");
            return;
        }
    
        OwningGameStateMachine.SetState(_nextState);
    }
}
