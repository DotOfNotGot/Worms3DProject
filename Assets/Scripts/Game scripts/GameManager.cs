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
    private CameraHandler sceneCamera;

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
            HandleUnitChange();
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
            HandleTurnChange();
        }
        SetNonActiveTeamsKinematic();
    }
    
    private void HandleTurnChange()
    {
        _currentInputManager = GetCurrentInputManager();
        sceneCamera.SetCameraInputManager(_currentInputManager);
        sceneCamera.SetCameraTarget(_units[currentTurnIndex][currentUnitIndex].transform);
        SetUnitsInput();
        SetUnitsWeapons();
        DisableUnitsGUI();
        _turnTimer.ResetTurnTimer();
    }

    private void HandleUnitChange()
    {
        if (currentUnitIndex + 1 != _units[CurrentTurnIndex].Count)
        {
            currentUnitIndex += 1;
        }
        else
        {
            currentUnitIndex = 0;
        }

        _currentInputManager = GetCurrentInputManager();
        sceneCamera.SetCameraInputManager(_currentInputManager);
        sceneCamera.SetCameraTarget(_units[currentTurnIndex][currentUnitIndex].transform);
        SetUnitsInput();
        SetNonActiveUnitsKinematic(CurrentTurnIndex);
    }
    private void SetUnitsWeapons()
    {
        for (int j = 0; j < teams.Count; j++)
        {
            for (int i = 0; i < _units[j].Count; i++)
            {
                WeaponSelector unitWeaponSelector = _units[j][i].GetComponentInChildren<WeaponSelector>();
                _units[j][i].GetComponentInChildren<PlayerController>().SetCurrentWeapon(unitWeaponSelector.GetCurrentWeapon());

                if (j != currentTurnIndex || i != currentUnitIndex)
                {
                    unitWeaponSelector.TurnWeaponsOff();
                }
            }
        }
    }

    private void SetUnitsInput()
    {
        for (int j = 0; j < teams.Count; j++)
        {
            foreach (var unit in _units[j])
            {
                unit.GetComponent<UnitsInputSetter>().DisableUnitInput(unit.GetComponent<PlayerInputManager>());
            }
        }
        _units[currentTurnIndex][currentUnitIndex].GetComponent<UnitsInputSetter>().EnableUnitInput(GetCurrentInputManager());
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
        return _currentInputManager = _units[currentTurnIndex][currentUnitIndex].GetComponent<PlayerInputManager>();
    }

    private void PrepareMatch()
    {
        _matchInfo = MatchInfo.Instance;
        // Sets info of match, instantiates the correct number of teams and units per team.
        if (_matchInfo != null)
        {
            SetInfo(_matchInfo.TurnTimerLength, _matchInfo.AmountOfPlayers, _matchInfo.AmountOfUnits);
        }
        else
        {
            SetInfo(_turnTimer.DurationInSeconds, 4, 2);
        }

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
                _units[j][i].GetComponent<UnitInformation>().SetIndexes(j, i);
            }

            // Gets all units from nested list so that they can be shown in editor. Will be removed later as it has no other purpose.
            foreach (GameObject gameObject in _units[j])
            {
                serializedUnitsList.Add(gameObject);
            }
        }

        HandleTurnChange();
    }
    private void SetInfo(float turnTimerLength, int amountOfPlayers, int amountOfUnits)
    {
        _turnTimer.SetStoredTimerDuration(turnTimerLength);

        for (int j = 0; j < amountOfPlayers; j++)
        {
            var currentTeam = Instantiate(teamPrefab, transform.GetChild(0));
            for (int i = 0; i < amountOfUnits; i++)
            {
                var currentUnit = Instantiate(playerPrefab, currentTeam.transform);
                currentUnit.transform.localPosition += new Vector3(i * 3f, 0, j * 3f);
            }
        }
    }
}
