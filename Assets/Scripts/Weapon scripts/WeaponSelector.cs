using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponSelector : MonoBehaviour
{
    private PlayerInputManager _inputManager;

    private AudioSource _audioSource;


    [SerializeField]
    private TMP_Dropdown _weaponsUIList;

    [SerializeField]
    private GameObject _weaponSelectorObject;

    private UnitsInputSetter _unitsInputSetter;

    [SerializeField]
    private List<WeaponBase> _weapons;
    [SerializeField]
    private List<GameObject> _weaponGOs;


    private WeaponBase _currentWeapon;
    private GameObject _currentWeaponGameObject;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _unitsInputSetter = GetComponentInParent<UnitsInputSetter>();
        // Instantiates weapons list objects in every unit.
        foreach (var weapon in _weapons)
        {
            if (weapon != null)
            {
                var currentWeaponGameObject = Instantiate(weapon.gameObject, gameObject.transform);
                _weaponGOs.Add(currentWeaponGameObject);
            }
            else
            {
                var currentWeaponGameObject = new GameObject();
                currentWeaponGameObject.transform.SetParent(gameObject.transform);
                currentWeaponGameObject.transform.localPosition = Vector3.zero;
                _weaponGOs.Add(currentWeaponGameObject);
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
            _weaponSelectorObject.SetActive(true);
            Cursor.visible = true;
        }
    }


    public void PlayWeaponSound(AudioClip weaponSound)
    {
        _audioSource.clip = weaponSound;
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    public void SelectWeapon()
    {
        foreach (var weapon in _weaponGOs)
        {
            weapon.SetActive(false);
        }
        CancelWeaponSelect();
        Cursor.visible = false;

        if (_weaponsUIList.value != 0)
        {
            _currentWeaponGameObject = _weaponGOs[_weaponsUIList.value];
            _currentWeapon = _currentWeaponGameObject.GetComponent<WeaponBase>();
            _currentWeaponGameObject.SetActive(true);
        }
        else
        {
            _currentWeapon = _weapons[_weaponsUIList.value];
        }
        gameObject.GetComponentInParent<UnitController>().SetCurrentWeapon(_currentWeapon);
    }

    public void CancelWeaponSelect()
    {
        _weaponSelectorObject.SetActive(false);
        Cursor.visible = false;
        _unitsInputSetter.EnableUnitInput(_inputManager);
    }

    public void DisableWeaponSelectorObject()
    {
        _weaponSelectorObject.SetActive(false);
    }

    public void SetCurrentWeaponByIndex(int newWeaponIndex)
    {
        if (_weaponGOs.Count != 0)
        {
            _currentWeapon = _weaponGOs[0].GetComponent<WeaponBase>();
        }
    }
    
    public WeaponBase GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public void SetInputManager(PlayerInputManager newInputManager)
    {
        _inputManager = newInputManager;
    }

    public void TurnWeaponsOff()
    {
        foreach (var weapon in _weaponGOs)
        {
            weapon.SetActive(false);
        }
    }

}
