using MarkUlrich.Utils;
using UnityEngine;

namespace MarkUlrich.GenericStateMachine
{
    public class StateMachineInstance : SingletonInstance<StateMachineInstance>
    {
        [SerializeField] private bool _debugMode = true;

        public bool DebugMode
        {
            get => _debugMode;
            private set
            {
                _debugMode = value;
                StateMachine.DebugMode = value;
            }
        }

        private StateMachine StateMachine { get; set; } = new();

        public int HashCode => StateMachine.GetHashCode();

        #region Sample
        private void Awake()
        {
            // Example of how to set the initial state.
            SetState<ExampleBootState>();
        }

        private void Start()
        {
            // Example of how to subscribe to a state instance's OnStateEnter event.
            StateMachine.GetState<ExampleGameState>().OnStateEnter += DebugStateInstance;
        }

        private void DebugStateInstance() => print("I work!");

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StateMachine.MoveToNextState();

            if (Input.GetKeyDown(KeyCode.I))
                DebugPrintStates();
        }

        private void DebugPrintStates()
        {
            print("Currently Subscribed States: ");
            for (int i = 0; i < StateMachine.States.Count; i++)
                print($"{i} : {StateMachine.States[i]} : {StateMachine.States[i].GetHashCode()}");
        }
        #endregion

        public void Subscribe(State state) => StateMachine.Subscribe(state);

        /// <summary>
        /// Sets the current state to the state parsed in the param.
        /// </summary>
        /// <param name="newState">The object reference of the state to change to.</param>
        public void SetState(State newState) => StateMachine.SetState(newState);

        /// <summary>
        /// Sets the current state to the state parsed in the Type param.
        /// </summary>
        /// <typeparam name="TState">The type reference of the state to change to.</typeparam>
        public void SetState<TState>() where TState : State, new()
            => StateMachine.SetState<TState>();

        /// <summary>
        /// Retrieves the state of type <typeparamref name="TState"/> from the owning state machine.
        /// </summary>
        /// <typeparam name="TState">The type of the state to retrieve.</typeparam>
        /// <returns>The state of type <typeparamref name="TState"/>.</returns>
        public State GetState<TState>() where TState : State, new() => StateMachine.GetState<TState>();

        /// <summary>
        /// Moves the state machine to the next state in the static flow.
        /// </summary>
        public void MoveToNextState() => StateMachine.MoveToNextState();

        public void DebugLog(string message)
        {
            if (!DebugMode)
                return;

            Debug.Log(message);
        }
    }
}
