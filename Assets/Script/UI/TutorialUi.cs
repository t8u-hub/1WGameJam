
using UnityEngine;
using UnityEngine.UI;

public class TutorialUi : UiBase
{
    [SerializeField]
    private Image _imgae;

    [SerializeField]
    private Image _gameOver;

    private bool _isShow = false;

    public override void Initialize()
    {
        var resultData = ResultTempData.Instance.GetData();
        var isCleard = resultData.IsClear;
        var charaLevel = resultData.FinalCharaLevel;

        var path = isCleard ? "UiResources/bg/04_ending/04_ending_cleard" :
            charaLevel == 1 ? "UiResources/bg/04_ending/01_ending_child" :
            charaLevel == 2 ? "UiResources/bg/04_ending/02_ending_young" :
                              "UiResources/bg/04_ending/03_ending_otona";

        _imgae.sprite = Resources.Load<Sprite>(path);

        _gameOver.gameObject.SetActive(!isCleard);

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
