using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{
    
    [Header("Match Info")]
    [SerializeField]
    private int amountOfPlayers;
    [SerializeField]
    private int amountOfUnits;
    [SerializeField]
    private float turnTimerLength;
    
    public int AmountOfPlayers { get => amountOfPlayers; }
    public int AmountOfUnits { get => amountOfUnits; }
    public float TurnTimerLength { get => turnTimerLength; }

    [Header("Post Match Info")] 
    [SerializeField]
    private bool wasWin;
    [SerializeField]
    private List<GameObject> winningTeamUnits;
    
    public List<GameObject> WinningTeamUnits { get => winningTeamUnits; }
    
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
        amountOfPlayers = playerAmount;
        amountOfUnits = unitAmount;
        turnTimerLength = turnTimer;
    }

    public void SetPostMatchInfo([CanBeNull] Team winningTeam, int teamIndex)
    {
        if (teamIndex == -1)
        {
            wasWin = false;
        }
        else
        {
            wasWin = true;
            foreach (var unit in winningTeam.Units)
            {
                winningTeamUnits.Add(unit);
            }
        }
    }
}
