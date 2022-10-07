using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : ProjectileBase
{
    [SerializeField]
    private float _explosionRadius = 5f;
    [SerializeField]
    private float _explosionForce = 10f;
    [SerializeField]
    private float _explosionUpwardsModifier = 1f;


    private int _framesExisted = 0;

    private void FixedUpdate()
    {
        _framesExisted += 1;
        if (_framesExisted > MaxFramesAlive)
        {
            ExplosionCheck();
        }
    }

    protected void ExplosionCheck()
    {
        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, _explosionRadius));

        foreach (var currentCollider in colliders)
        {
            if (currentCollider.CompareTag("Player"))
            {
                float distanceAdjustedDamage = Mathf.Sin((_explosionRadius - Vector3.Distance(currentCollider.transform.position, transform.position)) / _explosionRadius) * (Mathf.PI / 2) * ProjectileDamage;
                currentCollider.GetComponent<UnitInformation>().StoreDamage(Mathf.Clamp(distanceAdjustedDamage, 0f, ProjectileDamage));

                currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
                currentCollider.GetComponent<UnitController>().SetStepsSinceLastJumped();

                currentCollider.attachedRigidbody.isKinematic = false;
            }

            if (currentCollider.GetComponent<Rigidbody>() != null)
            {
                currentCollider.attachedRigidbody.AddExplosionForce(_explosionForce * currentCollider.attachedRigidbody.mass, transform.position, _explosionRadius, _explosionUpwardsModifier);
            }
        }
        _particleManager.PlayParticle("Explosion", transform.position);
        _framesExisted = 0;
        _projectilePool.ReleaseWeaponProjectile(_weaponShotFromName, gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }
#endif
}
