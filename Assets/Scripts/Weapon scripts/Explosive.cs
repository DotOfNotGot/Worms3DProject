using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionForce = 10f;

    public void ExplosionCheck()
    {
        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, explosionRadius));

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Rigidbody>() != null)
            {
                collider.attachedRigidbody.AddExplosionForce(explosionForce * collider.attachedRigidbody.mass, transform.position, explosionRadius);
            }
            if (collider.CompareTag("Destroyable"))
            {
                Destroy(collider.gameObject);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }

}
