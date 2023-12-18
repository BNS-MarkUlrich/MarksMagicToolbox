using UnityEngine;
using UnityEngine.InputSystem;

public class RTSInputParser : InputParser
{
    [Header("Applied Scripts")]
    [SerializeField] private RTSCameraMovement _rtsCameraMovement;
    [SerializeField] private ShipMovement _shipMovement;
    private Vector3 mousePosition;
    private bool activateCameraRotation;
    private Vector3 inputMovement;
    
    public bool IsRefocusingTarget { get; set; }

    private void FixedUpdate()
    {
        switch (CurrentActionMap.name)
        {
            // RTS
            case "RTS":
                MoveCamera(ReadMoveInput());
                ZoomCamera(GetScrollDelta());
                if (ControlsActions["ActivateRotation"].inProgress && activateCameraRotation)
                {
                    RotateCamera(GetMouseDelta());
                    return;
                }

                if (ControlsActions["FocusOnTarget"].inProgress || IsRefocusingTarget)
                {
                    FocusOnTarget();
                    return;
                }

                activateCameraRotation = false;
                FollowMousePosition();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _shipMovement.EnableTarget();
        _rtsCameraMovement.FocusOnTarget();
    }

    // RTS
    protected override void AddListeners(out bool hasListeners)
    {
        ControlsActions["SetShipDestination"].performed += SetTargetDestination;
        ControlsActions["ActivateRotation"].performed += SetRotationTarget;
        hasListeners = true;
    }
    
    protected override void RemoveListeners()
    {
        if (!HasListeners) return;
        ControlsActions["SetShipDestination"].performed -= SetTargetDestination;
        ControlsActions["ActivateRotation"].performed -= SetRotationTarget;
    }

    private void FollowMousePosition()
    {
        mousePosition = ControlsActions["MousePosition"].ReadValue<Vector2>();
    }

    private Vector2 GetScrollDelta()
    {
        return ControlsActions["ScrollZoom"].ReadValue<Vector2>();
    }

    private Vector3 CalculateMouseWorldPosition()
    {
        float distance;
        var ray = Camera.main!.ScreenPointToRay(mousePosition);
        var plane = new Plane(Vector3.up, 0);
        if (plane.Raycast(ray, out distance))
        {
            mousePosition = ray.GetPoint(distance);
        }

        return mousePosition;
    }

    private Vector3 CameraCenterToWorldPos()
    {
        float distance;
        var worldPos = Vector3.zero;
        var ray1 = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        var plane = new Plane(Vector3.up, 0);
        if (plane.Raycast(ray1, out distance))
        {
            worldPos = ray1.GetPoint(distance);
        }

        return worldPos;
    }

    private void SetTargetDestination(InputAction.CallbackContext context)
    {
        if (activateCameraRotation) return; // Remove later
        _shipMovement.SetTargetDestination(CalculateMouseWorldPosition());
    }

    private void SetRotationTarget(InputAction.CallbackContext context)
    {
        activateCameraRotation = true;
        mousePosition = CameraCenterToWorldPos();
    }

    private void FocusOnTarget()
    {
        IsRefocusingTarget = true;
        _rtsCameraMovement.FocusOnTarget();
    }

    private void MoveCamera(Vector3 moveInput)
    {
        _rtsCameraMovement.MoveRTSCamera(moveInput);
    }

    private void RotateCamera(Vector2 rotationDelta)
    {
        _rtsCameraMovement.RotateRTSCamera(rotationDelta);
    }

    private void ZoomCamera(Vector2 zoomDelta)
    {
        _rtsCameraMovement.ZoomRTSCamera(zoomDelta);
    }

    // MoveInput
    private Vector3 ReadMoveInput()
    {
        var input3D = ControlsActions["Movement"].ReadValue<Vector2>();
        inputMovement.Set(input3D.x, input3D.y / 2, input3D.y);

        return inputMovement;
    }
}
