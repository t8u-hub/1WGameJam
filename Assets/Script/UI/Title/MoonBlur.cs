using UnityEngine;
using UnityEngine.UI;

public class MoonBlur : MonoBehaviour
{
    private Image _image;

    [SerializeField]
    private float _frequency = 0.5f; // 明滅の速度
    [SerializeField]
    private float _alphaMax = 1f; // ブラーの最大のα値 (0~1)
    [SerializeField]
    private float _alphaMin = 0; // ブラーの最小のα値 (0~1)

    private void Start()
    {
        _image = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        float alpha = _alphaMin + (_alphaMax - _alphaMin) * Mathf.Sin(_frequency * Time.time);
        _image.color = new Color(1, 1, 1, alpha);
    }
}
