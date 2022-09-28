using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : ProjectileBase
{
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionForce = 10f;
    [SerializeField]
    private float explosionUpwardsModifier = 1f;

    protected int FramesExisted { get; private set; } = 0;


    private void FixedUpdate()
    {
        FramesExisted += 1;
        if (FramesExisted > MaxFramesAlive)
        {
            ExplosionCheck();
        }
    }

    protected void ExplosionCheck()
    {
        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, explosionRadius));

        foreach (var currentCollider in colliders)
        {
            if (currentCollider.GetComponent<Rigidbody>() != null)
            {
                if (currentCollider.CompareTag("Player"))
                {
                    float distanceAdjustedDamage = ProjectileDamage * (1 / (currentCollider.transform.position - transform.position).sqrMagnitude);
                    currentCollider.GetComponent<UnitInformation>().StoreDamage(distanceAdjustedDamage);

                    currentCollider.attachedRigidbody.isKinematic = false;
                    currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
                }
                currentCollider.attachedRigidbody.AddExplosionForce(explosionForce * currentCollider.attachedRigidbody.mass, transform.position, explosionRadius, explosionUpwardsModifier);
            }
            if (currentCollider.CompareTag("Destroyable"))
            {
            }
        }
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
#endif
}
