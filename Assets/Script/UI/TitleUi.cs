using UnityEngine;
using UnityEngine.UI;

public class TitleUi : UiBase
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private Button _titleButton;

    public override void Initialize()
    {
        _button.onClick.AddListener(() => SceneManager.Instance.ChangeScene(SceneDefine.Scene.Result));
        _titleButton.onClick.AddListener(() => SceneManager.Instance.ChangeScene(SceneDefine.Scene.Game));
    }
}

