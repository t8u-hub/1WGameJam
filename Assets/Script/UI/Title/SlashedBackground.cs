using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SlashedBackground : UiBase
{
    [SerializeField]
    private float _moveX;
    [SerializeField]
    private float _moveY;
    [SerializeField]
    private int _duration; // 移動にかける時間(ms)

    // private RectTransform _rectTransform;
    public override void Initialize()
    {
    }

    // 自身の RectTransform を _targetPosition まで _duration ミリ秒かけて移動する
    public async UniTask MoveSelf()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 initialPosition = rectTransform.position;

        // rectTransform を移動する頻度(ms)
        // 30fps の場合 33.333 ms
        int frameRate = 40;

        // rectTransform を移動する回数
        int loopTime = (int)Mathf.Ceil(_duration / frameRate);

        // 一度に移動する距離
        float deltaX = _moveX / loopTime;
        float deltaY = _moveY / loopTime;

        for (int i = 0; i < loopTime; i++)
        {
            rectTransform.position += new Vector3(deltaX, deltaY, 0);
            await UniTask.Delay(frameRate);
        }
    }
}
