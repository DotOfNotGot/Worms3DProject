using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputManager _inputManager;
    private RaycastHit[] _groundHits;
    private RaycastHit _groundHit;

    [Header("Player Components")]
    [SerializeField]
    private Rigidbody playerRb;
    [SerializeField]
    private CapsuleCollider playerCollider;
    [SerializeField]
    private Transform weaponAttachPoint;


    private Transform _cameraMainTransform;



    [Header("Player Attributes")]
    [SerializeField]
    private float jumpForce = 100f;
    [SerializeField]
    private float speed = 35f;
    [SerializeField]
    private float maxSpeed = 7f;
    [SerializeField]
    private float airSpeedMultiplier = 0.4f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private float maxSlopeAngle = 40f;
    [SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float groundDrag = 1f;
    [SerializeField]
    private Weapon currentWeapon;
    [SerializeField]
    private WeaponSelector playerWeaponSelector;

    [Header("Player Checks")]
    [SerializeField]
    private bool isGrounded;
    private bool _wasGroundedPreviousFrame;
    [SerializeField]
    private bool isOnSlope;
    [SerializeField]
    private bool isOnSteepSlope;
    [SerializeField]
    private bool isJumping;
    [SerializeField]
    private bool shouldAddFallSpeedMultiplier;
    [SerializeField]
    private int framesGrounded;

    public bool IsGrounded { get => isGrounded; }
    public bool WasGroundedPreviousFrame { get => _wasGroundedPreviousFrame; }
    public int FramesGrounded { get => framesGrounded; }

    [Header("Debug Attributes")]
    [SerializeField]
    private float debugRayDuration = 10f;

    private float _distToGround;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        _distToGround = playerCollider.bounds.extents.y;
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
            playerRb.drag = groundDrag;
            shouldAddFallSpeedMultiplier = false;

        }
        else
        {
            playerRb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            _wasGroundedPreviousFrame = true;
            framesGrounded++;
        }
        else
        {
            _wasGroundedPreviousFrame = false;
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


        if (playerRb.velocity.y < 0 && !isGrounded && shouldAddFallSpeedMultiplier)
        {
            AddFallSpeedMultiplier();
        }

    }

    public void ResetGroundedTimer()
    {
        framesGrounded = 0;
    }

    public void HandleWeapon()
    {
        //if (weaponAttachPoint.GetChild(0).transform.localPosition != Vector3.zero)
        //{
        //    weaponAttachPoint.GetChild(0).transform.localPosition = Vector3.zero;
        //}

        currentWeapon = playerWeaponSelector.GetCurrentWeapon();
        
        playerWeaponSelector.SetInputManager(_inputManager);

        if (currentWeapon == null) return;

        if (playerRb.velocity.magnitude < -1 || playerRb.velocity.magnitude > 1)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        else
        {
            currentWeapon.gameObject.SetActive(true);
        }

        
    }
    private void AddFallSpeedMultiplier()
    {
        playerRb.velocity += (fallMultiplier) * Physics.gravity.y * Time.deltaTime * Vector3.up;
    }

    private void OnMove()
    {
        playerRb.AddForce(_moveDirection * (speed * 20f), ForceMode.Force);
        Vector3 clampedPlayerVelocity = Vector3.ClampMagnitude(new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z), maxSpeed);
        clampedPlayerVelocity = new Vector3(clampedPlayerVelocity.x, playerRb.velocity.y, clampedPlayerVelocity.z);
        playerRb.velocity = clampedPlayerVelocity;

        float targetAngle = Mathf.Atan2(_inputManager.RawMoveInput.x, _inputManager.RawMoveInput.y) * Mathf.Rad2Deg + _cameraMainTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, targetAngle, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnJump()
    {
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
