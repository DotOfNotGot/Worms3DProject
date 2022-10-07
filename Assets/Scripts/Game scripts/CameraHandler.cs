using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraHandler : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _currentInputManager;


    [SerializeField] private Transform _target;

    [SerializeField, Range(0f, 20f)] private float _distance = 5f;

    [SerializeField, Min(0f)] private float _focusRadius = 1f;

    [SerializeField, Range(0f, 1f)] float _focusCentering = 0.5f;

    [SerializeField, Min(0f)] private float _allignDelay = 5f;

    [SerializeField, Range(1f, 360f)] private float _rotationSpeed = 90f;

    [SerializeField, Range(-89, 89f)] private float _minVerticalAngle = -30f, _maxVerticalAngle = 60f;

    [SerializeField] private LayerMask _obstructionMask = -1;

    private Vector3 _focusPoint;
    Vector2 _orbitAngles = new (45f, 0f);

    private Vector2 _rotationInput;

    private float _lastManualRotationTime;

    [SerializeField] private Camera _thisCamera;

    // Start is called before the first frame update
    void Awake()
    {
        _thisCamera = GetComponent<Camera>();
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    private void Update()
    {
        if (_currentInputManager != null)
        {
            _rotationInput = new Vector2(_currentInputManager.RawCameraRotateInput.y, _currentInputManager.RawCameraRotateInput.x);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {

        UpdateFocusPoint();

        Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(_orbitAngles);
        }
        else
        {
            lookRotation = _target.rotation;
        }

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = _focusPoint - lookDirection * _distance;

        Vector3 rectOffset = lookDirection * _thisCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = _target.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;
        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance,
                _obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    public void SetCameraTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    public void SetCameraInputManager(PlayerInputManager newInputManager)
    {
        _currentInputManager = newInputManager;
    }

    private bool ManualRotation()
    {
        const float e = 0.001f;
        if (_rotationInput.x < e || _rotationInput.x > e || _rotationInput.y < e || _rotationInput.y > e)
        {
            _orbitAngles += _rotationSpeed * Time.unscaledDeltaTime * _rotationInput;
            _lastManualRotationTime = Time.unscaledTime;
            return true;
        }

        return false;
    }

    private bool AutomaticRotation()
    {
        if (Time.unscaledTime - _lastManualRotationTime < _allignDelay)
        {
            return false;
        }

        return true;
    }


    private void ConstrainAngles()
    {
        _orbitAngles.x = Mathf.Clamp(_orbitAngles.x, _minVerticalAngle, _maxVerticalAngle);

        if (_orbitAngles.y < 0f)
        {
            _orbitAngles.y += 360f;
        }
        else if (_orbitAngles.y >= 360f)
        {
            _orbitAngles.y -= 360f;
        }
    }

    private void UpdateFocusPoint()
    {
        Vector3 targetPoint = _target.position;
        if (_focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, _focusPoint);
            float t = 1f;
            if (distance > 0.01f && _focusCentering > 0f)
            {
                t = Mathf.Pow(1f - _focusCentering, Time.unscaledDeltaTime);
            }

            if (distance > _focusRadius)
            {
                t = Mathf.Min(t, _focusRadius / distance);
            }

            _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
        }
        else
        {
            _focusPoint = targetPoint;
        }
    }

    private Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = _thisCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * _thisCamera.fieldOfView);
            halfExtends.x = halfExtends.y * _thisCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    private void OnValidate()
    {
        if (_maxVerticalAngle < _minVerticalAngle)
        {
            _maxVerticalAngle = _minVerticalAngle;
        }
    }
}