using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> teams;

    private List<List<GameObject>> _units;

    [SerializeField]
    private List<GameObject> serializedUnitsList;

    private void Awake()
    {
        GetTeams();
    }

    // Start is called before the first frame update
    void Start()
    {

        

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GetTeams()
    {
        teams = new List<GameObject>();
        _units = new List<List<GameObject>>();
        serializedUnitsList = new List<GameObject>();

        // Gets a list of all teams in scene.
        foreach (Transform transform in transform.GetChild(0).transform)
        {
            teams.Add(transform.gameObject);
        }

        // List of lists of units, divides the units into teams to make it easier to think about.
        for (int j = 0; j < teams.Count; j++)
        {
            _units.Add(new List<GameObject>());
            for (int i = 0; i < teams[j].transform.childCount; i++)
            {
                _units[j].Add(teams[j].transform.GetChild(i).gameObject);
            }
        }

        // Sets each units team index and unit index.
        for (int j = 0; j < _units.Count; j++)
        {
            for (int i = 0; i < _units[j].Count; i++)
            {
                var currentUnitInformation = _units[j][i].GetComponent<UnitInformation>();
                currentUnitInformation.SetIndexes(j, i);
            }
        }

        // Gets all units from nested list so that they can be shown in editor. Will be removed later as it has no other purpose.
        for (int i = 0; i < _units.Count; i++)
        {
            foreach (GameObject gameObject in _units[i])
            {
                serializedUnitsList.Add(gameObject);
            }
        }
    }

}
