using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInformation : MonoBehaviour
{

    [SerializeField, Range(0, 3)]
    private int teamIndex;

    [SerializeField, Range(0, 3)]
    private int unitIndex;

    [SerializeField, Range(0, 100)]
    private int storedDamage = 0;

    public int StoredDamage { get => storedDamage; }

    [SerializeField]
    private int hp = 100;

    private int _storedHP = 0;
    public int StoredHP { get => _storedHP; }


    public int HP { get => hp; }

    private Material _unitMaterial;

    private UnitUIHandler _uiHandler;

    private bool _isDead = false;

    public bool IsDead { get => _isDead; }


    // Start is called before the first frame update
    void Start()
    {
        _unitMaterial = GetComponent<Renderer>().material;
        _uiHandler = GetComponentInChildren<UnitUIHandler>();
        SetUnitColor();
    }

    // Sets the indexes of the unit.
    public void SetIndexes(int newTeamIndex, int newUnitIndex)
    {
        teamIndex = newTeamIndex;
        unitIndex = newUnitIndex;
    }

    public void StoreDamage(float damageAmount)
    {
        storedDamage += (int)damageAmount;
    }

    public void TakeDamage()
    {
        // TODO: Make the hp tick down slower ish.

        if (storedDamage == 0) return;

        if (_storedHP == 0)
        {
            _storedHP = hp;
        }

        if (hp > _storedHP - storedDamage && hp >= 0)
        {
            hp--;
            _uiHandler.SetPlayerInfoDisplay(hp);
        }
        else
        {
            if (_storedHP - storedDamage <= 0)
            {
                hp = -1;
            }
            else
            {
                hp = _storedHP - storedDamage;
            }
            _storedHP = hp;
            storedDamage = 0;
        }
    }

    public void SetDead()
    {
        _isDead = true;
    }

    public void Heal(int healAmount)
    {
        hp += healAmount;
    }

    // Uses team index to set the color of the unit.
    // At some point going to make it change something visually as well as color because colorblind people exist.
    private void SetUnitColor()
    {
        var color = Color.white;
        switch (teamIndex)
        {
            case 0:
                color = Color.red;
                break;
            case 1:
                color = Color.blue;
                break;
            case 2:
                color = Color.green;
                break;
            case 3:
                color = Color.yellow;
                break;
        }
        _unitMaterial.color = color;
    }

}
