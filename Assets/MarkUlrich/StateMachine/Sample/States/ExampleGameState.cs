namespace MarkUlrich.GenericStateMachine.Sample.States
{
    public class ExampleGameState : State
    {
        private const string LOAD_SCENE_NAME = "Statemachine";

        public override void Enter()
        {
            SetNextState<ExampleMainMenuState>();

            LoadScene(LOAD_SCENE_NAME);
        }

        public override void Exit() { }
    }
}
