using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{
    [SerializeField]
    private int amountOfPlayers;
    [SerializeField]
    private int amountOfUnits;
    [SerializeField]
    private float turnTimerLength;

    
    public int AmountOfPlayers { get => amountOfPlayers; }
    public int AmountOfUnits { get => amountOfUnits; }
    public float TurnTimerLength { get => turnTimerLength; }
    
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
    
}
