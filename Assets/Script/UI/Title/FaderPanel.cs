using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class FaderPanel : UiBase
{

    public override void Initialize()
    {
    }

    // waitTimeForFadeIn ミリ秒かけて _faderPanel の色を originalColor から targetColor に変化する
    public async UniTask Fade(Color originalColor, Color targetColor, int waitTimeForFadeIn)
    {
        //  _faderPanel
        Image _faderPanel = GetComponent<Image>();

        // _faderPanel の色を更新する頻度(ms)
        // 30fps の場合 33.333 ms
        int fadeFrameRate = 40;

        // _faderPanel の色を更新する回数
        int loopTime = (int)Mathf.Ceil(waitTimeForFadeIn / fadeFrameRate);

        // それぞれの色が一度に変化する量
        float deltaR = (targetColor.r - originalColor.r) / loopTime;
        float deltaG = (targetColor.g - originalColor.g) / loopTime;
        float deltaB = (targetColor.b - originalColor.b) / loopTime;
        float deltaA = (targetColor.a - originalColor.a) / loopTime;

        for (int i = 0; i < loopTime - 1; i++)
        {
            //  _faderPanel の色を更新する
            _faderPanel.color = new Color(
                originalColor.r + i * deltaR,
                originalColor.g + i * deltaG,
                originalColor.b + i * deltaB,
                originalColor.a + i * deltaA
            );

            // 待つ
            await UniTask.Delay(fadeFrameRate);
        }
        // 最後は強制的に targetColor にする
        _faderPanel.color = targetColor;
    }
}
