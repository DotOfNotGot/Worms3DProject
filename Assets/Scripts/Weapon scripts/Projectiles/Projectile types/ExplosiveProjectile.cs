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

    protected int FramesExisted { get; private set; } = 0;

    private bool _explosionCheckDone = false;


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
        if (_explosionCheckDone) return;
        _explosionCheckDone = true;

        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, _explosionRadius));

        foreach (var currentCollider in colliders)
        {
            if (currentCollider.CompareTag("Player"))
            {
                float distanceAdjustedDamage = Mathf.Sin((_explosionRadius - Vector3.Distance(currentCollider.transform.position, transform.position)) / _explosionRadius) * (Mathf.PI / 2) * ProjectileDamage;
                currentCollider.GetComponent<UnitInformation>().StoreDamage(Mathf.Clamp(distanceAdjustedDamage, 0f, ProjectileDamage));

                currentCollider.attachedRigidbody.isKinematic = false;
                currentCollider.GetComponent<UnitController>().ResetGroundedTimer();
            }

            if (currentCollider.GetComponent<Rigidbody>() != null)
            {
                
                currentCollider.attachedRigidbody.AddExplosionForce(_explosionForce * currentCollider.attachedRigidbody.mass, transform.position, _explosionRadius, _explosionUpwardsModifier);
            }
        }
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }
#endif
}
