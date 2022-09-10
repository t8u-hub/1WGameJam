using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectAnimator : MonoBehaviour
{
    private CanvasGroup _canvasGroup = null;

    private Image _image;

    #region フェード

    /// <summary>
    /// フェードの色を選択
    /// </summary>
    public void SetColor(Color32 color)
    {
        if (_image == null)
        {
            _image = this.GetComponent<Image>();
        }

        _image.color = color;
    }


    public void PlayFade(float start, float end, Action onComplete, float dulation = .2f)
    {
        UIManager.Instance.StartCoroutine(PlayFadeInner(start, end, onComplete, dulation));
    }


    public IEnumerator PlayFadeInner(float start, float end, Action onComplete, float dulation)
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                onComplete?.Invoke();
                yield break;
            }
        }

        // 所要時間が0より大きい時
        if (dulation > 0f)
        {
            var time = 0f;
            yield return new WaitUntil(() =>
            {
                var nomalizedTime = time / dulation;
                _canvasGroup.alpha = start + (end - start) * nomalizedTime;

                time += Time.unscaledDeltaTime;
                return time >= dulation;
            });
        }

        _canvasGroup.alpha = end;
        onComplete?.Invoke();
    }

    #endregion
}
