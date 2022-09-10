using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private Font[] _fontArray;

    [SerializeField]
    private Vector2 _threshold;

    System.Action _onNext = null;
    System.Action _onEnd = null;

    public void PlayDamage(int num, System.Action onNext, System.Action onEnd)
    {
        _onNext = onNext;
        _onEnd = onEnd;

        if (num < _threshold.x)
        {
            text.font = _fontArray[0];
        }
        else if (num < _threshold.y)
        {
            text.font = _fontArray[1];
        }
        else
        {
            text.font = _fontArray[2];
        }
        text.text = num.ToString();
        _animation.Play();
    }

    public void OnNextIn()
    {
        _onNext?.Invoke();
    }

    public void OnAnimationEnd()
    {
        _onEnd?.Invoke();
        Destroy(this.gameObject);
    }
}
