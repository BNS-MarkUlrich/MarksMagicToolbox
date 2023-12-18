using UnityEngine;

public class ManualFlightInputParser : InputParser
{
    [Header("Applied Scripts")]
    [SerializeField] protected ManualFlightMovement manualFlightMovement;
    [SerializeField] protected RTSCameraMovement rtsCameraMovement;

    [Header("Input Variables")]
    [SerializeField] private Vector2 inputMovement;
    [SerializeField] private bool ignorePitch = true;
    private Vector2 mouseDelta;

    protected override void AddListeners(out bool hasListeners)
    {
        hasListeners = true;
    }

    protected override void RemoveListeners()
    {
        if (!HasListeners) return;
    }

    private void FixedUpdate()
    {
        ApplyForwardThrust(ReadMoveInput().y);
        ApplyLateralThrust(ReadMoveInput());
        //ApplyTurningThrust(ReadMoveInput()); Mark: Testing some stuff
        RotateCamera(GetMouseDelta());
        ZoomCamera(GetScrollDelta());
        rtsCameraMovement.LockToTarget();
    }

    private Vector2 ReadMoveInput()
    {
        inputMovement = ControlsActions["Movement"].ReadValue<Vector2>();
        return inputMovement;
    }
    
    private Vector2 GetScrollDelta()
    {
        return ControlsActions["ScrollZoom"].ReadValue<Vector2>();
    }

    private void ApplyForwardThrust(float thrust)
    {
        manualFlightMovement.ApplyForwardThrust(thrust);
    }
    
    private void ApplyLateralThrust(Vector2 thrust)
    {
        manualFlightMovement.ApplyLateralThrust(thrust, ignorePitch);
    }
    
    private void ApplyTurningThrust(Vector2 thrust)
    {
        manualFlightMovement.ApplyTurningThrust(thrust, ignorePitch);
    }

    private void RotateCamera(Vector2 rotationDelta)
    {
        rtsCameraMovement.RotateRTSCamera(rotationDelta);
    }
    
    private void ZoomCamera(Vector2 zoomDelta)
    {
        rtsCameraMovement.ZoomRTSCamera(zoomDelta);
    }
}
