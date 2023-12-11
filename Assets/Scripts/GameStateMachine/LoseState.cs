using UnityEngine;

public class LoseState : State
{
    /// <summary>
    /// Executes code related to entering the Lose state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Game lost :(");
    }
    /// <summary>
    /// Executes code related to leaving the Lose state.
    /// </summary>
    public override void ExitState() => base.ExitState();
}
