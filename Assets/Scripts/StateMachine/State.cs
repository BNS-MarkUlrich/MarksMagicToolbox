using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateMachine
{
    /// <summary>
    /// Represents a base class for game states in the game state machine.
    /// </summary>
    public abstract class State
    {
        private string Name => GetType().Name;

        private State _nextState;

        protected StateMachineInstance OwningStateMachine { get; private set; } 
            = StateMachineInstance.Instance;

        public Action OnStateEnter;
        public Action OnStateExit;

        protected State() => InitState();

        protected void InitState() => OwningStateMachine.Subscribe(this);
        
        protected void SetNextState<TState>() where TState: State, new() 
            => _nextState = OwningStateMachine.GetState<TState>();

        protected void LoadScene(string sceneName, bool forceRelead = false)
        {
            if (forceRelead)
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"Loaded Scene ({sceneName})");
                return;
            }

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

            Debug.Log($"Entering - {Name}({GetHashCode()}) in StateMachine({OwningStateMachine.StateMachineHashCode})");
        }

        /// <summary>
        /// Executes code related to leaving the state.
        /// </summary>
        public virtual void ExitState()
        {
            Debug.Log($"Leaving - {Name}({GetHashCode()}) in StateMachine({OwningStateMachine.StateMachineHashCode})");

            OnStateExit?.Invoke();
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
        
            OwningStateMachine.SetState(_nextState);
        }
    }
}
