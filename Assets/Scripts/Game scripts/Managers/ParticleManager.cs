using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{


    [SerializeField]
    private string[] _particleKeys;

    [SerializeField]
    private GameObject[] _particleObjects;

    [SerializeField]
    private Dictionary<string, GameObject> _particleDict;

    void Awake()
    {

        _particleDict = new Dictionary<string, GameObject>();

        for (int i = 0; i < _particleObjects.Length; i++)
        {
            _particleDict.Add(_particleKeys[i], _particleObjects[i]);
        }

    }



    public void PlayParticle(string key, Vector3 particlePos)
    {
        Instantiate(_particleDict[key], particlePos, Quaternion.identity);
    }

}
