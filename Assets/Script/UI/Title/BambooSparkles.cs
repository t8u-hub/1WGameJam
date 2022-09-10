using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooSparkles : MonoBehaviour
{
    // 中央の竹の光芒が回転する

    private RectTransform _rectTransform;

    [SerializeField]
    private float _rotateSpeed = 3; // 回転速度
    void Start()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    // 回転
    void Update()
    {
        float rotZ = Time.time * _rotateSpeed % 360;
        _rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
    }
}
