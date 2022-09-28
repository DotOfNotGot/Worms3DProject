using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : ProjectileBase
{
    private int _framesExisted = 0;

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        _framesExisted += 1;
        if (_framesExisted > MaxFramesAlive)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    
    private void OnCollisionEnter(Collision collision)
    {
        var currentCollider = collision.collider;
        if (_framesExisted > 2 && collision.collider.CompareTag("Player"))
        {
            currentCollider.GetComponent<UnitInformation>().StoreDamage(distanceAdjustedDamage);

            currentCollider.attachedRigidbody.isKinematic = false;
            currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
        }
    }
    
}
