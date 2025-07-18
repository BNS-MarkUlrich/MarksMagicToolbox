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

        public StateMachine StateMachine { get; set; } = new();

        // TODO: Add initialising of States here so this no longer needs to be singleton? Upside is you can have multiple StateMachines at once, downside is you need to set the (initial) state(s) in each StateMachineInstance.
        // [ ] make a base statemachine instance which isn't a singleton, then inherit from it and make inherited classes singletons.
        // [ ] if possible, add a <TState> parameter to the constructor of the base class so you can easily set initial states in inherited classes.
    }
}
