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

    [SerializeField] private GameStates currentState = GameStates.TurnStart;

    [SerializeField] private CameraHandler sceneCamera;

    private MatchInfo _matchInfo;

    [SerializeField] private GameObject teamPrefab;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Team[] teams;
    [SerializeField] private List<Team> aliveTeams;

    [SerializeField] private List<GameObject> deadUnits;
    [SerializeField] private List<GameObject> damagedUnits;

    [SerializeField, Range(0, 3)] private int currentTurnIndex = 0, currentUnitIndex = 0;

    private bool _deadUnitTargetInProgress;

    private bool _shouldSwitchState = true;

    private bool _gameEnded;
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

                HandleCurrentlyInTurn();
                if (_turnTimer.DurationInSeconds <= 0f)
                {
                    currentState = GameStates.TurnEnd;
                }

                break;

            case GameStates.TurnEnd:

                HandleTurnEnd();
                if (_shouldSwitchState)
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
                    _matchInfo.SetPostMatchInfo(aliveTeams[0],
                        aliveTeams[0].AliveUnits[0].GetComponent<UnitInformation>().TeamIndex);
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
        sceneCamera.SetCameraTarget(aliveTeams[currentTurnIndex].AliveUnits[currentUnitIndex].transform);
        sceneCamera.SetCameraInputManager(_currentInputManager);
    }

    private void HandleCurrentlyInTurn()
    {
        CheckForDamagedUnits();
        CheckForTurnAction();
    }

    private void CheckForDamagedUnits()
    {
        foreach (var team in aliveTeams)
        {
            foreach (var unit in team.AliveUnits)
            {
                var shouldSkipUnit = false;
                var currentUnitInfo = unit.GetComponent<UnitInformation>();
                
                if (currentUnitInfo.StoredDamage == 0) continue;
                
                foreach (var damagedUnit in damagedUnits.Where(damagedUnit => damagedUnit == unit))
                { 
                    shouldSkipUnit = true;
                }

                if (!shouldSkipUnit)
                {
                    damagedUnits.Add(unit);
                }
            }
        }
    }

    private void CheckForTurnAction()
    {
        if (aliveTeams[currentTurnIndex].AliveUnits[currentUnitIndex].GetComponent<UnitController>().HasShot)
        {
            _turnTimer.SetTurnTimer(5f);
        }
    }

    private void HandleTurnEnd()
    {
        DisableUnitsInput();
        SetUnitsWeapons();
        DisableUnitsGUI();
        CheckForDeadUnits();
        _shouldSwitchState = SetUnitsHp();
    }

    private bool SetUnitsHp()
    {
        if (damagedUnits.Count == 0) return true;
        
        foreach (var unit in damagedUnits)
        {
            var currentUnitInfo = unit.GetComponent<UnitInformation>();
            if (currentUnitInfo.StoredDamage == 0)
            {
                damagedUnits.Remove(unit);
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

    private void CheckForDeadUnits()
    {
        foreach (var damagedUnit in damagedUnits)
        {
            var currentUnitInfo = damagedUnit.GetComponent<UnitInformation>();
            if (currentUnitInfo.Hp > 0 || currentUnitInfo.IsDead) continue;
            
            currentUnitInfo.SetDead();
            deadUnits.Add(damagedUnit);
        }
    }

    private void DisableUnitsGUI()
    {
        foreach (var team in aliveTeams)
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

        if (aliveTeams.Count <= 1)
        {
            _gameEnded = true;
            return;
        }

        HandleTurnAndUnitIndex();
        
        _turnTimer.ResetTurnTimer();
    }

    private bool HandleDeadUnits()
    {
        if (deadUnits.Count == 0 && !_deadUnitTargetInProgress)
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
        sceneCamera.SetCameraTarget(deadUnits[0].transform);
        yield return new WaitForSecondsRealtime(2.5f);
        for (int i = 0; i < aliveTeams.Count; i++)
        {
            aliveTeams[i].RemoveDeadUnitFromList(deadUnits[0]);
            
            if (aliveTeams[i].AliveUnits.Count != 0) continue;
            
            aliveTeams.Remove(aliveTeams[i]);
            i = 0;
        }

        deadUnits[0].SetActive(false);
        yield return new WaitForSecondsRealtime(2.5f);
        deadUnits.Remove(deadUnits[0]);
        _deadUnitTargetInProgress = false;
    }

    private void HandleTurnAndUnitIndex()
    {
        if (currentTurnIndex + 1 != aliveTeams.Count)
        {
            currentTurnIndex += 1;
        }
        else
        {
            currentTurnIndex = 0;
        }

        if (currentTurnIndex != 0 && aliveTeams[currentTurnIndex].AliveUnits.Count != 1) return;
            
        if (currentUnitIndex + 1 < aliveTeams[currentTurnIndex].AliveUnits.Count)
        { 
            currentUnitIndex += 1;
        }
        else
        { 
            currentUnitIndex = 0;
        }
    }

    private void SetUnitsWeapons()
    {
        foreach (var team in aliveTeams)
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
        foreach (var unit in aliveTeams.SelectMany(team => team.AliveUnits))
        {
            unit.GetComponent<UnitsInputSetter>().DisableUnitInput(unit.GetComponent<PlayerInputManager>());
        }
    }

    private void EnableCurrentUnitInput()
    {
        // Enables the active units input.
        aliveTeams[currentTurnIndex].AliveUnits[currentUnitIndex].GetComponent<UnitsInputSetter>()
            .EnableUnitInput(GetCurrentInputManager());
    }

    private void SetNonActiveTeamsKinematic()
    {
        var activeUnitRigidbody = aliveTeams[currentTurnIndex].AliveUnits[currentUnitIndex].GetComponent<Rigidbody>();
        
        // Sets the active unit rigidbody dynamic.
        activeUnitRigidbody.isKinematic = false;
        
        // Loops through all alive units and sets their rigidbodies kinematic.
        foreach (var team in aliveTeams)
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
            aliveTeams[currentTurnIndex].AliveUnits[currentUnitIndex].GetComponent<PlayerInputManager>();
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

        aliveTeams = new List<Team>(teams);
        
        // Sets each units team index and unit index.
        for (int j = 0; j < teams.Length; j++)
        {
            for (int i = 0; i < teams[j].Units.Length; i++)
            {
                teams[j].Units[i].GetComponent<UnitInformation>().SetIndexes(j, i);
            }
        }

        SetThingsForFirstRound();
        HandleTurnStart();
        HandleTurnEnd();
    }

    private void SetInfo(float turnTimerLength, int amountOfPlayers, int amountOfUnits)
    {
        _turnTimer.SetStoredTimerDuration(turnTimerLength);

        teams = new Team[amountOfPlayers];
        for (int i = 0; i < amountOfPlayers; i++)
        {
            teams[i] = new Team();
        }

        foreach (var team in teams)
        {
            team.InitializeUnitsArray(amountOfUnits);
        }

        for (int j = 0; j < amountOfPlayers; j++)
        {
            var currentTeam = Instantiate(teamPrefab, transform.GetChild(0));
            for (int i = 0; i < amountOfUnits; i++)
            {
                var currentUnit = Instantiate(playerPrefab, currentTeam.transform);
                currentUnit.transform.localPosition += new Vector3(i * 3f, 0, j * 3f);
                teams[j].SetUnitsArray(currentUnit.gameObject, i);
            }
            teams[j].SetAliveUnitsList();
        }
    }
}