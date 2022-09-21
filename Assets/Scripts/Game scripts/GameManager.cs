using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    private PlayerInputManager _currentInputManager;
    private TurnTimer _turnTimer;

    [SerializeField]
    private MatchInfo _matchInfo;

    [SerializeField]
    private GameObject teamPrefab;
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private List<GameObject> teams;

    private List<List<GameObject>> _units;

    [SerializeField]
    private List<GameObject> serializedUnitsList;

    [SerializeField, Range(0, 3)]
    private int currentTurnIndex = 0, currentUnitIndex = 0;


    public int CurrentTurnIndex { get => currentTurnIndex; }
    public List<List<GameObject>> Units { get => _units; }


    private void Start()
    {
        _turnTimer = GetComponent<TurnTimer>();
        PrepareMatch();
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
            SetNonActiveUnitsKinematic(CurrentTurnIndex);
        }
    }

    private void FixedUpdate()
    {
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

            currentUnitIndex = 0;
            _turnTimer.ResetTurnTimer();
            DisableUnitsGUI();
        }
        SetNonActiveTeamsKinematic();

    }

    private void DisableUnitsGUI()
    {
        for (int j = 0; j < teams.Count; j++)
        {
            for (int i = 0; i < _units[j].Count; i++)
            {
                _units[j][i].GetComponentInChildren<WeaponSelector>().DisableWeaponSelectorObject();
            }
        }
    }

    private void SetNonActiveTeamsKinematic()
    {
        for (int j = 0; j < teams.Count; j++)
        {
            SetNonActiveUnitsKinematic(j);
        }
    }

    private void SetNonActiveUnitsKinematic(int teamIndex)
    {
        for (int i = 0; i < _units[teamIndex].Count; i++)
        {
            var indexPlayerController = _units[teamIndex][i].GetComponent<PlayerController>();

            if (teamIndex == currentTurnIndex && i == currentUnitIndex)
            {
                _units[teamIndex][i].GetComponent<Rigidbody>().isKinematic = false;
            }
            else if (indexPlayerController.IsGrounded && indexPlayerController.FramesGrounded > 50)
            {
                indexPlayerController.GetComponent<Rigidbody>().isKinematic = true;
            }
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

    private void PrepareMatch()
    {
        _matchInfo = MatchInfo.Instance;
        if (_matchInfo != null)
        {
            _turnTimer.SetStoredTimerDuration(_matchInfo.TurnTimerLength);

            for (int i = 0; i < _matchInfo.AmountOfPlayers; i++)
            {
                var currentTeam = Instantiate(teamPrefab, transform.GetChild(0));
                for (int j = 0; j < _matchInfo.AmountOfUnits; j++)
                {
                    var currentUnit = Instantiate(playerPrefab, currentTeam.transform);
                    currentUnit.transform.localPosition += new Vector3(j * 3f, 0, i * 3f);
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                var currentTeam = Instantiate(teamPrefab, transform.GetChild(0));
                for (int j = 0; j < 2; j++)
                {
                    var currentUnit = Instantiate(playerPrefab, currentTeam.transform);
                    currentUnit.transform.localPosition += new Vector3(j * 3f, 0, i * 3f);
                }
            }
        }

        _turnTimer.ResetTurnTimer();

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

            // Sets each units team index and unit index.
            for (int i = 0; i < _units[j].Count; i++)
            {
                var currentUnitInformation = _units[j][i].GetComponent<UnitInformation>();
                currentUnitInformation.SetIndexes(j, i);
            }

            // Gets all units from nested list so that they can be shown in editor. Will be removed later as it has no other purpose.
            foreach (GameObject gameObject in _units[j])
            {
                serializedUnitsList.Add(gameObject);
            }
        }

    }

}
