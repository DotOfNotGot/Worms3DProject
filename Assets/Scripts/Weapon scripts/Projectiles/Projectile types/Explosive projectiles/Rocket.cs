using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : ExplosiveProjectile
{
    private void Update()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, ProjectileRb.velocity);
    }
    void OnCollisionEnter(Collision collision)
    {
        ExplosionCheck();
    }
}
