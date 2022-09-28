using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonChargeableWeapon : WeaponBase
{
    [SerializeField]
    private int amountOfBullets = 1;
    
    private bool _shouldShoot;
    private void Update()
    {
        if (CurrentInputManager.ShootAction.triggered)
        {
            _shouldShoot = true;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentInputManager.GetComponent<UnitController>().HasShot || !_shouldShoot) return;
        _shouldShoot = false;
        UseWeapon(amountOfBullets,1f);
    }
}
