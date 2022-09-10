using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TitleUi : UiBase
{
    [SerializeField]
    private FaderPanel _faderPanel;
    [SerializeField]
    private GameObject _titleLogo;
    [SerializeField]
    private GameObject _initialObjects;
    [SerializeField]
    private GameObject _slashedEffectObjects;

    // スペースキーを押され続けている時間(sec)
    private float _timePressed = 0.0f;

    public override async void Initialize()
    {
        // 画面が切れるエフェクト用の画像は最初は表示しない
        _slashedEffectObjects.SetActive(false);

        int waitTimeForFadeIn = 2000; // フェードインにかける時間(ms)
        await _faderPanel.Fade(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), waitTimeForFadeIn);

        // 竹を揺らす & 月の明るさを変える & 竹の光芒が回る

        // スペースを2秒以上押していたらシーン遷移
        float timeToPressSpace = 2.0f;
        await UniTask.WaitUntil(() => CountUpWhilePressed(timeToPressSpace));

        // 最初に表示されていた画像を全て消して、画面が切れるエフェクトに移行する
        ChangeImages();
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

    private void ChangeImages()
    {
        _initialObjects.SetActive(false);
        _titleLogo.SetActive(false);
        _slashedEffectObjects.SetActive(true);
    }
}

