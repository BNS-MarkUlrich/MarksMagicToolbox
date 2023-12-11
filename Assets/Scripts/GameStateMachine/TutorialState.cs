public class TutorialState : State
{
    /// <summary>
    /// Executes code related to entering the Tutorial state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<SetupState>();
    }

    /// <summary>
    /// Executes code related to leaving the Tutorial state.
    /// </summary>
    public override void ExitState() =>  base.ExitState();
}
