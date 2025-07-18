namespace MarkUlrich.GenericStateMachine.Sample.States
{
    public class ExampleMainMenuState : State
    {
        public override void Enter()
        {
            SetNextState<ExampleGameState>();
        }

        public override void Exit() { }
    }
}
