using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PostGameHandler : MonoBehaviour
{

    private MatchInfo _matchInfo;

    [SerializeField]
    private List<GameObject> _winningTeamUnits;

    [SerializeField]
    private Camera _sceneCamera;

    private bool _wasWin = false;

    [SerializeField]
    private Transform[] _podiumTransforms;

    private TMP_Text _winText;



    // Start is called before the first frame update
    void Awake()
    {
        GetPostMatchInfo();
        _winText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        if (!_wasWin)
        {
            _sceneCamera.transform.position = new Vector3(8, _sceneCamera.transform.position.y, _sceneCamera.transform.position.z);
        }
        else
        {
            _sceneCamera.transform.position = new Vector3(-27, _sceneCamera.transform.position.y, _sceneCamera.transform.position.z);
            _winText.text = $"Team {_winningTeamUnits[0].GetComponent<UnitInformation>().TeamIndex + 1} won!";
            for (int i = 0; i < _winningTeamUnits.Count; i++)
            {
                _winningTeamUnits[i].transform.SetParent(_podiumTransforms[i]);
                _winningTeamUnits[i].transform.rotation = _podiumTransforms[i].transform.rotation;
                _winningTeamUnits[i].transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    private void GetPostMatchInfo()
    {
        _matchInfo = MatchInfo.Instance;
        if (_matchInfo.WasWin)
        {
            _winningTeamUnits = _matchInfo.WinningTeamUnits;
        }

        _wasWin = _matchInfo.WasWin;

    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

}
