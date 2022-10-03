using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] 
    private MatchInfo _matchInfo;
    
    [SerializeField]
    private TMP_Dropdown _teamsDropdown;

    [SerializeField] 
    private TMP_Dropdown _unitsDropdown;

    [SerializeField]
    private TextMeshProUGUI _timerUI;
    
    [SerializeField] 
    private int _amountOfTeams;
    [SerializeField] 
    private int _amountOfUnits;
    [SerializeField]
    private float _turnTimerLength = 90f;
    
    // Start is called before the first frame update
    void Start()
    {
        _matchInfo = MatchInfo.Instance;
        _timerUI.text = _turnTimerLength.ToString();
    }
    public void OnPlusPressed()
    {
        if (_turnTimerLength + 30 < 120)
        {
            _turnTimerLength += 30f;
        }
        else
        {
            _turnTimerLength = 120f;
        }
        _timerUI.text = _turnTimerLength.ToString();
    }

    public void OnMinusPressed()
    {
        if (_turnTimerLength - 30 >= 30)
        {
            _turnTimerLength -= 30f;
        }
        else
        {
            _turnTimerLength = 30;
        }

        _timerUI.text = _turnTimerLength.ToString();
    }
    public void OnStartPressed()
    {
        _amountOfTeams = _teamsDropdown.value + 2;
        _amountOfUnits = _unitsDropdown.value + 1;
        _matchInfo.SetMatchInfo(_amountOfTeams, _amountOfUnits, _turnTimerLength);
        SceneManager.LoadScene(1);
    }
}
