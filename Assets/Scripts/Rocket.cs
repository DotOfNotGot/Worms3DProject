using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody rocketRb;
    void Awake()
    {
        rocketRb.AddForce(transform.up * 10f, ForceMode.Impulse);
    }

    private void Update()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rocketRb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Destroyable"))
        {
            Destroy(collision.gameObject);
        }
        if (!collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
