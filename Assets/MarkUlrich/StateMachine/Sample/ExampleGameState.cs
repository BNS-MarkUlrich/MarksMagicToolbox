namespace MarkUlrich.GenericStateMachine.Sample
{
    public class ExampleGameState : State
    {
        private const string LOAD_SCENE_NAME = "Statemachine";

        /// <summary>
        /// Executes code related to entering the Game state.
        /// </summary>
        public override void EnterState()
        {
            base.EnterState();
            SetNextState<ExampleMainMenuState>();

            LoadScene(LOAD_SCENE_NAME);
        }

        /// <summary>
        /// Executes code related to leaving the Game state.
        /// </summary>
        public override void ExitState() => base.ExitState();
    }
}
