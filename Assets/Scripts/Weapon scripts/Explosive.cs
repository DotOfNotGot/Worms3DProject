using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainDestruction))]
public class Explosive : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionForce = 10f;

    private TerrainDestruction _terrainDestruction;

    private void Awake()
    {
        _terrainDestruction = GetComponent<TerrainDestruction>();
    }

    public void ExplosionCheck()
    {
        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, explosionRadius));

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Rigidbody>() != null)
            {
                //collider.attachedRigidbody.AddExplosionForce(explosionForce * collider.attachedRigidbody.mass, transform.position, explosionRadius);
            }
            if (collider.CompareTag("Destroyable"))
            {
                _terrainDestruction.GetTerrainToDestroy(collider.gameObject);
            }
        }
        //Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }

}
