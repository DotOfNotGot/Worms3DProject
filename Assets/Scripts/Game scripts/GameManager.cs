using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

public class GameManager : MonoBehaviour
{
    private PlayerInputManager _currentInputManager;
    private TurnTimer _turnTimer;

    private enum GameStates
    {
        TurnStart,
        CurrentlyInTurn,
        TurnEnd,
        BetweenTurns,
        GameEnd
    }

    [SerializeField]
    private GameStates currentState = GameStates.TurnStart;

    [SerializeField] private CameraHandler sceneCamera;

    private MatchInfo _matchInfo;

    [SerializeField] private GameObject teamPrefab;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject[] teams;
    [SerializeField] private List<GameObject> aliveTeams;



    [SerializeField] private List<GameObject> deadUnits;

    private List<List<GameObject>> _units;
    private List<List<GameObject>> _aliveUnits;

    [SerializeField, Range(0, 3)] private int currentTurnIndex = 0, currentUnitIndex = 0;

    private bool _turnTimerReachedZeroThisFrame = true;

    private bool _deadUnitTargetInProgress = false;

    private bool _shouldSwitchState = true;

    private bool _gameEnded = false;

    public int CurrentTurnIndex { get => currentTurnIndex; }

    public List<List<GameObject>> AliveUnits { get => _aliveUnits; }


    private void Start()
    {
        _turnTimer = GetComponent<TurnTimer>();
        deadUnits = new List<GameObject>();
        PrepareMatch();
    }

    private void FixedUpdate()
    {


        switch (currentState)
        {
            case GameStates.TurnStart:

                HandleTurnStart();

                currentState = GameStates.CurrentlyInTurn;

                break;
            case GameStates.CurrentlyInTurn:

                if (_turnTimer.DurationInSeconds <= 0f)
                {
                    currentState = GameStates.TurnEnd;
                    _turnTimerReachedZeroThisFrame = false;
                }

                break;
            case GameStates.TurnEnd:


                HandleTurnEnd();

                currentState = GameStates.BetweenTurns;
                break;
            case GameStates.BetweenTurns:

                HandleBetweenTurns();

                if (_gameEnded)
                {
                    currentState = GameStates.GameEnd;
                }
                else if (_shouldSwitchState)
                {
                    currentState = GameStates.TurnStart;
                }
                break;
            case GameStates.GameEnd:

                if (aliveTeams.Count == 1)
                {
                    Debug.Log(aliveTeams[0] + " won!", aliveTeams[0]);
                }
                else
                {
                    Debug.Log("Draw");
                }

                break;
        }

        SetNonActiveTeamsKinematic();
    }


    private void HandleTurnStart()
    {
        _turnTimerReachedZeroThisFrame = true;
        EnableCurrentUnitInput();
        _currentInputManager = GetCurrentInputManager();
        sceneCamera.SetCameraTarget(_aliveUnits[currentTurnIndex][currentUnitIndex].transform);
        sceneCamera.SetCameraInputManager(_currentInputManager);
    }
    private void HandleTurnEnd()
    {
        CheckForDeadUnits();

        DisableUnitsGUI();
    }

    private void HandleBetweenTurns()
    {
        _shouldSwitchState = HandleDeadUnits();
        SetThingsForRound();
        if (!_shouldSwitchState) return;

        if (aliveTeams.Count <= 1)
        {
            _gameEnded = true;
            return;
        }

        if (currentTurnIndex + 1 != aliveTeams.Count)
        {
            currentTurnIndex += 1;
        }
        else
        {
            currentTurnIndex = 0;
            if (currentUnitIndex + 1 != _aliveUnits[currentTurnIndex].Count)
            {
                currentUnitIndex += 1;
            }
            else
            {
                currentUnitIndex = 0;
            }
        }
    }

    private void SetThingsForRound()
    {
        DisableUnitsInput();
        SetUnitsWeapons();
        _turnTimer.ResetTurnTimer();
    }

    private void CheckForDeadUnits()
    {
        for (int i = 0; i < aliveTeams.Count; i++)
        {
            foreach (var unit in _aliveUnits[i])
            {
                if (unit.GetComponent<UnitInformation>().HP <= 0)
                {
                    deadUnits.Add(unit);
                }
            }
        }
    }

    private bool HandleDeadUnits()
    {

        if (deadUnits.Count == 0)
        {
            return true;
        }
        else
        {
            if (!_deadUnitTargetInProgress)
            {
                StartCoroutine(HandleDeadUnitsTarget());
            }
            return false;
        }

    }

    private IEnumerator HandleDeadUnitsTarget()
    {
        _deadUnitTargetInProgress = true;
        sceneCamera.SetCameraTarget(deadUnits[0].transform);
        yield return new WaitForSecondsRealtime(2.5f);
        for (int i = 0; i < _aliveUnits.Count; i++)
        {
            _aliveUnits[i].Remove(deadUnits[0]);
            if (_aliveUnits[i].Count == 0)
            {
                _aliveUnits.Remove(_aliveUnits[i]);
                aliveTeams.Remove(aliveTeams[i]);
                i = 0;
            }
        }
        
        deadUnits[0].SetActive(false);
        yield return new WaitForSecondsRealtime(2.5f);
        deadUnits.Remove(deadUnits[0]);
        _deadUnitTargetInProgress = false;
    }
    private void SetUnitsWeapons()
    {
        for (int j = 0; j < aliveTeams.Count; j++)
        {
            for (int i = 0; i < _aliveUnits[j].Count; i++)
            {
                WeaponSelector unitWeaponSelector = _aliveUnits[j][i].GetComponentInChildren<WeaponSelector>();
                unitWeaponSelector.SetCurrentWeaponByIndex(0);
                _aliveUnits[j][i].GetComponentInChildren<PlayerController>()
                    .SetCurrentWeapon(unitWeaponSelector.GetCurrentWeapon());

                unitWeaponSelector.TurnWeaponsOff();
            }
        }
    }

    private void DisableUnitsInput()
    {
        for (int j = 0; j < aliveTeams.Count; j++)
        {
            foreach (var unit in _aliveUnits[j])
            {
                unit.GetComponent<UnitsInputSetter>().DisableUnitInput(unit.GetComponent<PlayerInputManager>());
            }
        }

    }

    private void EnableCurrentUnitInput()
    {
        _aliveUnits[currentTurnIndex][currentUnitIndex].GetComponent<UnitsInputSetter>().EnableUnitInput(GetCurrentInputManager());
    }

    private void DisableUnitsGUI()
    {
        for (int j = 0; j < aliveTeams.Count; j++)
        {
            for (int i = 0; i < _aliveUnits[j].Count; i++)
            {
                _aliveUnits[j][i].GetComponentInChildren<WeaponSelector>().DisableWeaponSelectorObject();
            }
        }
    }

    private void SetNonActiveTeamsKinematic()
    {
        for (int j = 0; j < aliveTeams.Count; j++)
        {
            SetNonActiveUnitsKinematic(j);
        }
    }

    private void SetNonActiveUnitsKinematic(int teamIndex)
    {
        for (int i = 0; i < _aliveUnits[teamIndex].Count; i++)
        {
            var indexPlayerController = _aliveUnits[teamIndex][i].GetComponent<PlayerController>();

            if (teamIndex == currentTurnIndex && i == currentUnitIndex)
            {
                _aliveUnits[teamIndex][i].GetComponent<Rigidbody>().isKinematic = false;
            }
            else if (indexPlayerController.IsGrounded && indexPlayerController.FramesGrounded > 50)
            {
                indexPlayerController.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public Transform GetCameraFollowTarget()
    {
        return _aliveUnits[currentTurnIndex][currentUnitIndex].transform;
    }

    public PlayerInputManager GetCurrentInputManager()
    {
        return _currentInputManager = _aliveUnits[currentTurnIndex][currentUnitIndex].GetComponent<PlayerInputManager>();
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

        _units = new List<List<GameObject>>();

        // Gets a list of all teams in scene.
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            teams[i] = transform.GetChild(0).GetChild(i).gameObject;
        }

        aliveTeams = new List<GameObject>(teams);

        // List of lists of units, divides the units into lists based on teams to make it easier to think about.
        for (int j = 0; j < teams.Length; j++)
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
        }

        _aliveUnits = _units;

        SetThingsForRound();
        HandleTurnStart();
        HandleTurnEnd();
    }

    private void SetInfo(float turnTimerLength, int amountOfPlayers, int amountOfUnits)
    {
        _turnTimer.SetStoredTimerDuration(turnTimerLength);

        teams = new GameObject[amountOfPlayers];

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