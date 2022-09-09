using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconUi : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Sprite[] _spriteArray;

    [SerializeField]
    Animation _animation;

    private Sprite _nextSprite = null;

    public void SetNewWeapon(int level)
    {
        _nextSprite = _spriteArray[level - 1];
        _animation.Play();
    }

    public void SwitchSpriteLabel()
    {
        _image.sprite = _nextSprite;
        _image.color = Color.white;
    }
}
