using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    [field: SerializeField]
    public GameObject[] Units { get; }
    [field: SerializeField]
    public List<GameObject> AliveUnits { get; }
}
