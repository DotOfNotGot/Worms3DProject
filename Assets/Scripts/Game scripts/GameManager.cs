using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    private PlayerInputManager _currentInputManager;
    private TurnTimer _turnTimer;

    [SerializeField]
    private List<GameObject> teams;

    private List<List<GameObject>> _units;

    [SerializeField]
    private List<GameObject> serializedUnitsList;

    [SerializeField, Range(0,3)]
    private int currentTurnIndex = 0, currentUnitIndex = 0;

    public int CurrentTurnIndex { get => currentTurnIndex; }
    public List<List<GameObject>> Units { get => _units; }



    private void Awake()
    {
        SetTeams();
        _turnTimer = GetComponent<TurnTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentInputManager.SwitchUnitAction.triggered)
        {
            if (currentUnitIndex + 1 != _units[CurrentTurnIndex].Count)
            {
                currentUnitIndex += 1;
            }
            else
            {
                currentUnitIndex = 0;
            }
        }

        if (_turnTimer.DurationInSeconds <= 0f)
        {
            if (currentTurnIndex + 1 != teams.Count)
            {
                currentTurnIndex += 1;
            }
            else
            {
                currentTurnIndex = 0;
            }
            _turnTimer.ResetTurnTimer();
        }

    }

    public Transform GetCameraFollowTarget()
    {
        return _units[currentTurnIndex][currentUnitIndex].transform;
    }

    public PlayerInputManager GetCurrentInputManager()
    {
        for (int j = 0; j < teams.Count; j++)
        {
            for (int i = 0; i < _units[j].Count; i++)
            {
                if (j == currentTurnIndex && i == currentUnitIndex)
                {
                    _units[j][i].GetComponent<PlayerInput>().enabled = true;
                }
                else
                {
                    _units[j][i].GetComponent<PlayerInput>().enabled = false;
                }
            }
        }
        _currentInputManager = _units[currentTurnIndex][currentUnitIndex].GetComponent<PlayerInputManager>();
        return _units[currentTurnIndex][currentUnitIndex].GetComponent<PlayerInputManager>();
    }

    private void SetTeams()
    {
        teams = new List<GameObject>();
        _units = new List<List<GameObject>>();
        serializedUnitsList = new List<GameObject>();

        // Gets a list of all teams in scene.
        foreach (Transform transform in transform.GetChild(0).transform)
        {
            teams.Add(transform.gameObject);
        }

        // List of lists of units, divides the units into lists based on teams to make it easier to think about.
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
