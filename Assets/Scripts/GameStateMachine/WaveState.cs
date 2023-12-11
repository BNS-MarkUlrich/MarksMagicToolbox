using System;
using UnityEngine;
public class WaveState : State
{
    public static Action OnWinGame;
    public static Action OnLoseGame;

    public static Action OnWaveStart;
    public static Action OnWaveEnd;

    /// <summary>
    /// Executes code related to entering the Wave state.
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        SetNextState<SetupState>();
        OnWaveStart?.Invoke();

        OnWinGame += WonGame;
        OnLoseGame += LostGame;
        OnWaveEnd += ExitState;
    }
    
    /// <summary>
    /// Executes code related to leaving the Wave state.
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();

        OnWinGame -= WonGame;
        OnLoseGame -= LostGame;
        OnWaveEnd -= ExitState;
    }
    protected void WonGame() => OwningGameStateMachine.SetState<WinState>();
    
    protected void LostGame() => OwningGameStateMachine.SetState<LoseState>();

    /// <summary>
    /// Ends the wave and moves to the Setup phase.
    /// </summary>
    public static void EndWave()
    {
        GameStateMachine.SetState<SetupState>();
    }
}
