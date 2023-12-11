public class SetupState : State
{
    /// <summary>
    /// Executes code related to entering the Setup state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<WaveState>();
    }

    /// <summary>
    /// Executes code related to leaving the Setup state.
    /// </summary>
    public override void ExitState() =>  base.ExitState();
}
