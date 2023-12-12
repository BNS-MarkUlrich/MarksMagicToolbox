public class ExampleBootState : State
{
    private const string LOAD_SCENE_NAME = "Statemachine";

    /// <summary>
    /// Executes code related to entering the Boot state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<ExampleMainMenuState>();

        LoadScene(LOAD_SCENE_NAME);
        
        MoveToNextState();
    }

    /// <summary>
    /// Executes code related to leaving the Boot state.
    /// </summary>
    public override void ExitState() =>  base.ExitState();
}
