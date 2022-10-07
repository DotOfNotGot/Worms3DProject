using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    [field: SerializeField]
    public GameObject[] Units { get; private set; }
    [field: SerializeField]
    public List<GameObject> AliveUnits { get; private set; }

    public void InitializeUnitsArray(int amountOfUnits)
    {
        Units = new GameObject[amountOfUnits];
    }
    
    public void SetUnitsArray(GameObject unitToAdd, int indexToAddAt)
    {
        Units[indexToAddAt] = unitToAdd;
    }
    
    public void SetAliveUnitsList()
    {
        AliveUnits = new List<GameObject>();
        foreach (var unit in Units)
        {
            AliveUnits.Add(unit);
        }
    }

    public void RemoveDeadUnitFromList(GameObject deadUnit)
    {
        AliveUnits.Remove(deadUnit);
    }
    
}
