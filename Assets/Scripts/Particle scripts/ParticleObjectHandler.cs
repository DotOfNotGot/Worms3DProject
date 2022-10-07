using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObjectHandler : MonoBehaviour
{
    [SerializeField]
    private string _objectKey;
    
    private int _aliveTimer;
    protected ParticleManager _particleManager;

    private void Awake()
    {
        _particleManager = FindObjectOfType<ParticleManager>();
    }

    void FixedUpdate()
    {
        _aliveTimer++;
        if (_aliveTimer != 100) return;
        _aliveTimer = 0;
        _particleManager.ReturnParticleObjectToPool(_objectKey, gameObject);

    }
}
