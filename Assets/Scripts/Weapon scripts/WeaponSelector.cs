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
            var currentWeaponGameObject = Instantiate(weapon.gameObject, gameObject.transform);
            currentWeaponGameObject.SetActive(false);
            weaponGOs.Add(currentWeaponGameObject);
        }
        _currentWeapon = weaponGOs[0].GetComponent<Weapon>();
    }

    private void Update()
    {
        if (_inputManager == null) return;

        if (_inputManager.OpenWeaponSelectorAction.triggered)
        {
            _inputManager.ToggleInputOn(false);
            _currentWeapon.DisableInputManager();
            _inputManager.enabled = false;
            if (weaponSelectorObject.activeSelf)
            {
                _currentWeapon.EnableInputManager();
                _inputManager.enabled = true;
                CancelWeaponSelect();
            }
            else
            {
                weaponSelectorObject.SetActive(true);
                Cursor.visible = true;
            }
        }
    }

    public void SelectWeapon()
    {
        foreach (var weapon in weaponGOs)
        {
            weapon.SetActive(false);
        }
        _currentWeapon = weaponGOs[weaponsUIList.value].GetComponent<Weapon>();
        _currentWeaponGameObject = weaponGOs[weaponsUIList.value];
        _currentWeaponGameObject.SetActive(true);
        CancelWeaponSelect();
        _currentWeapon.SetInputManager(_inputManager);
        Cursor.visible = false;
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
}
