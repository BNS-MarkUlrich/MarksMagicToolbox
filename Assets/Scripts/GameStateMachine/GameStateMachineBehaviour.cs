using System;
using UnityEngine;

public class GameStateMachineBehaviour : MonoBehaviour
{
    #region Testing

    private int _waveCount;
    private int _waveCountThreshold = 5;

    private void Start()
    {
        State setupState = GameStateMachine.GetState<SetupState>();
        setupState.OnStateEnter += DebugStateInstance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            DebugLoseGame();

        if (Input.GetKeyDown(KeyCode.Space))
            DebugNextState();

        if (Input.GetKeyDown(KeyCode.I))
            DebugPrintStates();
    }

    private void DebugLoseGame()
    {
        GameStateMachine.LoseGame();
    }

    private void DebugNextState()
    {
        if (GameStateMachine.CurrentStateIs<WaveState>())
        {
            _waveCount++;
            if (_waveCount >= _waveCountThreshold)
            {
                GameStateMachine.WinGame();
                _waveCount = 0;
                return;
            }
        }

        GameStateMachine.MoveToNextState();
    }

    private void DebugPrintStates()
    {
        print("Currently Subscribed States: ");
        for (int i = 0; i < GameStateMachine.States.Count; i++)
        {
            print($"{i} : {GameStateMachine.States[i]} : {GameStateMachine.States[i].GetHashCode()}");
        }
    }

    private void DebugStateInstance()
    {
        print("I work!");
    }
    
    #endregion
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        this.InitStateMachine();
        SetState<BootState>();
    }

    /// <summary>
    /// Sets the current state to the state parsed in the param.
    /// </summary>
    /// <param name="newState">The object reference of the state to change to.</param>
    public void SetState(State newState) => GameStateMachine.SetState(newState);

    /// <summary>
    /// Sets the current state to the state parsed in the Type param.
    /// </summary>
    /// <typeparam name="TState">The type reference of the state to change to.</typeparam>
    public void SetState<TState>() where TState : State, new()
        => GameStateMachine.SetState<TState>();
}
