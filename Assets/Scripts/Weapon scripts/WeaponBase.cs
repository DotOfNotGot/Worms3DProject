using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField]
    private string weaponName;

    protected PlayerInputManager CurrentInputManager { get; private set; }

    [SerializeField] 
    private GameObject projectilePrefab;
    [SerializeField] 
    private float launchForce = 50f;

    [SerializeField]
    private int weaponUses = 1;
    
    private void Awake()
    {
        SetInputManager();
    }

    

    protected void UseWeapon(int amountOfProjectiles, float modifier)
    {
        for (int i = 0; i < amountOfProjectiles; i++)
        {
            var currentProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            currentProjectile.GetComponent<ProjectileBase>().LaunchProjectile(launchForce * modifier);
        }

        if (weaponUses == 0)
        {
            CurrentInputManager.gameObject.GetComponent<UnitController>().SetHasShot(true);
        }
        else
        {
            weaponUses--;
        }
    }
    private void SetInputManager()
    {
        CurrentInputManager = GetComponentInParent<PlayerInputManager>();
    }
}
