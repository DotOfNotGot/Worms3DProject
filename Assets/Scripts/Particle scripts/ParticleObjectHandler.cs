using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObjectHandler : MonoBehaviour
{

    private int _aliveTimer;

    void FixedUpdate()
    {
        _aliveTimer++;
        if (_aliveTimer == 100)
            Destroy(gameObject);
    }
}
