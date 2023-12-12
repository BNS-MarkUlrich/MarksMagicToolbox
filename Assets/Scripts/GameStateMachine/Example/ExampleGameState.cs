public class ExampleGameState : State
{
    /// <summary>
    /// Executes code related to entering the Game state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<ExampleBootState>();

        LoadScene("StatemachineTest");
    }

    /// <summary>
    /// Executes code related to leaving the Game state.
    /// </summary>
    public override void ExitState() => base.ExitState();
}
