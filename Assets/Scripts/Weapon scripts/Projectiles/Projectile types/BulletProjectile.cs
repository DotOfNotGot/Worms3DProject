using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : ProjectileBase
{
    private int _framesExisted = 0;
    [SerializeField] private float _bulletPushForce = 10f;

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        _framesExisted += 1;
        if (_framesExisted > MaxFramesAlive)
        {
            _framesExisted = 0;
            _projectilePool.ReleaseWeaponProjectile(_weaponShotFromName, gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        var currentCollider = collision.collider;
        Debug.Log(collision.collider, collision.collider);
        if (currentCollider == null || currentCollider.CompareTag("Projectile")) return;

        if (currentCollider.CompareTag("Player"))
        {
            currentCollider.GetComponent<UnitInformation>().StoreDamage(ProjectileDamage);

            currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
            currentCollider.GetComponent<UnitController>().SetStepsSinceLastJumped();
            currentCollider.attachedRigidbody.isKinematic = false;
            currentCollider.attachedRigidbody.AddForce(transform.up * _bulletPushForce);
        }

        _particleManager.PlayParticle("BulletImpact", transform.position);
        _framesExisted = 0;
        _projectilePool.ReleaseWeaponProjectile(_weaponShotFromName, gameObject);
    }

}