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
        _sceneCamera.SetCameraTarget(_aliveTeams[_currentTurnIndex].AliveUnits[_currentUnitIndex].transform);
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
        if (_aliveTeams[_currentTurnIndex].AliveUnits[_currentUnitIndex].GetComponent<UnitController>().HasShot && !_hasSetTurnTimer)
        {
            _turnTimer.SetTurnTimer(5f);
            _hasSetTurnTimer = true;
            SetUnitsWeapons();
        }
    }

    private void HandleTurnEnd()
    {
        _hasSetTurnTimer = false;
        _aliveTeams[_currentTurnIndex].AliveUnits[_currentUnitIndex].GetComponent<UnitController>().SetHasShot(false);
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

        if (!_deadUnitTargetInProgress)
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
        if (_currentTurnIndex + 1 != _aliveTeams.Count)
        {
            _currentTurnIndex += 1;
        }
        else
        {
            _currentTurnIndex = 0;
        }

        if (_currentTurnIndex != 0) return;
            
        int storedUnitIndex = _currentUnitIndex;
        
        if (_currentUnitIndex + 1 < _aliveTeams[_currentTurnIndex].AliveUnits.Count)
        { 
            _currentUnitIndex += 1;
        }
        else
        { 
            _currentUnitIndex = 0;
        }
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
        _aliveTeams[_currentTurnIndex].AliveUnits[_currentUnitIndex].GetComponent<UnitsInputSetter>()
            .EnableUnitInput(GetCurrentInputManager());
    }

    private void SetNonActiveTeamsKinematic()
    {
        var activeUnitRigidbody = _aliveTeams[_currentTurnIndex].AliveUnits[_currentUnitIndex].GetComponent<Rigidbody>();
        
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
        return _currentInputManager =
            _aliveTeams[_currentTurnIndex].AliveUnits[_currentUnitIndex].GetComponent<PlayerInputManager>();
    }

    private void SetThingsForFirstRound()
    {
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
                currentUnit.transform.localPosition += new Vector3(i * 3f, 0, j * 3f);
                _teams[j].SetUnitsArray(currentUnit.gameObject, i);
            }
            _teams[j].SetAliveUnitsList();
        }
    }
}