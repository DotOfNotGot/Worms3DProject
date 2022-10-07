using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitUIHandler : MonoBehaviour
{

    private Transform _cameraTransform;
    private Canvas _playerInfoCanvas;

    [SerializeField]
    private TextMeshProUGUI _unitHPDisplay;

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

    public void SetUnitInfoDisplay(int newHp)
    {
        if (newHp < 0)
        {
            newHp = 0;
        }
        _unitHPDisplay.text = newHp.ToString();
    }

    public void DisableUnitInfoDisplay()
    {
        _unitHPDisplay.enabled = false;
    }

}
