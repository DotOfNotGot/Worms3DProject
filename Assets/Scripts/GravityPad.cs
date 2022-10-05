using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPad : MonoBehaviour
{
    [SerializeField]
    private float _padForce = 5f;
    public void ApplyForce(Rigidbody passedRb)
    {
        passedRb.velocity = new Vector3(passedRb.velocity.x, _padForce, passedRb.velocity.z);
        Debug.Log("ooosoo");
    }

}
