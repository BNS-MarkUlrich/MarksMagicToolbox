using UnityEngine;
using UnityEngine.InputSystem;

public class TPInputParser : InputParser
{
    [Header("Applied Scripts")]
    [SerializeField] protected TPMovement tpMovement;
    [SerializeField] protected RTSCameraMovement rtsCameraMovement;

    [Header("Input Variables")]
    [SerializeField] private Vector2 inputMovement;
    [SerializeField] private bool ignorePitch = true;
    private Vector2 mouseDelta;

    protected override void AddListeners(out bool hasListeners)
    {
        hasListeners = true;
        ControlsActions["Jump"].performed += Jump;
    }

    protected override void RemoveListeners()
    {
        if (!HasListeners) return;
        ControlsActions["Jump"].performed -= Jump;
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
        tpMovement.ApplyForwardThrust(thrust);
    }
    
    private void ApplyLateralThrust(Vector2 thrust)
    {
        tpMovement.ApplyLateralThrust(thrust, ignorePitch);
    }

    private void RotateCamera(Vector2 rotationDelta)
    {
        rtsCameraMovement.RotateRTSCamera(rotationDelta);
    }
    
    private void ZoomCamera(Vector2 zoomDelta)
    {
        rtsCameraMovement.ZoomRTSCamera(zoomDelta);
    }

    private void Jump(InputAction.CallbackContext _)
    {
        tpMovement.Jump(3);
    }
}
