using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitUIHandler : MonoBehaviour
{

    private Transform _cameraTransform;
    private Canvas _playerInfoCanvas;

    [SerializeField]
    private TextMeshProUGUI unitHPDisplay;

    // Start is called before the first frame update
    void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _playerInfoCanvas = gameObject.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        _playerInfoCanvas.transform.rotation = Quaternion.LookRotation(_cameraTransform.forward, gameObject.transform.parent.up);
    }

    public void SetPlayerInfoDisplay(int newHP)
    {
        if (newHP < 0)
        {
            newHP = 0;
        }
        unitHPDisplay.text = newHP.ToString();
    }

}