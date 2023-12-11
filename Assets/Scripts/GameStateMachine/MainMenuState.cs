public class MainMenuState : State
{
    /// <summary>
    /// Executes code related to entering the Main Menu state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<TutorialState>();
    }

    /// <summary>
    /// Executes code related to leaving the Main Menu state.
    /// </summary>
    public override void ExitState() =>  base.ExitState();
}
