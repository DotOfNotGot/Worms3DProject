using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Explosive
{
    private void Update()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, ProjectileRb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (FramesExisted > 2)
        {
            ExplosionCheck();
        }
    }

}
