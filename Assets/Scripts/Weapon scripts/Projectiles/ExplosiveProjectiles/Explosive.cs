using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Projectile
{
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionForce = 10f;
    [SerializeField]
    private float explosionUpwardsModifier = 1f;

    private int _framesExisted = 0;
    public int FramesExisted { get => _framesExisted; }

    [SerializeField]
    private int maxFramesAlive = 200;


    private void FixedUpdate()
    {
        _framesExisted += 1;
        if (_framesExisted > maxFramesAlive)
        {
            ExplosionCheck();
        }
    }

    public void ExplosionCheck()
    {
        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, explosionRadius));

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Rigidbody>() != null)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.attachedRigidbody.isKinematic = false;
                    collider.GetComponent<PlayerController>().ResetGroundedTimer();
                }
                collider.attachedRigidbody.AddExplosionForce(explosionForce * collider.attachedRigidbody.mass, transform.position, explosionRadius, explosionUpwardsModifier);
            }
            if (collider.CompareTag("Destroyable"))
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
