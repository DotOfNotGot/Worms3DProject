using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInformation : MonoBehaviour
{

    [SerializeField, Range(0, 3)]
    private int teamIndex;

    [SerializeField, Range(0, 3)]
    private int unitIndex;

    [SerializeField]
    private int hp = 100;

    public int HP { get => hp; }

    private Material _unitMaterial;

    // Start is called before the first frame update
    void Start()
    {
        _unitMaterial = GetComponent<Renderer>().material;
        SetUnitColor();
    }

    // Sets the indexes of the unit.
    public void SetIndexes(int newTeamIndex, int newUnitIndex)
    {
        teamIndex = newTeamIndex;
        unitIndex = newUnitIndex;
    }

    public void Damage(float damageAmount)
    {
        hp -= (int)damageAmount;
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
