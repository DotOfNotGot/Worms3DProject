using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chargeable : Weapon
{

    private float _launchForceModifier = 0f;

    private bool _chargeLaunchForce;
    private bool _shouldShoot;

    private bool _localShootActionState;
    private bool _localShootActionStatePreviousFrame;

    public void HandleCharging()
    {
        _localShootActionStatePreviousFrame = _localShootActionState;
        _localShootActionState = ShootActionState;
        if (_localShootActionState)
        {
            _chargeLaunchForce = true;
        }
        else
        {
            _chargeLaunchForce = false;
        }

        if (!_localShootActionState && _localShootActionStatePreviousFrame)
        {
            _shouldShoot = true;
        }
    }

    public void UseChargedWeapon()
    {
        if (_chargeLaunchForce)
        {
            _launchForceModifier += 0.1f;
        }

        if (_shouldShoot || _launchForceModifier >= 5f)
        {
            _launchForceModifier /= 5f;
            _shouldShoot = false;
            _chargeLaunchForce = false;
            if (!HasShot)
            {
                UseWeapon(_launchForceModifier);
            }
        }
    }
}
