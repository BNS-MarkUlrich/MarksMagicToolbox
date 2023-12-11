public class BootState : State
{
    private const string LOAD_SCENE_NAME = "Develop";

    /// <summary>
    /// Executes code related to entering the Boot state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<MainMenuState>();

        //LoadScene(LOAD_SCENE_NAME);
        
        MoveToNextState();
    }

    /// <summary>
    /// Executes code related to leaving the Boot state.
    /// </summary>
    public override void ExitState() =>  base.ExitState();
}
