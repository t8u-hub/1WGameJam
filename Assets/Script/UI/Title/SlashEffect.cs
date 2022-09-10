using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SlashEffect : MonoBehaviour
{
    private Image _image;
    [SerializeField]
    private int _duration; // 画面を切るエフェクトに要する時間(ms)

    void Start()
    {
        _image = gameObject.GetComponent<Image>();
        _image.fillAmount = 0;
    }

    public async UniTask ChangeFillAmount()
    {
        int frameRate = 40;
        int loopTime = (int)Mathf.Ceil(_duration / frameRate);

        // fill amount が一度に変化する量
        float deltaAmount = 1f / (float)loopTime;

        for (int i = 0; i < loopTime - 1; i++)
        {
            _image.fillAmount += deltaAmount;
            await UniTask.Delay(frameRate);
        }

        // 最後は強制的に 1 にする
        _image.fillAmount = 1;

    }
}
