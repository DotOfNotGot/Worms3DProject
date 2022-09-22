using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody projectileRb;
    public Rigidbody ProjectileRb { get => projectileRb; }

    public void LaunchProjectile(float newLaunchForce)
    {
        projectileRb.velocity = transform.up * newLaunchForce;
    }
}
