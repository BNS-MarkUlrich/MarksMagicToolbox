using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
    /// <summary>
    /// Represents a class that manages the state machine for the game.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// The list of states in the game state machine.
        /// </summary>
        public List<State> States { get; } = new List<State>();
        
        /// <summary>
        /// Gets the current state of the game.
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
        /// Sets the current state to the state parsed in the Parameter.
        /// </summary>
        /// <param name="newState">The object reference state to change to.</param>
        internal void SetState(State newState)
        {
            CurrentState?.ExitState();
            
            CurrentState = newState;
            CurrentState.EnterState();
        }

        /// <summary>
        /// Sets the current state to the state parsed in the Type param.
        /// </summary>
        /// <typeparam name="TState">The type reference of the state to change to.</typeparam>
        internal void SetState<TState>() where TState: State, new() 
            => SetState(GetState<TState>());

        /// <summary>
        /// Moves the current state to the next state set in the state.
        /// </summary>
        internal void MoveToNextState() => CurrentState.MoveToNextState();
        
        /// <summary>
        /// Returns whether the current state equals the type param.
        /// </summary>
        /// <typeparam name="TState">The state to check against.</typeparam>
        /// <returns>Whether the current state equals the type param.</returns>
        public bool CurrentStateIs<TState>() where TState: State, new() 
            => CurrentState.GetType() == new TState().GetType();

        /// <summary>
        /// Returns whether the current state equals the type param.
        /// </summary>
        /// <typeparam name="TState">The state to check against.</typeparam>
        /// <returns>Whether the current state equals the type param.</returns>
        public bool CurrentStateIs(State state)
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
    }
}
