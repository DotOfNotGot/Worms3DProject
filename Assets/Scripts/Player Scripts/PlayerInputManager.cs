using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private Vector2 rawMoveInput;
    [SerializeField] private Vector2 rawCameraRotateInput;

    private InputAction _moveAction;
    private InputAction _rotateCameraAction;
    public InputAction JumpAction { get; private set; }
    public InputAction ShootAction { get; private set; }
    public InputAction SwitchUnitAction { get; private set;}
    public Vector2 RawMoveInput { get => rawMoveInput; }
    public Vector2 RawCameraRotateInput { get => rawCameraRotateInput; }

    // Start is called before the first frame update
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _rotateCameraAction = _playerInput.actions["Rotate Camera"];
        JumpAction = _playerInput.actions["Jump"];
        ShootAction = _playerInput.actions["Shoot"];
        SwitchUnitAction = _playerInput.actions["Switch Unit"];
    }

    // Update is called once per frame
    void Update()
    {
        rawMoveInput = _moveAction.ReadValue<Vector2>();
        rawCameraRotateInput = _rotateCameraAction.ReadValue<Vector2>();
    }
}