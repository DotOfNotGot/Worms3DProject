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

    
    protected float ProjectileDamage { get => _projectileDamage; }
    protected Rigidbody ProjectileRb { get => _projectileRb; }
    protected int MaxFramesAlive { get => _maxFramesAlive; }

    public void LaunchProjectile(float newLaunchForce)
    {
        _projectileRb.velocity = transform.up * newLaunchForce;
    }
}
