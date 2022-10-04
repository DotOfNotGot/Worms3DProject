using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    private Dictionary<string, ObjectPool<GameObject>> _projectilePoolDict;

    // Start is called before the first frame update
    void Awake()
    {
        _projectilePoolDict = new Dictionary<string, ObjectPool<GameObject>>();
    }

    public void AddWeaponToDict(string weaponKey, GameObject projectileGo)
    {
        if (_projectilePoolDict.Keys.Contains(weaponKey)) return;
            
        _projectilePoolDict.Add(weaponKey, new ObjectPool<GameObject>(() => Instantiate(projectileGo, transform),
            projectileObject =>
            {
                projectileObject.SetActive(true);
            }, projectileObject =>
            {
                projectileObject.SetActive(false);
            }, projectileObject =>
            {
                Destroy(projectileObject);
            }, false, 50, 55));
    }

    public GameObject GetWeaponProjectile(string weaponKey, Vector3 projectilePosition, Quaternion projectileRotation)
    {
        var currentProjectile = _projectilePoolDict[weaponKey].Get();
        currentProjectile.transform.position = projectilePosition;
        currentProjectile.transform.rotation = projectileRotation;
        return currentProjectile;
    }

    public void ReleaseWeaponProjectile(string weaponKey, GameObject projectileGo)
    {
        _projectilePoolDict[weaponKey].Release(projectileGo);
    }
    
}
