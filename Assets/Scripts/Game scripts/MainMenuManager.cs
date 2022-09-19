using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    private Canvas _menuCanvas;
    
    [SerializeField]
    private TMP_Dropdown teamsDropdown;

    [SerializeField] 
    private TMP_Dropdown unitsDropdown;
    
    [SerializeField] 
    private int amountOfTeams;
    [SerializeField] 
    private int amountOfUnits;
    
    // Start is called before the first frame update
    void Start()
    {
        _menuCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnStartPressed()
    {
        amountOfTeams = teamsDropdown.value + 2;
        amountOfUnits = unitsDropdown.value + 1;
    }
    
}
