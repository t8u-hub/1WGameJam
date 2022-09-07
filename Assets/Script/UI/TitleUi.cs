using UnityEngine;
using UnityEngine.UI;

public class TitleUi : UiBase
{
    [SerializeField]
    private Button _button;

    public override void Initialize()
    {
        _button.onClick.AddListener(() => SceneManager.Instance.ChangeScene(SceneDefine.Scene.Game));
    }
}

