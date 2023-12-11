using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameStateMachine
{
    public static GameStateMachineBehaviour OwningGameStateMachine { get; private set; }
    public static List<State> States { get; } = new List<State>();
    public static State CurrentState { get; private set; }

    /// <summary>
    /// Allows States to subscribe themselves to the StateMachine.
    /// </summary>
    /// <param name="state">The State subscribing itself to the StateMachine.</param>
    public static void Subscribe(this State state)
    {
        for (int i = 0; i < States.Count; i++)
            if (States[i].ToString() == state.ToString())
                return;

        States.Add(state);
    }

    /// <summary>
    /// Initialises the StateMachine behaviour script.
    /// </summary>
    /// <param name="gameStateMachineBehaviour">The behaviour object reference to initialise.</param>
    internal static void InitStateMachine(this GameStateMachineBehaviour gameStateMachineBehaviour) 
        => OwningGameStateMachine = gameStateMachineBehaviour;

    /// <summary>
    /// Sets the current state to the state parsed in the Parameter.
    /// </summary>
    /// <param name="newState">The object reference state to change to.</param>
    internal static void SetState(State newState)
    {
        CurrentState?.ExitState();
        
        CurrentState = newState;
        CurrentState.EnterState();
    }

    /// <summary>
    /// Moves the current state to the next state set in the state.
    /// </summary>
    internal static void MoveToNextState() => CurrentState.MoveToNextState();
    
    /// <summary>
    /// Returns whether the current state equals the type param.
    /// </summary>
    /// <typeparam name="TState">The state to check against.</typeparam>
    /// <returns>Whether the current state equals the type param.</returns>
    public static bool CurrentStateIs<TState>() where TState: State, new() 
        => CurrentState.GetType() == new TState().GetType();

    /// <summary>
    /// Returns whether the current state equals the type param.
    /// </summary>
    /// <typeparam name="TState">The state to check against.</typeparam>
    /// <returns>Whether the current state equals the type param.</returns>
    public static bool CurrentStateIs(State state)
        => CurrentState.GetType() == state.GetType();

    /// <summary>
    /// Returns the reference of this state in the StateMachine. Will create new instance if not yet available.
    /// </summary>
    /// <typeparam name="TState">The state class you would like to access</typeparam>
    /// <returns>The reference of this state in the StateMachine.</returns>
    public static State GetState<TState>() where TState : State, new()
    {
        State parameterState = new TState();
        return States.FirstOrDefault(state => state.ToString() == parameterState.ToString()) ?? parameterState;
    }
    
    /// <summary>
    /// Sets the current state to the state parsed in the Type param.
    /// </summary>
    /// <typeparam name="TState">The type reference of the state to change to.</typeparam>
    internal static void SetState<TState>() where TState: State, new() 
        => SetState(GetState<TState>());

    /// <summary>
    /// Triggers the OnWinGame Action from the WaveState.
    /// </summary>
    internal static void WinGame()
    {
        WaveState.OnWinGame?.Invoke();
    }

    /// <summary>
    /// Triggers the OnLoseGame Action from the WaveState.
    /// </summary>
    internal static void LoseGame()
    {
        WaveState.OnLoseGame?.Invoke();
    }
}
