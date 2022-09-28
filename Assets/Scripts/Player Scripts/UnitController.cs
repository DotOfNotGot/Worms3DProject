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
    private Rigidbody unitRb;
    [SerializeField]
    private CapsuleCollider unitCollider;

    private Transform _cameraMainTransform;

    [Header("Unit Attributes")]
    [SerializeField]
    private float jumpForce = 100f;
    [SerializeField]
    private float speed = 35f;
    [SerializeField]
    private float maxSpeed = 7f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private float maxSlopeAngle = 40f;
    [SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float groundDrag = 1f;
    [SerializeField]
    private WeaponBase currentWeapon;
    [SerializeField]
    private WeaponSelector playerWeaponSelector;

    [Header("Unit Checks")]
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool isOnSlope;
    [SerializeField]
    private bool isOnSteepSlope;
    [SerializeField]
    private bool isJumping;
    [SerializeField]
    private bool shouldAddFallSpeedMultiplier;
    [SerializeField]
    private bool hasShot;
    
    [SerializeField]
    private int framesGrounded;
    
    public bool IsGrounded { get => isGrounded; }
    public bool HasShot { get => hasShot; }
    public int FramesGrounded { get => framesGrounded; }

    [Header("Debug Attributes")]
    [SerializeField]
    private float debugRayDuration = 10f;

    private float _distToGround;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        _distToGround = unitCollider.bounds.extents.y;
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

        if (isGrounded)
        {
            unitRb.drag = groundDrag;
            shouldAddFallSpeedMultiplier = false;

        }
        else
        {
            unitRb.drag = 0;
        }
    }

    private void FixedUpdate()
    {

        HandleWeapon();

        if (isGrounded)
        {
            framesGrounded++;
        }
        else
        {
            framesGrounded = 0;
        }
        GroundCheck();

        if (isGrounded && _moveDirection != Vector3.zero)
        {
            OnMove();
        }

        if (isJumping && isGrounded)
        {
            OnJump();
        }

        if (unitRb.velocity.y < 0 && !isGrounded && shouldAddFallSpeedMultiplier)
        {
            AddFallSpeedMultiplier();
        }

    }
    public void ResetGroundedTimer()
    {
        framesGrounded = 0;
    }

    public void SetCurrentWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;
        playerWeaponSelector.SetInputManager(_inputManager);
    }

    private void HandleWeapon()
    {
        if (currentWeapon == null) return;

        if (unitRb.velocity.magnitude < -1 || unitRb.velocity.magnitude > 1)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        else
        {
            currentWeapon.gameObject.SetActive(true);
        }
    }

    public void SetHasShot(bool newState)
    {
        hasShot = newState;
    }

    private void AddFallSpeedMultiplier()
    {
        unitRb.velocity += (fallMultiplier) * Physics.gravity.y * Time.deltaTime * Vector3.up;
    }

    private void OnMove()
    {
        unitRb.AddForce(_moveDirection * (speed * 20f), ForceMode.Force);
        Vector3 clampedPlayerVelocity = Vector3.ClampMagnitude(new Vector3(unitRb.velocity.x, 0, unitRb.velocity.z), maxSpeed);
        clampedPlayerVelocity = new Vector3(clampedPlayerVelocity.x, unitRb.velocity.y, clampedPlayerVelocity.z);
        unitRb.velocity = clampedPlayerVelocity;

        float targetAngle = Mathf.Atan2(_inputManager.RawMoveInput.x, _inputManager.RawMoveInput.y) * Mathf.Rad2Deg + _cameraMainTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, targetAngle, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnJump()
    {
        unitRb.velocity = new Vector3(unitRb.velocity.x, 0, unitRb.velocity.z);
        unitRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = false;
        shouldAddFallSpeedMultiplier = true;
    }

    private IEnumerator JumpBuffer()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
    }
    private void GroundCheck()
    {
        _groundHits = Physics.SphereCastAll(transform.position, 0.25f, transform.TransformDirection(Vector3.down), _distToGround + 0.1f, 3);

        if (_groundHits.Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            isOnSlope = false;
            isOnSteepSlope = false;
        }

        foreach (var hit in _groundHits)
        {
            //Debug.Log(hit.normal);
            _groundHit = hit;
            if (Vector3.Angle(transform.TransformDirection(Vector3.up), hit.normal) == 0)
            {
                isOnSlope = false;
                isOnSteepSlope = false;
                break;
            }

            if (Vector3.Angle(transform.TransformDirection(Vector3.up), hit.normal) < maxSlopeAngle)
            {
                isOnSlope = true;
                isOnSteepSlope = false;
            }
            else if (Vector3.Angle(transform.TransformDirection(Vector3.up), hit.normal) > maxSlopeAngle)
            {
                isOnSlope = false;
                isOnSteepSlope = true;
            }
        }

        Debug.DrawRay(_groundHit.point, _groundHit.normal, Color.red, debugRayDuration, true);
    }

}
