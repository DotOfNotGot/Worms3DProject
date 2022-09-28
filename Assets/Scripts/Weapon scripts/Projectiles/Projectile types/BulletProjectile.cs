using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : ProjectileBase
{
    private int _framesExisted = 0;
    [SerializeField] private float bulletPushForce = 10f;

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        _framesExisted += 1;
        if (_framesExisted > MaxFramesAlive)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        var currentCollider = collision.collider;
        if (currentCollider.CompareTag("Player"))
        {
            currentCollider.GetComponent<UnitInformation>().StoreDamage(ProjectileDamage);

            currentCollider.attachedRigidbody.isKinematic = false;
            currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
            currentCollider.attachedRigidbody.AddForce(ProjectileRb.velocity.normalized * bulletPushForce);
        }
        Destroy(gameObject);
    }
}