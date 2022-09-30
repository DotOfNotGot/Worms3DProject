using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField]
    private string _weaponName;

    [SerializeField] 
    private Transform _projectileSpawnPoint;
    
    protected PlayerInputManager CurrentInputManager { get; private set; }

    [SerializeField] 
    private GameObject _projectilePrefab;
    [SerializeField] 
    private float _launchForce = 50f;

    [SerializeField]
    private int _weaponUses = 1;
    
    private void Awake()
    {
        SetInputManager();
    }
    protected void UseWeapon(float modifier)
    {
        
        var currentProjectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, transform.rotation);
        currentProjectile.GetComponent<ProjectileBase>().LaunchProjectile(_launchForce * modifier);
    }

    protected void HandleWeaponUses()
    {
        _weaponUses--;
        if (_weaponUses == 0)
        {
            CurrentInputManager.gameObject.GetComponent<UnitController>().SetHasShot(true);
        }
    }

    private void SetInputManager()
    {
        CurrentInputManager = GetComponentInParent<PlayerInputManager>();
    }
}
