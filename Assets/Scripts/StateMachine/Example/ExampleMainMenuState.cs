namespace StateMachine
{
    public class ExampleMainMenuState : State
    {
        /// <summary>
        /// Executes code related to entering the Main Menu state.
        /// </summary>
        public override void EnterState()
        {
            base.EnterState();
            SetNextState<ExampleGameState>();
        }

        /// <summary>
        /// Executes code related to leaving the Main Menu state.
        /// </summary>
        public override void ExitState() => base.ExitState();
    }
}
