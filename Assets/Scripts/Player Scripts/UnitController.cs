using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private PlayerInputManager _inputManager;
    private RaycastHit[] _groundHits;
    private RaycastHit _groundHit;

    [Header("Unit Components")]
    [SerializeField]
    private Rigidbody _unitRb;
    [SerializeField]
    private CapsuleCollider _unitCollider;

    private Transform _cameraMainTransform;

    [Header("Unit Attributes")]
    [SerializeField]
    private float _jumpForce = 100f;
    [SerializeField]
    private float _speed = 35f;
    [SerializeField]
    private float _maxSpeed = 7f;
    [SerializeField]
    private float _rotationSpeed = 5f;
    [SerializeField]
    private float _maxSlopeAngle = 40f;
    [SerializeField]
    private float _fallMultiplier = 2.5f;
    [SerializeField]
    private float _groundDrag = 1f;
    [SerializeField]
    private WeaponBase _currentWeapon;
    [SerializeField]
    private WeaponSelector _playerWeaponSelector;

    [Header("Unit Checks")]
    [SerializeField]
    private bool _isGrounded;
    [SerializeField]
    private bool _isOnSlope;
    [SerializeField]
    private bool _isOnSteepSlope;
    [SerializeField]
    private bool _isJumping;
    [SerializeField]
    private bool _shouldAddFallSpeedMultiplier;
    [SerializeField]
    private bool _hasShot;
    
    [SerializeField]
    private int _framesGrounded;
    
    public bool IsGrounded { get => _isGrounded; }
    public bool HasShot { get => _hasShot; }
    public int FramesGrounded { get => _framesGrounded; }

    [Header("Debug Attributes")]
    [SerializeField]
    private float _debugRayDuration = 10f;

    private float _distToGround;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        _distToGround = _unitCollider.bounds.extents.y;
    }

    private void Start()
    {
        _cameraMainTransform = Camera.main.transform;
    }

    private void Update()
    {

        _moveDirection = new Vector3(_inputManager.RawMoveInput.x, 0, _inputManager.RawMoveInput.y);
        _moveDirection = _cameraMainTransform.forward * _moveDirection.z + _cameraMainTransform.right * _moveDirection.x;
        _moveDirection.y = 0f;

        if (_inputManager.JumpAction.triggered)
        {
            StartCoroutine(JumpBuffer());
        }

        if (_isGrounded)
        {
            _unitRb.drag = _groundDrag;
            _shouldAddFallSpeedMultiplier = false;

        }
        else
        {
            _unitRb.drag = 0;
        }
    }

    private void FixedUpdate()
    {

        HandleWeapon();

        if (_isGrounded)
        {
            _framesGrounded++;
        }
        else
        {
            _framesGrounded = 0;
        }
        GroundCheck();

        if (_isGrounded && _moveDirection != Vector3.zero)
        {
            OnMove();
        }

        if (_isJumping && _isGrounded)
        {
            OnJump();
        }

        if (_unitRb.velocity.y < 0 && !_isGrounded && _shouldAddFallSpeedMultiplier)
        {
            AddFallSpeedMultiplier();
        }

    }
    public void ResetGroundedTimer()
    {
        _framesGrounded = 0;
    }

    public void SetCurrentWeapon(WeaponBase newWeapon)
    {
        _currentWeapon = newWeapon;
        _playerWeaponSelector.SetInputManager(_inputManager);
    }

    private void HandleWeapon()
    {
        if (_currentWeapon == null) return;

        if (_unitRb.velocity.magnitude < -1 || _unitRb.velocity.magnitude > 1)
        {
            _currentWeapon.gameObject.SetActive(false);
        }
        else
        {
            _currentWeapon.gameObject.SetActive(true);
        }
    }
    public void SetHasShot(bool newState)
    {
        _hasShot = newState;
    }

    private void AddFallSpeedMultiplier()
    {
        _unitRb.velocity += (_fallMultiplier) * Physics.gravity.y * Time.deltaTime * Vector3.up;
    }

    private void OnMove()
    {
        _unitRb.AddForce(_moveDirection * (_speed * 20f), ForceMode.Force);
        Vector3 clampedPlayerVelocity = Vector3.ClampMagnitude(new Vector3(_unitRb.velocity.x, 0, _unitRb.velocity.z), _maxSpeed);
        clampedPlayerVelocity = new Vector3(clampedPlayerVelocity.x, _unitRb.velocity.y, clampedPlayerVelocity.z);
        _unitRb.velocity = clampedPlayerVelocity;

        float targetAngle = Mathf.Atan2(_inputManager.RawMoveInput.x, _inputManager.RawMoveInput.y) * Mathf.Rad2Deg + _cameraMainTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, targetAngle, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    private void OnJump()
    {
        _unitRb.velocity = new Vector3(_unitRb.velocity.x, 0, _unitRb.velocity.z);
        _unitRb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _isJumping = false;
        _shouldAddFallSpeedMultiplier = true;
    }

    private IEnumerator JumpBuffer()
    {
        _isJumping = true;
        yield return new WaitForSeconds(0.1f);
        _isJumping = false;
    }
    private void GroundCheck()
    {
        _groundHits = Physics.SphereCastAll(transform.position, 0.25f, transform.TransformDirection(Vector3.down), _distToGround + 0.1f, 3);

        if (_groundHits.Length > 0)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
            _isOnSlope = false;
            _isOnSteepSlope = false;
        }

        foreach (var hit in _groundHits)
        {
            //Debug.Log(hit.normal);
            _groundHit = hit;
            if (Vector3.Angle(transform.TransformDirection(Vector3.up), hit.normal) == 0)
            {
                _isOnSlope = false;
                _isOnSteepSlope = false;
                break;
            }

            if (Vector3.Angle(transform.TransformDirection(Vector3.up), hit.normal) < _maxSlopeAngle)
            {
                _isOnSlope = true;
                _isOnSteepSlope = false;
            }
            else if (Vector3.Angle(transform.TransformDirection(Vector3.up), hit.normal) > _maxSlopeAngle)
            {
                _isOnSlope = false;
                _isOnSteepSlope = true;
            }
        }

        Debug.DrawRay(_groundHit.point, _groundHit.normal, Color.red, _debugRayDuration, true);
    }

}
