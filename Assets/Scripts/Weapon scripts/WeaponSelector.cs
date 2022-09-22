using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponSelector : MonoBehaviour
{

    private PlayerInputManager _inputManager;

    [SerializeField]
    private TMP_Dropdown weaponsUIList;

    [SerializeField]
    private GameObject weaponSelectorObject;

    [SerializeField]
    private List<Weapon> weapons;
    [SerializeField]
    private List<GameObject> weaponGOs;


    private Weapon _currentWeapon;
    private GameObject _currentWeaponGameObject;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var weapon in weapons)
        {
            if (weapon != null)
            {
                var currentWeaponGameObject = Instantiate(weapon.gameObject, gameObject.transform);
                currentWeaponGameObject.SetActive(false);
                weaponGOs.Add(currentWeaponGameObject);
            }
            else
            {
                var currentWeaponGameObject = Instantiate(new GameObject(), gameObject.transform);
                currentWeaponGameObject.SetActive(false);
                weaponGOs.Add(currentWeaponGameObject);
            }
        }
    }

    private void Update()
    {
        if (_inputManager == null) return;

        if (_inputManager.OpenWeaponSelectorAction.triggered)
        {
            _inputManager.ToggleInputOn(false);
            _inputManager.enabled = false;
            weaponSelectorObject.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void SelectWeapon()
    {
        foreach (var weapon in weaponGOs)
        {
            weapon.SetActive(false);
        }
        CancelWeaponSelect();
        Cursor.visible = false;

        if (weaponsUIList.value != 0)
        {
            _currentWeaponGameObject = weaponGOs[weaponsUIList.value];
            _currentWeapon = _currentWeaponGameObject.GetComponent<Weapon>();
            _currentWeaponGameObject.SetActive(true);
            _currentWeapon.SetInputManager(_inputManager);
        }
        else
        {
            _currentWeapon = weapons[weaponsUIList.value];
        }
    }

    public void CancelWeaponSelect()
    {
        weaponSelectorObject.SetActive(false);
        Cursor.visible = false;
        _inputManager.enabled = true;
        _inputManager.ToggleInputOn(true);
    }

    public void DisableWeaponSelectorObject()
    {
        weaponSelectorObject.SetActive(false);
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }
    public void SetInputManager(PlayerInputManager newInputManager)
    {
        _inputManager = newInputManager;
    }

    public void TurnWeaponsOff()
    {
        foreach (var weapon in weaponGOs)
        {
            weapon.SetActive(false);
        }
    }

}
