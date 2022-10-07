using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{
    
    [Header("Match Info")]
    [SerializeField]
    private int _amountOfPlayers;
    [SerializeField]
    private int _amountOfUnits;
    [SerializeField]
    private float _turnTimerLength;
    
    public int AmountOfPlayers { get => _amountOfPlayers; }
    public int AmountOfUnits { get => _amountOfUnits; }
    public float TurnTimerLength { get => _turnTimerLength; }

    [Header("Post Match Info")] 
    [SerializeField]
    private bool _wasWin;
    [SerializeField]
    private List<GameObject> _winningTeamUnits;
    
    public List<GameObject> WinningTeamUnits { get => _winningTeamUnits; }
    public bool WasWin { get => _wasWin; }
    
    public static MatchInfo Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetMatchInfo(int playerAmount, int unitAmount, float turnTimer)
    {
        _amountOfPlayers = playerAmount;
        _amountOfUnits = unitAmount;
        _turnTimerLength = turnTimer;
    }

    public void SetPostMatchInfo([CanBeNull] Team passedWinningTeam, int teamIndex)
    {
        if (teamIndex == -1)
        {
            _wasWin = false;
        }
        else
        {
            _wasWin = true;
            _winningTeamUnits = new List<GameObject>(passedWinningTeam.Units);
            foreach (var unit in _winningTeamUnits)
            {
                unit.transform.parent = null;
                DontDestroyOnLoad(unit.gameObject);
                DisableWinningUnitComponents(unit);
            }
        }
    }

    private void DisableWinningUnitComponents(GameObject unit)
    {
        unit.GetComponent<Rigidbody>().isKinematic = true;
        unit.GetComponent<UnitController>().enabled = false;
        unit.GetComponent<PlayerInputManager>().enabled = false;
        unit.GetComponentInChildren<UnitUIHandler>().enabled = false;
    }

}
