using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameSceneView : SceneViewBase
{
    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    [SerializeField]
    private Vector2[] _cameraParamArray;
    public Vector2[] CameraParamArray => _cameraParamArray;
}

public class GameSceneController : SceneControllerBase<GameSceneView>
{
    public void ChangeStageLevel(int level)
    {
        _view.StartCoroutine(ChangeStageLevelInner(level, () => { }));
    }

    private IEnumerator ChangeStageLevelInner(int level, System.Action onComplete)
    {
        var complete = false;
        var waitComplete = new WaitUntil(() => complete);
        
        UIManager.Instance.FadeOut(() => complete = true);
        yield return waitComplete;

        var pos = _view.MainCamera.transform.position;
        pos.y = _view.CameraParamArray[level - 1].x;
        _view.MainCamera.transform.position = pos;

        _view.MainCamera.orthographicSize = _view.CameraParamArray[level - 1].y;

        UIManager.Instance.FadeIn(() => complete = true);
        yield return waitComplete;
    }
}
