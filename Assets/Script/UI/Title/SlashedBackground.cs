using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SlashedBackground : UiBase
{
    [SerializeField]
    private readonly Vector3 _targetPosition;

    // private RectTransform _rectTransform;
    public override void Initialize()
    {
    }

    /// <summary>
    /// 自身の RectTransform を _targetPosition まで duration ミリ秒かけて移動する
    /// </summary>
    /// <param name="duration">移動にかける時間</param>
    public async UniTask MoveSelf(int duration)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 initialPosition = rectTransform.position;

        // rectTransform を移動する頻度(ms)
        // 30fps の場合 33.333 ms
        int frameRate = 40;

        // rectTransform を移動する回数
        int loopTime = (int)Mathf.Ceil(duration / frameRate);

        // 一度に移動する距離
        float deltaX = (_targetPosition.x - initialPosition.x) / loopTime;
        float deltaY = (_targetPosition.y - initialPosition.y) / loopTime;
        float deltaZ = (_targetPosition.z - initialPosition.z) / loopTime;

        for (int i = 0; i < loopTime - 1; i++)
        {
            rectTransform.position += new Vector3(deltaX, deltaY, deltaZ);

            // 待つ
            await UniTask.Delay(frameRate);
        }
        // 最後は強制的に targetColor にする
        rectTransform.position = _targetPosition;
    }
}
