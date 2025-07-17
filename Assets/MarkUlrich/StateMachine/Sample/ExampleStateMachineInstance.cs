using UnityEngine;

namespace MarkUlrich.GenericStateMachine.Sample
{
    public class ExampleStateMachineInstance : StateMachineInstance
    {
        #region Sample
        private void Awake()
        {
            // Example of how to set the initial state.
            StateMachine.SetState<ExampleBootState>();
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
    }
}
