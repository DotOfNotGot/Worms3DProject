using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Explosive
{
    [SerializeField]
    private Rigidbody rocketRb;
    private float _launchForce;

    private int _framesExisted = 0;

    private void Update()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rocketRb.velocity);
    }

    private void FixedUpdate()
    {
        _framesExisted += 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_framesExisted > 5) 
        {
            ExplosionCheck();
            Destroy(gameObject);
        }
    }



    public void LaunchRocket(float newLaunchForce)
    {
        //rocketRb.AddForce(transform.up * newLaunchForce, ForceMode.Impulse);
        rocketRb.velocity = transform.up * newLaunchForce;
    }

}
