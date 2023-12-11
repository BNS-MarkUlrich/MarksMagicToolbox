using UnityEngine;
public class WinState : State
{
    /// <summary>
    /// Executes code related to entering the Win state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();            
        Debug.Log("Game won! :D");
    }

    /// <summary>
    /// Executes code related to leaving the Win state.
    /// </summary>
    public override void ExitState() => base.ExitState();
}
