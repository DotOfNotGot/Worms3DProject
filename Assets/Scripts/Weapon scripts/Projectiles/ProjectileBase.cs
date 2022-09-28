using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField]
    private Rigidbody projectileRb;

    [SerializeField]
    private int projectileDamage;

    [SerializeField]
    private int maxFramesAlive = 200;

    
    protected float ProjectileDamage { get => projectileDamage; }
    protected Rigidbody ProjectileRb { get => projectileRb; }
    protected int MaxFramesAlive { get => maxFramesAlive; }

    public void LaunchProjectile(float newLaunchForce)
    {
        projectileRb.velocity = transform.up * newLaunchForce;
    }
}
