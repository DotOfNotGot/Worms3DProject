using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

public class GameManager : MonoBehaviour
{
    private PlayerInputManager _currentInputManager;
    private TurnTimer _turnTimer;

    private int _roundCounter = 0;
    [SerializeField]
    private GameObject _waterPlane;

    private enum GameStates
    {
        TurnStart,
        CurrentlyInTurn,
        TurnEnd,
        BetweenTurns,
        GameEnd
    }

    [SerializeField] private GameStates _currentState = GameStates.TurnStart;

    [SerializeField] private CameraHandler _sceneCamera;

    private MatchInfo _matchInfo;

    [SerializeField] private GameObject _teamPrefab;
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Team[] _teams;
    [SerializeField] private List<Team> _aliveTeams;

    [SerializeField] private List<GameObject> _deadUnits;
    [SerializeField] private List<GameObject> _damagedUnits;

    [SerializeField, Range(0, 3)] private int _currentTurnIndex = 0, _currentUnitIndex = 0;
    private int _unitIndexToUse = 0;

    private GameObject _currentTeamCurrentUnitGo;

    [SerializeField]
    private Transform[] _teamSpawnPositions;

    private bool _deadUnitTargetInProgress;

    private bool _shouldSwitchState = true;

    private bool _hasSetTurnTimer = false;

    private bool _gameEnded;
    private void Start()
    {
        _turnTimer = GetComponent<TurnTimer>();
        _deadUnits = new List<GameObject>();
        PrepareMatch();
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case GameStates.TurnStart:

                HandleTurnStart();

                _currentState = GameStates.CurrentlyInTurn;

                break;

            case GameStates.CurrentlyInTurn:

                HandleCurrentlyInTurn();
                if (_turnTimer.DurationInSeconds <= 0f)
                {
                    _currentState = GameStates.TurnEnd;
                }

                break;

            case GameStates.TurnEnd:

                HandleTurnEnd();
                if (_shouldSwitchState)
                    _currentState = GameStates.BetweenTurns;
                break;

            case GameStates.BetweenTurns:

                HandleBetweenTurns();

                if (_gameEnded)
                {
                    _currentState = GameStates.GameEnd;
                    return;
                }
                else if (_shouldSwitchState)
                {
                    _currentState = GameStates.TurnStart;
                }

                break;

            case GameStates.GameEnd:

                if (_aliveTeams.Count == 1)
                {
                    _matchInfo.SetPostMatchInfo(_aliveTeams[0],
                        _aliveTeams[0].AliveUnits[0].GetComponent<UnitInformation>().TeamIndex);
                }
                else
                {
                    _matchInfo.SetPostMatchInfo(null, -1);
                }

                SceneManager.LoadScene(2);

                return;
        }

        SetNonActiveTeamsKinematic();
    }


    private void HandleTurnStart()
    {
        EnableCurrentUnitInput();
        _currentInputManager = GetCurrentInputManager();
        _sceneCamera.SetCameraTarget(_currentTeamCurrentUnitGo.transform);
        _sceneCamera.SetCameraInputManager(_currentInputManager);
    }

    private void HandleCurrentlyInTurn()
    {
        CheckForDamagedUnits();
        CheckForTurnAction();
    }

    private void CheckForDamagedUnits()
    {
        foreach (var team in _aliveTeams)
        {
            foreach (var unit in team.AliveUnits)
            {
                var shouldSkipUnit = false;
                var currentUnitInfo = unit.GetComponent<UnitInformation>();


                if (currentUnitInfo.StoredDamage == 0) continue;


                foreach (var damagedUnit in _damagedUnits.Where(damagedUnit => damagedUnit == unit))
                {
                    shouldSkipUnit = true;
                }

                if (!shouldSkipUnit)
                {
                    _damagedUnits.Add(unit);
                }
            }
        }
    }

    private void CheckForTurnAction()
    {
        var currentTurnActiveUnit = _currentTeamCurrentUnitGo;
        if ((currentTurnActiveUnit.GetComponent<UnitController>().HasShot || currentTurnActiveUnit.GetComponent<UnitInformation>().StoredDamage > 0) && !_hasSetTurnTimer)
        {
            _turnTimer.SetTurnTimer(5f);
            _hasSetTurnTimer = true;
            SetUnitsWeapons();
        }
    }

    private void HandleTurnEnd()
    {
        _hasSetTurnTimer = false;
        _currentTeamCurrentUnitGo.GetComponent<UnitController>().SetHasShot(false);
        DisableUnitsInput();
        SetUnitsWeapons();
        DisableUnitsGUI();
        CheckForDeadUnits();
        _shouldSwitchState = SetUnitsHp();
    }

    // Sets units hp according to damage. If it returns true it switches state to between turns.
    private bool SetUnitsHp()
    {
        if (_damagedUnits.Count == 0) return true;

        foreach (var unit in _damagedUnits)
        {
            var currentUnitInfo = unit.GetComponent<UnitInformation>();
            if (currentUnitInfo.StoredDamage == 0)
            {
                _damagedUnits.Remove(unit);
                break;
            }

            StartCoroutine(UnitTakeDamageOverTime(currentUnitInfo));
        }

        return false;
    }

    private IEnumerator UnitTakeDamageOverTime(UnitInformation currentUnitInfo)
    {
        yield return new WaitForSeconds(0.1f);
        currentUnitInfo.TakeDamage();
    }

    // Checks all damaged units for dead units and adds them to a list of dead units.
    private void CheckForDeadUnits()
    {
        foreach (var damagedUnit in _damagedUnits)
        {
            var currentUnitInfo = damagedUnit.GetComponent<UnitInformation>();
            if (currentUnitInfo.Hp > 0 || currentUnitInfo.IsDead) continue;

            currentUnitInfo.SetDead();
            _deadUnits.Add(damagedUnit);
        }
    }

    // Disables all units GUI at the end of turn.
    private void DisableUnitsGUI()
    {
        foreach (var team in _aliveTeams)
        {
            foreach (var unit in team.AliveUnits)
            {
                unit.GetComponentInChildren<WeaponSelector>().DisableWeaponSelectorObject();
            }
        }
    }

    private void HandleBetweenTurns()
    {
        _shouldSwitchState = HandleDeadUnits();
        if (!_shouldSwitchState) return;

        // Checks if a team has won or if there was a draw. Returns from method if game is over.
        if (_aliveTeams.Count <= 1)
        {
            _gameEnded = true;
            return;
        }

        HandleTurnAndUnitIndex();

        _turnTimer.ResetTurnTimer();
    }

    private bool HandleDeadUnits()
    {
        if (_deadUnits.Count == 0 && !_deadUnitTargetInProgress)
        {
            return true;
        }

        if (_deadUnits.Count != 0 && !_deadUnitTargetInProgress)
        {
            StartCoroutine(HandleDeadUnitsTarget());
        }

        return false;
    }

    private IEnumerator HandleDeadUnitsTarget()
    {
        _deadUnitTargetInProgress = true;
        _sceneCamera.SetCameraTarget(_deadUnits[0].transform);
        yield return new WaitForSecondsRealtime(2.5f);
        for (int i = 0; i < _aliveTeams.Count; i++)
        {
            _aliveTeams[i].RemoveDeadUnitFromList(_deadUnits[0]);

            if (_aliveTeams[i].AliveUnits.Count != 0) continue;

            _aliveTeams.Remove(_aliveTeams[i]);
            i = 0;
        }

        _deadUnits[0].SetActive(false);
        yield return new WaitForSecondsRealtime(2.5f);
        _deadUnits.Remove(_deadUnits[0]);
        _deadUnitTargetInProgress = false;
    }

    private void HandleTurnAndUnitIndex()
    {
        // Gets the biggest unit index from the team with most units.
        int biggestUnitIndex = GetBiggestUnitIndex();
        bool roundCounterIncreased = false;

        // Makes sure that unit index isn't bigger than what should be possible so that it doesn't go out of range
        if (biggestUnitIndex < _currentUnitIndex)
        {
            _currentUnitIndex = biggestUnitIndex;
        }

        // Checks if turn index + 1 is bigger than teams alive. If it is then go to next round and set the counterbool to true.
        if (_currentTurnIndex + 1 < _aliveTeams.Count)
        {
            _currentTurnIndex += 1;
        }
        else
        {
            _currentTurnIndex = 0;
            _roundCounter++;
            roundCounterIncreased = true;
            _waterPlane.transform.position = Vector3.Lerp(_waterPlane.transform.position,
                new Vector3(0, _waterPlane.transform.position.y + _roundCounter, 0),
                _roundCounter / 10);
        }

        // The current team active this turns biggest index.
        int currentTeamBiggestUnitIndex = _aliveTeams[_currentTurnIndex].AliveUnits.Count - 1;

        // Only change current unit index if roundCounter bool is true.
        if (_currentUnitIndex + 1 <= biggestUnitIndex && roundCounterIncreased)
        {
            _currentUnitIndex += 1;
        }
        else if(roundCounterIncreased)
        {
            _currentUnitIndex = 0;
        }

        // Checks if the current teams biggest index isn't smaller than currentunitindex.
        if (currentTeamBiggestUnitIndex < _currentUnitIndex)
        {
            _unitIndexToUse = currentTeamBiggestUnitIndex;
        }
        else
        {
            _unitIndexToUse = _currentUnitIndex;
        }

        SetCurrentUnitGo(_currentTurnIndex, _unitIndexToUse);
    }

    private void SetCurrentUnitGo(int teamIndex, int unitIndex)
    {
        _currentTeamCurrentUnitGo = _aliveTeams[teamIndex].AliveUnits[unitIndex];
        Debug.Log($"{teamIndex} {unitIndex} " + _currentTeamCurrentUnitGo, _currentTeamCurrentUnitGo);
    }


    private int GetBiggestUnitIndex()
    {
        var teamsBiggestUnitIndex = new List<int>();

        for (int i = 0; i < _aliveTeams.Count; i++)
        {
            teamsBiggestUnitIndex.Add(_aliveTeams[i].AliveUnits.Count - 1);
            teamsBiggestUnitIndex.Sort();

        }

        for (int i = 0; i < teamsBiggestUnitIndex.Count; i++)
        {
            if (teamsBiggestUnitIndex.Count != 1)
            {
                teamsBiggestUnitIndex.RemoveAt(0);
                i = 0;
            }
        }

        return teamsBiggestUnitIndex[0];

    }
    private void SetUnitsWeapons()
    {
        foreach (var team in _aliveTeams)
        {
            foreach (var unit in team.AliveUnits)
            {
                var unitWeaponSelector = unit.GetComponentInChildren<WeaponSelector>();

                unitWeaponSelector.SetCurrentWeaponByIndex(0);

                unit.GetComponentInChildren<UnitController>().SetCurrentWeapon(unitWeaponSelector.GetCurrentWeapon());

                unitWeaponSelector.TurnWeaponsOff();
            }
        }
    }

    private void DisableUnitsInput()
    {
        // Loops through all alive units and disables their input.
        foreach (var unit in _aliveTeams.SelectMany(team => team.AliveUnits))
        {
            unit.GetComponent<UnitsInputSetter>().DisableUnitInput(unit.GetComponent<PlayerInputManager>());
        }
    }

    private void EnableCurrentUnitInput()
    {
        // Enables the active units input.
        _currentTeamCurrentUnitGo.GetComponent<UnitsInputSetter>()
            .EnableUnitInput(GetCurrentInputManager());
    }

    private void SetNonActiveTeamsKinematic()
    {
        var activeUnitRigidbody = _currentTeamCurrentUnitGo.GetComponent<Rigidbody>();

        // Sets the active unit rigidbody dynamic.
        activeUnitRigidbody.isKinematic = false;

        // Loops through all alive units and sets their rigidbodies kinematic.
        foreach (var team in _aliveTeams)
        {
            foreach (var unit in team.AliveUnits)
            {
                var currentUnitPlayerController = unit.GetComponent<UnitController>();
                var currentUnitRb = currentUnitPlayerController.GetComponent<Rigidbody>();

                if (currentUnitPlayerController.IsGrounded && currentUnitPlayerController.FramesGrounded > 50 && currentUnitRb != activeUnitRigidbody)
                {
                    currentUnitRb.isKinematic = true;
                }
            }
        }
    }

    private PlayerInputManager GetCurrentInputManager()
    {
        return _currentInputManager = _currentTeamCurrentUnitGo.GetComponent<PlayerInputManager>();
    }

    private void SetThingsForFirstRound()
    {
        SetCurrentUnitGo(0,0);
        DisableUnitsInput();
        SetUnitsWeapons();
        _turnTimer.ResetTurnTimer();
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

        _aliveTeams = new List<Team>(_teams);

        // Sets each units team index and unit index.
        for (int j = 0; j < _teams.Length; j++)
        {
            for (int i = 0; i < _teams[j].Units.Length; i++)
            {
                _teams[j].Units[i].GetComponent<UnitInformation>().SetIndexes(j, i);
            }
        }

        SetThingsForFirstRound();
        HandleTurnStart();
        HandleTurnEnd();
    }

    private void SetInfo(float turnTimerLength, int amountOfPlayers, int amountOfUnits)
    {
        _turnTimer.SetStoredTimerDuration(turnTimerLength);

        _teams = new Team[amountOfPlayers];
        for (int i = 0; i < amountOfPlayers; i++)
        {
            _teams[i] = new Team();
        }

        foreach (var team in _teams)
        {
            team.InitializeUnitsArray(amountOfUnits);
        }

        for (int j = 0; j < amountOfPlayers; j++)
        {
            var currentTeam = Instantiate(_teamPrefab, transform.GetChild(0));
            for (int i = 0; i < amountOfUnits; i++)
            {
                var currentUnit = Instantiate(_playerPrefab, currentTeam.transform);
                if (_teamSpawnPositions.Length > 0)
                {
                    currentUnit.transform.position = _teamSpawnPositions[j].position + new Vector3(i * 2f, 0, 0);
                }
                else
                {
                    currentUnit.transform.position += new Vector3(i * 3f, 5, j * 3f);
                }
                _teams[j].SetUnitsArray(currentUnit, i);
            }
            _teams[j].SetAliveUnitsList();
        }
    }
}