using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkUlrich.GenericStateMachine
{
    /// <summary>
    /// Represents a class that manages the state machine for the game.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// The scene instance of this StateMachine.
        /// </summary>
        public StateMachineBehaviour Instance { get; set; }

        /// <summary>
        /// A collection of all states subscribed to this state machine.
        /// </summary>
        public HashSet<State> States { get; } = new HashSet<State>();

        /// <summary>
        /// The current, active state of the state machine.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Allows States to subscribe themselves to the StateMachine.
        /// </summary>
        /// <param name="state">The State subscribing itself to the StateMachine.</param>
        public void Subscribe(State state)
        {
            if (!States.Any(s => s.ToString() == state.ToString()))
                States.Add(state);
        }

        /// <summary>
        /// Sets the current state to the state parsed in the parameter.
        /// </summary>
        /// <param name="newState">The object reference state to change to.</param>
        internal State SetState(State newState)
        {
            CurrentState?.ExitState();

            CurrentState = newState;
            CurrentState.EnterState();

            return CurrentState;
        }

        /// <summary>
        /// Sets the current state to the state parsed in the Type parameter.
        /// </summary>
        /// <typeparam name="TState">The type reference of the state to change to.</typeparam>
        internal State SetState<TState>() where TState : State, new()
            => SetState(GetState<TState>());

        /// <summary>
        /// Moves the current state to the next state set in the state if one is set.
        /// </summary>
        internal void MoveToNextState() => CurrentState.MoveToNextState();

        /// <summary>
        /// Returns whether the current state equals the type parameter.
        /// </summary>
        /// <typeparam name="TState">The state to check against.</typeparam>
        /// <returns>Whether the current state equals the type param.</returns>
        public bool IsCurrentState<TState>() where TState : State, new()
            => CurrentState.GetType() == new TState().GetType();

        /// <summary>
        /// Returns whether the current state equals the type param.
        /// </summary>
        /// <typeparam name="TState">The state to check against.</typeparam>
        /// <returns>Whether the current state equals the type param.</returns>
        public bool IsCurrentState(State state)
            => CurrentState.GetType() == state.GetType();

        /// <summary>
        /// Returns the reference of this state in the StateMachine. Will create new instance if not yet available.
        /// </summary>
        /// <typeparam name="TState">The state class you would like to access</typeparam>
        /// <returns>The reference of this state in the StateMachine.</returns>
        public State GetState<TState>() where TState : State, new()
        {
            State parameterState = new TState();
            return States.FirstOrDefault(state => state.ToString() == parameterState.ToString()) ?? parameterState;
        }

        /// <summary>
        /// Indicates whether the state machine is in debug mode.
        /// </summary>
        /// <remarks>
        /// Default = true.
        /// </remarks>
        public bool DebugMode { get; set; } = true;

        /// <summary>
        /// Prints a debug message to the console if DebugMode in the StateMachine is enabled.
        /// </summary>
        public void DebugLog(string message)
        {
            if (!DebugMode)
                return;

            Debug.Log(message);
        }
    }
}
