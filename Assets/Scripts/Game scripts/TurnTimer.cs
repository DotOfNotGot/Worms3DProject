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

    // Start is called before the first frame update
    void Start()
    {
       UpdateTimerDisplay();
    }

    // Update is called once per frame
    void Update()
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
        timerElement.text = durationInSeconds.ToString();
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
