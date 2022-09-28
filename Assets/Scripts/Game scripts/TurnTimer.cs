using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TurnTimer : MonoBehaviour
{
    [SerializeField]
    private float durationInSeconds = 90f;

    private float _storedStartDuration;

    public float DurationInSeconds { get => durationInSeconds; }

    [SerializeField]
    private TextMeshProUGUI timerElement;

    private void Awake()
    {
        SetStoredTimerDuration(durationInSeconds);
    }
    private void Start()
    {
       UpdateTimerDisplay();
    }

    private void Update()
    {
        if (durationInSeconds > 0)
        {
            durationInSeconds -= Time.deltaTime;
        }
        else
        {
            durationInSeconds = 0;
        }
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        var timer = (int)durationInSeconds;
        timerElement.text = timer.ToString();
    }

    public void ResetTurnTimer()
    {
        durationInSeconds = _storedStartDuration;
    }

    public void SetTurnTimer(float newTime)
    {
        durationInSeconds = newTime;
    }

    public void SetStoredTimerDuration(float timerStartDuration)
    {
        _storedStartDuration = timerStartDuration;
    }

}
