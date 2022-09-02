using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneView : SceneViewBase
{
    [SerializeField]
    private Button _nextButton;
    public Button NextButton => _nextButton;
}

public class GameSceneController : SceneControllerBase<GameSceneView>
{
    public override IEnumerator OnLoadScene()
    {
        _view.NextButton.onClick.AddListener(() => SceneManager.Instance.ChangeScene(SceneDefine.Scene.Title));
        yield return base.OnLoadScene();
    }
}
