using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private string weaponName;

    [SerializeField]
    private PlayerInputManager _currentInputManager;
    public PlayerInputManager CurrentInputManager { get => _currentInputManager; }

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float launchForce = 50f;


    private bool _hasShot;
    public bool HasShot { get => _hasShot; }
    [SerializeField]
    private bool shootActionState;
    public bool ShootActionState { get => shootActionState; }

    public void HandleWeaponInputs()
    {
        if (_currentInputManager == null)
        {
            return;
        }

        if (_currentInputManager.ShootAction.triggered)
        {
            shootActionState = true;
        }

        if (_currentInputManager.ShootAction.WasReleasedThisFrame())
        {
            shootActionState = false;
        }
    }

    public void UseWeapon(float modifier)
    {
        var currentProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        currentProjectile.GetComponent<Explosive>().LaunchProjectile(launchForce * modifier);
        _hasShot = true;
    }

    public void EnableInputManager()
    {
        if (_currentInputManager == null)
        {
            Debug.Log("fuck");
            return;
        }
            
        _currentInputManager.enabled = true;
    }

    public void DisableInputManager()
    {
        if (_currentInputManager == null)
        {
            Debug.Log("fuck" + gameObject);
            return;
        }
            
        _currentInputManager.enabled = false;
    }

    public void SetInputManager(PlayerInputManager newInputManager)
    {
        _currentInputManager = newInputManager;
    }
}
