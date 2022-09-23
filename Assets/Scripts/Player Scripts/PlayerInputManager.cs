using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private Vector2 rawMoveInput;
    [SerializeField] private Vector2 rawCameraRotateInput;

    private bool _shouldUpdateInputValue = true;

    private InputAction _moveAction;
    private InputAction _rotateCameraAction;
    public InputAction JumpAction { get; private set; }
    public InputAction ShootAction { get; private set; }
    public InputAction SwitchUnitAction { get; private set;}
    public InputAction OpenWeaponSelectorAction { get; private set; }
    public Vector2 RawMoveInput { get => rawMoveInput; }
    public Vector2 RawCameraRotateInput { get => rawCameraRotateInput; }

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _rotateCameraAction = _playerInput.actions["Rotate Camera"];
        JumpAction = _playerInput.actions["Jump"];
        ShootAction = _playerInput.actions["Shoot"];
        SwitchUnitAction = _playerInput.actions["Switch Unit"];
        OpenWeaponSelectorAction = _playerInput.actions["Open Weapon Selector"];
        
    }

    public void SetAllInputZero()
    {
        rawMoveInput = Vector2.zero;
        rawCameraRotateInput = Vector2.zero;
    }


    // Update is called once per frame
    void Update()
    {
        if (_shouldUpdateInputValue)
        {
            rawMoveInput = _moveAction.ReadValue<Vector2>();
            rawCameraRotateInput = _rotateCameraAction.ReadValue<Vector2>();
        }
    }

    public void ToggleInputOn(bool desiredState)
    {
        _shouldUpdateInputValue = desiredState;
        _playerInput.enabled = desiredState;
    }

}
