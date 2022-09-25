using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInformation : MonoBehaviour
{

    [SerializeField, Range(0, 3)]
    private int teamIndex;

    [SerializeField, Range(0, 3)]
    private int unitIndex;

    private int _storedDamage = 0;

    [SerializeField]
    private int hp = 100;

    public int HP { get => hp; }

    private Material _unitMaterial;

    private UnitUIHandler _uiHandler;


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
        _storedDamage += (int)damageAmount;
    }

    public void TakeDamage()
    {
        // TODO: Make the hp tick down slower ish.
        int storedHp = hp;
        while (hp != storedHp - _storedDamage)
        {
            if (_storedDamage == 0) break;
            StartCoroutine(DamageTicker());
        }
        _storedDamage = 0;
    }

    private IEnumerator DamageTicker()
    {
        hp--;
        _uiHandler.SetPlayerInfoDisplay(hp);
        yield return new WaitForSecondsRealtime(1f);
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
