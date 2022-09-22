using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager currentInputManager;

    [SerializeField]
    private Transform target;
    private Transform _oldTarget;

    [SerializeField, Range(0f, 20f)]
    private float distance = 5f;

    [SerializeField, Min(0f)]
    private float focusRadius = 1f;

    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;

    [SerializeField, Min(0f)]
    private float allignDelay = 5f;

    [SerializeField, Range(1f, 360f)]
    private float rotationSpeed = 90f;

    [SerializeField, Range(-89, 89f)]
    private float minVerticalAngle = -30f, maxVerticalAngle = 60f;

    [SerializeField]
    private LayerMask obstructionMask = -1;

    private Vector3 _focusPoint;
    Vector2 _orbitAngles = new Vector2(45f, 0f);

    private Vector2 _rotationInput;

    private float _lastManualRotationTime;

    [SerializeField]
    private Camera thisCamera;

    private bool _readyForRound = true;
    public bool ReadyForRound { get => _readyForRound; }

    private RaycastHit _hit;

    // Start is called before the first frame update
    void Awake()
    {
        thisCamera = GetComponent<Camera>();
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    private void Update()
    {
        if (currentInputManager != null)
        {
            _rotationInput = new Vector2(currentInputManager.RawCameraRotateInput.y, currentInputManager.RawCameraRotateInput.x);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // TODO: Fix lerp+freeze between turns. Have it not at game start.


        if (ReadyForRound)
        {
            UpdateFocusPoint();
        }

        if (Vector3.Distance(_focusPoint, target.position) > 1 && currentInputManager != enabled)
        {
            _readyForRound = false;
            _focusPoint = Vector3.Lerp(_focusPoint, target.position, 5f * Time.deltaTime);
        }
        else
        {
            StartCoroutine(SetReadyForRoundTrue());
        }

        

        Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(_orbitAngles);
        }
        else
        {
            if (ReadyForRound)
            {
                lookRotation = transform.localRotation;
            }
            else
            {
                lookRotation = target.rotation;
            }
        }
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = _focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * thisCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = target.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;
        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance, obstructionMask))
        {
            _hit = hit;
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    public void SetCameraTarget(Transform newTarget)
    {
        if (target != null && target != newTarget)
        {
            _oldTarget = target;
        }
        target = newTarget;
    }

    public void SetCameraInputManager(PlayerInputManager newInputManager)
    {
        currentInputManager = newInputManager;
    }

    private bool ManualRotation()
    {
        const float e = 0.001f;
        if (_rotationInput.x < e || _rotationInput.x > e || _rotationInput.y < e || _rotationInput.y > e)
        {
            _orbitAngles += rotationSpeed * Time.unscaledDeltaTime * _rotationInput;
            _lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    private bool AutomaticRotation()
    {
        if (Time.unscaledTime - _lastManualRotationTime < allignDelay)
        {
            return false;
        }
        return true;
    }


    private void ConstrainAngles()
    {
        _orbitAngles.x = Mathf.Clamp(_orbitAngles.x, minVerticalAngle, maxVerticalAngle);

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
        Vector3 targetPoint = target.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, _focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }

            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }

            _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);


        }
        else
        {

            _focusPoint = targetPoint;
        }
    }

    private IEnumerator SetReadyForRoundTrue()
    {
        yield return new WaitForSeconds(3f);
        _readyForRound = true;
    }

    public void SetReadyForRoundFalse()
    {
        _readyForRound = false;
    }

    private Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = thisCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * thisCamera.fieldOfView);
            halfExtends.x = halfExtends.y * thisCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }
    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(thisCamera.transform.position, _hit.point);
    }


}
