using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInformation : MonoBehaviour
{

    [SerializeField, Range(0, 3)]
    private int _teamIndex;

    [SerializeField, Range(0, 3)]
    private int _unitIndex;

    [SerializeField, Range(0, 100)]
    private int _storedDamage = 0;

    [SerializeField]
    private int _hp = 100;

    private int _storedHp = 0;

    public int TeamIndex { get => _teamIndex; }
    public int StoredDamage { get => _storedDamage; }


    public int Hp { get => _hp; }

    private Material _unitMaterial;

    private UnitUIHandler _uiHandler;

    private bool _isDead = false;

    public bool IsDead { get => _isDead; }


    // Start is called before the first frame update
    void Start()
    {
        _unitMaterial = GetComponent<Renderer>().materials[0];
        _uiHandler = GetComponentInChildren<UnitUIHandler>();
        SetUnitColor();
    }

    // Sets the indexes of the unit.
    public void SetIndexes(int newTeamIndex, int newUnitIndex)
    {
        _teamIndex = newTeamIndex;
        _unitIndex = newUnitIndex;
    }

    public void StoreDamage(float damageAmount)
    {
        _storedDamage += (int)damageAmount;
    }

    public void TakeDamage()
    {
        if (_storedDamage == 0) return;

        if (_storedHp == 0)
        {
            _storedHp = _hp;
        }

        if (_hp > _storedHp - _storedDamage && _hp >= 0)
        {
            _hp--;
            _uiHandler.SetPlayerInfoDisplay(_hp);
            return;
        }

        if (_storedHp - _storedDamage <= 0)
        {
            _hp = -1;
        }
        else
        {
            _hp = _storedHp - _storedDamage;
        }
        _storedHp = _hp;
        _storedDamage = 0;
    }

    public void SetDead()
    {
        _isDead = true;
    }

    public void Heal(int healAmount)
    {
        _hp += healAmount;
    }

    // Uses team index to set the color of the unit.
    // At some point going to make it change something visually as well as color because colorblind people exist.
    private void SetUnitColor()
    {
        var color = Color.white;
        switch (_teamIndex)
        {
            case 0:
                color = new Color32(201, 54, 64, 255);
                break;
            case 1:
                color = new Color32(66, 135, 245, 255);
                break;
            case 2:
                color = new Color32(153, 229, 80, 255);
                break;
            case 3:
                color = new Color32(241, 247, 52, 255);
                break;
        }
        _unitMaterial.color = color;
    }

}
