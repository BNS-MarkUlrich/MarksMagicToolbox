using MarkUlrich.GenericStateMachine.Sample.States;
using UnityEngine;

namespace MarkUlrich.GenericStateMachine.Sample
{
    /// <summary>
    /// This class is purely to demonstrate what you can do with a StateMachineInstance.
    /// </summary>
    public class ExampleStateMachineInstance : StateMachineBehaviour
    {
        private void Awake()
        {
            // Example of how to set the initial state. Replace with your own state.
            StateMachine.SetState<ExampleBootState>();
        }

        private void Start()
        {
            // Example of how to subscribe to a state instance's OnStateEnter event.
            // Also showcases when action is triggered in relation to entering the state.
            StateMachine.GetState<ExampleGameState>().OnStateEnter += DebugStateEnterAction;

            // Example of how to subscribe to a state instance's OnStateExit event.
            // Also showcases when action is triggered in relation to exiting the state.
            StateMachine.GetState<ExampleGameState>().OnStateExit += DebugStateExitAction;
        }

        private void DebugStateEnterAction()
        {
            print("<color=purple>OnStateEnter</color> action triggered for <color=cyan>ExampleGameState</color>.");
        }

        private void DebugStateExitAction()
        {
            print("<color=purple>OnStateExit</color> action triggered for <color=cyan>ExampleGameState</color>.");
        }

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
            int index = 0;
            foreach (var state in StateMachine.States)
            {
                print($"{index} : {state.Name} : {state.GetHashCode()}");
                index++;
            }
        }
    }
}
