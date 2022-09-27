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

    private UnitsInputSetter _unitsInputSetter;

    [SerializeField]
    private List<Weapon> weapons;
    [SerializeField]
    private List<GameObject> weaponGOs;


    private Weapon _currentWeapon;
    private GameObject _currentWeaponGameObject;

    // Start is called before the first frame update
    void Start()
    {
        _unitsInputSetter = GetComponentInParent<UnitsInputSetter>();
        // Instantiates weapons list objects in every unit.
        foreach (var weapon in weapons)
        {
            if (weapon != null)
            {
                var currentWeaponGameObject = Instantiate(weapon.gameObject, gameObject.transform);
                weaponGOs.Add(currentWeaponGameObject);
            }
            else
            {
                var currentWeaponGameObject = new GameObject();
                currentWeaponGameObject.transform.SetParent(gameObject.transform);
                currentWeaponGameObject.transform.localPosition = Vector3.zero;
                weaponGOs.Add(currentWeaponGameObject);
            }
        }
        TurnWeaponsOff();
    }

    private void Update()
    {
        if (_inputManager == null) return;

        if (_inputManager.OpenWeaponSelectorAction.triggered)
        {
            _unitsInputSetter.DisableUnitInput(_inputManager);
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
        }
        else
        {
            _currentWeapon = weapons[weaponsUIList.value];
        }
        gameObject.GetComponentInParent<PlayerController>().SetCurrentWeapon(_currentWeapon);
    }

    public void CancelWeaponSelect()
    {
        weaponSelectorObject.SetActive(false);
        Cursor.visible = false;
        _unitsInputSetter.EnableUnitInput(_inputManager);
    }

    public void DisableWeaponSelectorObject()
    {
        weaponSelectorObject.SetActive(false);
    }

    public void SetCurrentWeaponByIndex(int newWeaponIndex)
    {
        if (weaponGOs.Count != 0)
        {
            _currentWeapon = weaponGOs[0].GetComponent<Weapon>();
        }
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
