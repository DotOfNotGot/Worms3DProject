using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _projectileRb;

    [SerializeField]
    private int _projectileDamage;

    [SerializeField]
    private int _maxFramesAlive = 200;

    [SerializeField]
    private string _projectileSoundKey;

    protected ParticleManager _particleManager;
    protected ProjectilePool _projectilePool;

    protected string _weaponShotFromName;

    protected float ProjectileDamage { get => _projectileDamage; }
    protected Rigidbody ProjectileRb { get => _projectileRb; }
    protected int MaxFramesAlive { get => _maxFramesAlive; }



    private void Awake()
    {
        _particleManager = FindObjectOfType<ParticleManager>();
        _projectilePool = FindObjectOfType<ProjectilePool>();
    }

    public void LaunchProjectile(float newLaunchForce)
    {
        _projectileRb.velocity = transform.up * newLaunchForce;
    }

    public void GetWeaponName(string weaponName)
    {
        _weaponShotFromName = weaponName;
    }
}
