using System;
using UnityEngine;

public class GameStateMachineBehaviour : SingletonInstance<GameStateMachineBehaviour>
{
    private void Awake()
    {
        this.InitStateMachine();

        // Example of how to set the initial state.
        SetState<ExampleBootState>();
    }

    private void Start() => StateMachineTestStart();
    
    private void Update() => StateMachineTestUpdate();

    /// <summary>
    /// Sets the current state to the state parsed in the param.
    /// </summary>
    /// <param name="newState">The object reference of the state to change to.</param>
    public void SetState(State newState) => GameStateMachine.SetState(newState);

    /// <summary>
    /// Sets the current state to the state parsed in the Type param.
    /// </summary>
    /// <typeparam name="TState">The type reference of the state to change to.</typeparam>
    public void SetState<TState>() where TState : State, new()
        => GameStateMachine.SetState<TState>();


    #region Testing
    private void StateMachineTestStart() => GameStateMachine.GetState<ExampleGameState>().OnStateEnter += DebugStateInstance;

    private void StateMachineTestUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DebugNextState();

        if (Input.GetKeyDown(KeyCode.I))
            DebugPrintStates();
    }

    private void DebugNextState() => GameStateMachine.MoveToNextState();

    private void DebugPrintStates()
    {
        print("Currently Subscribed States: ");
        for (int i = 0; i < GameStateMachine.States.Count; i++)
            print($"{i} : {GameStateMachine.States[i]} : {GameStateMachine.States[i].GetHashCode()}");
    }

    private void DebugStateInstance() => print("I work!");
    #endregion
}
