using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameHandler : MonoBehaviour
{

    private MatchInfo _matchInfo;

    [SerializeField]
    private List<GameObject> winningTeamUnits;

    [SerializeField]
    private Camera sceneCamera;

    private bool _wasWin = false;

    [SerializeField]
    private Transform[] podiumTransforms;


    // Start is called before the first frame update
    void Awake()
    {
        GetPostMatchInfo();
    }

    private void Start()
    {
        if (!_wasWin)
        {
            sceneCamera.transform.position = new Vector3(8, sceneCamera.transform.position.y, sceneCamera.transform.position.z);
        }
        else
        {
            sceneCamera.transform.position = new Vector3(-27, sceneCamera.transform.position.y, sceneCamera.transform.position.z);
            for (int i = 0; i < winningTeamUnits.Count; i++)
            {
                winningTeamUnits[i].transform.SetParent(podiumTransforms[i].GetChild(0));
                winningTeamUnits[i].transform.rotation = podiumTransforms[i].GetChild(0).transform.rotation;
                winningTeamUnits[i].transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    private void GetPostMatchInfo()
    {
        _matchInfo = MatchInfo.Instance;
        if (_matchInfo.WasWin)
        {
            winningTeamUnits = _matchInfo.WinningTeamUnits;
        }

        _wasWin = _matchInfo.WasWin;

    }
}
