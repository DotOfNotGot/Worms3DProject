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

    protected bool _hasCollided;

    protected Collider _currentCollider;

    protected ParticleManager _particleManager;

    protected float ProjectileDamage { get => _projectileDamage; }
    protected Rigidbody ProjectileRb { get => _projectileRb; }
    protected int MaxFramesAlive { get => _maxFramesAlive; }



    private void Awake()
    {
        _particleManager = FindObjectOfType<ParticleManager>();
    }

    public void LaunchProjectile(float newLaunchForce)
    {
        _projectileRb.velocity = transform.up * newLaunchForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _hasCollided = true;
        _currentCollider = collision.collider;
    }

}
