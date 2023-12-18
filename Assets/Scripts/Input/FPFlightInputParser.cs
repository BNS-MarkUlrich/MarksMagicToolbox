using UnityEngine;

public class FPFlightInputParser : InputParser
{
    [Header("Applied Scripts")]
    [SerializeField] protected ManualFlightMovement manualFlightMovement;
    [SerializeField] protected FPCameraController fpCameraController;
    
    [Header("Input Variables")]
    [SerializeField] protected float cameraBoundsRadius = 15;
    [SerializeField] protected float cameraSnapRadius = 1;
    [SerializeField] protected bool ignorePitch;
    protected Vector2 inputMovement;
    
    protected override void AddListeners(out bool hasListeners)
    {
        hasListeners = true;
    }

    protected override void RemoveListeners()
    {
        if (!HasListeners) return;
    }

    protected virtual void FixedUpdate()
    {
        ApplyForwardThrust(ReadMoveInput().y);
        ApplyPitchThrust(ReadMoveInput().x);
        ApplyTurningThrust(GetMouseDelta());
    }
    
    protected Vector2 ReadMoveInput()
    {
        inputMovement = ControlsActions["Movement"].ReadValue<Vector2>();
        return inputMovement;
    }

    protected void ApplyForwardThrust(float thrust)
    {
        manualFlightMovement.ApplyForwardThrust(thrust);
    }
    
    protected void ApplyPitchThrust(float thrust)
    {
        manualFlightMovement.ApplyPitchThrust(thrust);
    }
    
    protected void ApplyTurningThrust(Vector2 thrust)
    {
        fpCameraController.LookRotationClamped(thrust, cameraBoundsRadius, cameraSnapRadius);
        manualFlightMovement.ApplyTurningThrust(fpCameraController.NormalizedVelocity, ignorePitch);
    }
}
