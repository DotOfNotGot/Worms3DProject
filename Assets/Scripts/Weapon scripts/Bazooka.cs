using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : MonoBehaviour, IWeapon
{


    private PlayerInputManager _inputManager;

    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float launchForce = 50f;

    private float _launchForceModifier = 0f;

    private bool _chargeLaunchForce;
    private bool _shouldShoot;

    private void Update()
    {

        if (_inputManager == null)
        {
            return;
        }

        if (_inputManager.ShootAction.triggered && _chargeLaunchForce)
        {
            _shouldShoot = true;
        }

        if (_inputManager.ShootAction.triggered && !_chargeLaunchForce)
        {
            _chargeLaunchForce = true;
        }
    }

    private void FixedUpdate()
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
            Shoot();
        }
    }

    public void Shoot()
    {
        var currentProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        currentProjectile.GetComponent<Rocket>().LaunchRocket(launchForce * _launchForceModifier);
    }

    public void SetInputManager(PlayerInputManager newInputManager)
    {
        _inputManager = newInputManager;
    }

}
