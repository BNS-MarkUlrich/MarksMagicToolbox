using UnityEngine;
public class PlayerInputParser : InputParser
{
    [Header("Player")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private FPCameraController _fpCameraController;

    [Header("Other Input")]
    private Vector3 inputMovement;

    private void FixedUpdate()
    {
        switch (CurrentActionMap.name)
        {
            // Player
            case "Player":
                MovePlayer(ReadMoveInput());
                RotatePlayer(GetMouseDelta());
                break;
        }
    }

    protected override void AddListeners(out bool hasListeners)
    {
        hasListeners = true;
    }

    protected override void RemoveListeners()
    {

    }

    // Player
    private void MovePlayer(Vector3 moveInput)
    {
        _playerMovement.MovePlayer(moveInput);
    }

    // MoveInput
    private Vector3 ReadMoveInput()
    {
        var input3D = ControlsActions["Movement"].ReadValue<Vector2>();
        inputMovement.Set(input3D.x, 0, input3D.y);

        return inputMovement;
    }

    private void RotatePlayer(Vector2 rotationDelta)
    {
        _fpCameraController.RotateHorizontally(rotationDelta);
    }
}
