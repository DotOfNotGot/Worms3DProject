using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeableWeapon : WeaponBase
{

    private float _launchForceModifier = 0f;

    private bool _chargeLaunchForce;
    private bool _shouldShoot;

    private bool _shootActionState;
    private bool _localShootActionStatePreviousFrame;
    
    private void HandleWeaponInputs()
    {
        _localShootActionStatePreviousFrame = _shootActionState;
        
        if (CurrentInputManager.ShootAction.triggered)
        {
            _shootActionState = true;
        }
        else if (CurrentInputManager.ShootAction.WasReleasedThisFrame())
        {
            _shootActionState = false;
        }
    }
    private void HandleCharging()
    {
        _chargeLaunchForce = _shootActionState;

        if (!_shootActionState && _localShootActionStatePreviousFrame)
        {
            _shouldShoot = true;
        }
    }
    private void Update()
    {
        HandleWeaponInputs();
        HandleCharging();
    }

    private void FixedUpdate()
    {
        if (CurrentInputManager.GetComponent<UnitController>().HasShot) return;
        UseChargedWeapon();
    }
    private void UseChargedWeapon()
    {
        if (_chargeLaunchForce)
        {
            _launchForceModifier += 0.1f;
        }

        if (!_shouldShoot && !(_launchForceModifier >= 5f)) return;
        
        _launchForceModifier /= 5f;
        _shouldShoot = false;
        _chargeLaunchForce = false;
        UseWeapon(1,_launchForceModifier);
    }
}
