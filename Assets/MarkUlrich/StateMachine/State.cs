using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarkUlrich.GenericStateMachine
{
    /// <summary>
    /// Represents a base class for game states in the game state machine.
    /// </summary>
    public abstract class State
    {
        public string Name => GetType().Name;

        private State _nextState;

        public Action OnStateEnter;
        public Action OnStateExit;

        protected StateMachine OwningStateMachine { get; private set; }
            = StateMachineInstance.Instance.StateMachine;

        protected State() => InitState();

        protected void InitState() => OwningStateMachine.Subscribe(this);

        protected void SetNextState<TState>() where TState : State, new()
            => _nextState = OwningStateMachine.GetState<TState>();

        /// <summary>
        /// Executes code related to entering the state and invokes related events.
        /// </summary>
        // TODO: Add separate virtual method so child classes don't need to call base.EnterState().
        // [ ] rename this state so name can be used in the child class.
        // [ ] make this method private so child classes can't call it.
        public virtual void EnterState()
        {
            OnStateEnter?.Invoke();

            OwningStateMachine.DebugLog
            (
                $"<color=cyan>Entering - {Name}({GetHashCode()}) in StateMachine({OwningStateMachine.GetHashCode()})</color>"
            );
        }

        /// <summary>
        /// Executes code related to leaving the state and invokes related events.
        /// </summary>
        // TODO: Add separate abstract method so child classes don't need to call base.ExitState().
        // [ ] rename this state so name can be used in the child class.
        // [ ] make this method private so child classes can't call it.
        public virtual void ExitState()
        {
            OwningStateMachine.DebugLog
            (
                $"<color=orange>Leaving - {Name}({GetHashCode()}) in StateMachine({OwningStateMachine.GetHashCode()})</color>"
            );

            OnStateExit?.Invoke();
        }

        /// <summary>
        /// Triggers the Owning StateMachine to move to the next state.
        /// </summary>
        public void MoveToNextState()
        {
            if (_nextState == null)
            {
                Debug.LogError
                (
                    "Next State variable was not set! Use 'SetNextState<TState>()' to set it. Cancelling..."
                );
                return;
            }

            OwningStateMachine.SetState(_nextState);
        }

        /// <summary>
        /// Loads a scene by name, optionally forcing a reload if the scene is already active.
        /// </summary>
        protected void LoadScene(string sceneName, bool forceReload = false)
        {
            if (forceReload)
            {
                SceneManager.LoadScene(sceneName);
                OwningStateMachine.DebugLog($"Loaded Scene ({sceneName})");
                return;
            }

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName))
                return;

            SceneManager.LoadScene(sceneName);
            OwningStateMachine.DebugLog($"Loaded Scene ({sceneName})");
        }
    }
}
