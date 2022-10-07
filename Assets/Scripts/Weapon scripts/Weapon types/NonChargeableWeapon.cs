using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonChargeableWeapon : WeaponBase
{
    [SerializeField]
    private int _amountOfBullets = 1;

    private int _bulletsToShoot;
    
    private bool _shouldShoot;

    private bool _shotInProgress;

    [SerializeField]
    private float _timeBetweenProjectiles = 0.1f;

    private void Start()
    {
        _bulletsToShoot = _amountOfBullets;
    }

    private void Update()
    {
        if (CurrentInputManager.ShootAction.triggered && !_shouldShoot && !CurrentInputManager.GetComponent<UnitController>().HasShot)
        {
            _shouldShoot = true;
        }
    }

    private void FixedUpdate()
    {
        if (!_shouldShoot) return;

        if (!_shotInProgress && _bulletsToShoot != 0)
        {
            StartCoroutine(ShootProjectile());
        }

        if (_bulletsToShoot == 0)
        {
            _shouldShoot = false;
            _bulletsToShoot = _amountOfBullets;
            HandleWeaponUses();
        }

    }

    private IEnumerator ShootProjectile()
    {
        UseWeapon(1f);
        _shotInProgress = true;

        yield return new WaitForSeconds(_timeBetweenProjectiles);
        _bulletsToShoot--;
        _shotInProgress = false;
    }

}
