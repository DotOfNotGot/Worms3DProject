using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private Vector2 rawMoveInput;

    private InputAction _moveAction;
    public InputAction JumpAction { get; private set; }
    public InputAction ShootAction { get; private set; }
    public Vector2 RawMoveInput { get => rawMoveInput; }

    // Start is called before the first frame update
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        JumpAction = _playerInput.actions["Jump"];
        ShootAction = _playerInput.actions["Shoot"];
    }

    // Update is called once per frame
    void Update()
    {
        rawMoveInput = _moveAction.ReadValue<Vector2>();
    }
}
