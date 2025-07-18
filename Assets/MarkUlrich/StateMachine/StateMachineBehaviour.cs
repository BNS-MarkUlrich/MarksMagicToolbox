using MarkUlrich.Utils;
using UnityEngine;

namespace MarkUlrich.GenericStateMachine
{
    public class StateMachineBehaviour : SingletonInstance<StateMachineBehaviour>
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

        public StateMachine StateMachine { get; private set; } = new();

        public StateMachineBehaviour() => StateMachine.Instance = this;
    }
}
