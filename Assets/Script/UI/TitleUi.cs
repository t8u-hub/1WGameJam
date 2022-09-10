using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TitleUi : UiBase
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private Button _titleButton;

    [SerializeField]
    private FaderPanel _faderPanel;

    [SerializeField]
    private GameObject _bamboos;

    [SerializeField]
    private GameObject _moon;

    [SerializeField]
    private GameObject _background;

    // スペースキーを押され続けている時間(sec)
    private float _timePressed = 0.0f;

    public override async void Initialize()
    {
        _button.onClick.AddListener(() => SceneManager.Instance.ChangeScene(SceneDefine.Scene.Result));
        _titleButton.onClick.AddListener(() => SceneManager.Instance.ChangeScene(SceneDefine.Scene.Game));

        int waitTimeForFadeIn = 2000; // フェードインにかける時間(ms)
        await _faderPanel.Fade(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), waitTimeForFadeIn);

        // 竹を揺らす & 月の明るさを変える & 竹の光芒が回る

        // スペースを2秒以上押していたらシーン遷移
        float timeToPressSpace = 2.0f;
        await UniTask.WaitUntil(() => CountUpWhilePressed(timeToPressSpace));

        // 各アニメーションを止める
        StopAnimations();
    }

    /// <param name="threshold">このメソッドが true を返すためにスペースキーを押し続ける時間(sec)</param>
    private bool CountUpWhilePressed(float threshold)
    {
        if (_timePressed > threshold)
        {
            return true;
        }
        if (Input.GetKey("space"))
        {
            _timePressed += Time.deltaTime;
        }
        if (Input.GetKeyUp("space"))
        {
            // キーを離すとカウントが 0 に戻る
            _timePressed = 0.0f;
        }
        return false;
    }

    private void StopAnimations()
    {
        // 揺れる竹のアニメーションを停止する
        BambooImage[] _bambooImages = _bamboos.GetComponentsInChildren<BambooImage>();
        foreach (BambooImage _bambooImage in _bambooImages)
        {
            _bambooImage.StopWaving();
            _bambooImage.gameObject.SetActive(false);
        }

        _moon.SetActive(false);
        _background.SetActive(false);
    }
}

