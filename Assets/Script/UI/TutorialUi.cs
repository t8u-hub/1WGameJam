
using UnityEngine;
using UnityEngine.UI;

public class TutorialUi : UiBase
{
    [SerializeField]
    private Image _imgae;
    
    private bool _isShow = false;

    public override void Initialize()
    {
        var isCleard = ResultTempData.Instance.GetData().IsClear;
        var path = isCleard ? "UiResources/bg/04_ending/01_ending_cleard" : "UiResources/bg/04_ending/01_ending_cleard";

        _imgae.sprite = Resources.Load<Sprite>(path);
        _isShow = true;
    }

    public void Update()
    {
        if (_isShow && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.Instance.ChangeScene(SceneDefine.Scene.Title);
        }
    }
}
