using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponBase : MonoBehaviour
{
    [SerializeField]
    private string _weaponName;

    [SerializeField]
    private AudioClip _weaponSound;

    [SerializeField] 
    private Transform _projectileSpawnPoint;
    
    protected PlayerInputManager CurrentInputManager { get; private set; }

    [SerializeField] 
    private GameObject _projectilePrefab;
    [SerializeField] 
    private float _launchForce = 50f;

    [SerializeField]
    private int _weaponUses = 1;

    private int _weaponUsesRemaining;
    
    private ProjectilePool _projectilePool;


    private void Awake()
    {
        _projectilePool = FindObjectOfType<ProjectilePool>();
        _projectilePool.AddWeaponToDict(_weaponName, _projectilePrefab);
        SetInputManager();
        _weaponUsesRemaining = _weaponUses;
    }
    protected void UseWeapon(float modifier)
    {
        GetComponentInParent<WeaponSelector>().PlayWeaponSound(_weaponSound);
        var currentProjectile = _projectilePool.GetWeaponProjectile(_weaponName, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
        var currentProjectileBase = currentProjectile.GetComponent<ProjectileBase>();
        currentProjectileBase.GetWeaponName(_weaponName);
        currentProjectileBase.LaunchProjectile(_launchForce * modifier);
    }

    protected void HandleWeaponUses()
    {
        _weaponUsesRemaining--;
        if (_weaponUsesRemaining <= 0)
        {
            CurrentInputManager.gameObject.GetComponent<UnitController>().SetHasShot(true);
            _weaponUsesRemaining = _weaponUses;
        }
    }

    private void SetInputManager()
    {
        CurrentInputManager = GetComponentInParent<PlayerInputManager>();
    }
}
