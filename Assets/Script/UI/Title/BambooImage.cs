using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooImage : UiBase
{
    [SerializeField]
    private bool _mirror = false; // true にすると動きが逆になる
    [SerializeField]
    private float _amplitude = 0.05f; // 竹が揺れる幅
    [SerializeField]
    private float _frequency = 5f; // 竹が揺れる速さ
    private RectTransform _rectTransform;
    private Vector3 _initialPosition;

    public override void Initialize()
    {
    }

    private void Start()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _initialPosition = _rectTransform.position;
    }

    private void Update()
    {
        int sign = _mirror ? 1 : -1;
        // 竹が揺れる動き
        float posX = _initialPosition.x + sign * _amplitude * Mathf.Sin(_frequency * Time.time);
        _rectTransform.position = new Vector3(posX, _initialPosition.y, _initialPosition.z);
    }
}
