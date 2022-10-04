using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    [SerializeField]
    private string[] _particleKeys;

    [SerializeField]
    private GameObject[] _particleObjects;
    
    private Dictionary<string, ObjectPool<GameObject>> _particlePoolsDict;

    void Awake()
    {
        _particlePoolsDict = new Dictionary<string, ObjectPool<GameObject>>();

        for (int i = 0; i < _particleObjects.Length; i++)
        {
            var i1 = i;
            _particlePoolsDict.Add(_particleKeys[i], new ObjectPool<GameObject>(() => Instantiate(_particleObjects[i1], transform.GetChild(0)), particleObject =>
            {
                particleObject.SetActive(true);
            }, particleObject =>
            {
                particleObject.SetActive(false);
            }, particleObject =>
            {
                Destroy(particleObject);
            }, false, 50, 55));
        }
    }

    public void PlayParticle(string key, Vector3 particlePos)
    {
        var currentParticleObject = _particlePoolsDict[key].Get();
        currentParticleObject.transform.position = particlePos;
        currentParticleObject.GetComponent<ParticleSystem>().Play();
        currentParticleObject.GetComponent<AudioSource>().Play();
    }

    public void ReturnParticleObjectToPool(string key, GameObject particleObjectToReturn)
    {
        _particlePoolsDict[key].Release(particleObjectToReturn);
    }

}
