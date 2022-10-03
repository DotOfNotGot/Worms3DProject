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
            Destroy(gameObject);
        }

        if (_hasCollided)
        {
            HandleProjectileCollision();
        }

    }

    private void HandleProjectileCollision()
    {
        if (_currentCollider == null || _currentCollider.CompareTag("Projectile")) return;



        if (_currentCollider.CompareTag("Player"))
        {
            _currentCollider.GetComponent<UnitInformation>().StoreDamage(ProjectileDamage);

            _currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
            _currentCollider.attachedRigidbody.isKinematic = false;
            _currentCollider.attachedRigidbody.AddForce(ProjectileRb.velocity.normalized * _bulletPushForce);
        }
        _particleManager.PlayParticle("BulletImpact", transform.position);
        Destroy(gameObject);
    }
}