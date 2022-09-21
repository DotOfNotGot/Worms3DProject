using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeWeapon : Chargeable
{
    private void Update()
    {
        HandleWeaponInputs();
        HandleCharging();
    }
    private void FixedUpdate()
    {
        UseChargedWeapon();
    }
}
