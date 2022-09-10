using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconUi : MonoBehaviour
{
    private static readonly Vector3 DEFAULT_SCALE = new Vector3(1f, 1f, 1f);
    private static readonly Vector3 PUSED_SCALE = new Vector3(.95f, .95f, 1f);

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Sprite[] _spriteArray;

    [SerializeField]
    Animation _animation;

    [SerializeField]
    KeyCode _keyCode;

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


    private void Update()
    {
        if (Input.GetKey(_keyCode) && !_animation.isPlaying)
        {
            this.transform.localScale = PUSED_SCALE;
            return;
        }

        if (!Input.GetKey(_keyCode) && !_animation.isPlaying && this.transform.localScale.x != 1f)
        {
            this.transform.localScale = DEFAULT_SCALE;
            return;
        }
    }
}
