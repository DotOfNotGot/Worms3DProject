using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInformation : MonoBehaviour
{

    private int _teamIndex;

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
            _uiHandler.SetUnitInfoDisplay(_hp);
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

    // Uses team index to set the color of the unit.
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
                color = new Color32(200, 200, 80, 255);
                break;
            case 3:
                color = new Color32(55, 255, 52, 255);
                break;
                default: color = new Color32(255,255,255,255);
                break;
        }
        _unitMaterial.color = color;
    }

}
