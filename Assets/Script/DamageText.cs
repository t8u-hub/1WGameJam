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

    public void PlayDamage(int num)
    {
        text.text = num.ToString();
        _animation.Play();
    }

    public void OnAnimationEnd()
    {
        Destroy(this.gameObject);
    }
}
