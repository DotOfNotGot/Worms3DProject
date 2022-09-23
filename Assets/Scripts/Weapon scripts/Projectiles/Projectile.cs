using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody projectileRb;

    [SerializeField]
    private int projectileDamage;
    public float ProjectileDamage { get => projectileDamage; }
    public Rigidbody ProjectileRb { get => projectileRb; }

    public void LaunchProjectile(float newLaunchForce)
    {
        projectileRb.velocity = transform.up * newLaunchForce;
    }
}
