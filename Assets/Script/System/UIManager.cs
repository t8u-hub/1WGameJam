using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public enum UiType
    {
        Normal,
        System,
    }

    [SerializeField]
    private Canvas _uiCanvas;

    [SerializeField]
    private GraphicRaycaster _uiGraphicRaycaster;

    [SerializeField]
    private Canvas _systemUiCanvas;

    [SerializeField]

    private GraphicRaycaster _systemUiGraphicRaycaster;

    [SerializeField]
    private ObjectAnimator _fadeAnimator;

    /// <summary>
    /// 指定されたprefabをキャンバス以下に生成☀
    /// </summary>
    public GameObject CreateUi(GameObject prefab, UiType uiType = UiType.Normal)
    {
        var parent = (uiType == UiType.Normal) ? _uiCanvas : _systemUiCanvas;
        return GameObject.Instantiate(prefab, parent.transform);
    }

    /// <summary>
    /// 指定したキャンバス以下のUIを削除
    /// </summary>
    public void ClearnUi(UiType uiType = UiType.Normal)
    {
        var parent = (uiType == UiType.Normal) ? _uiCanvas : _systemUiCanvas;
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// すべてのUIを削除
    /// </summary>
    public void CleanAllUi()
    {
        ClearnUi(UiType.Normal);
        ClearnUi(UiType.System);
    }

    public void FadeIn(System.Action onComplete)
    {
        _fadeAnimator.PlayFade(1, 0, onComplete);
    }

    public void FadeOut(System.Action onComplete)
    {
        _fadeAnimator.PlayFade(0, 1, onComplete);
    }

    public void LockGameCanvas()
    {
        _uiGraphicRaycaster.enabled = false;
        _systemUiGraphicRaycaster.enabled = false;
    }

    public void UnlockGameCanvas()
    {
        _uiGraphicRaycaster.enabled = true;
        _systemUiGraphicRaycaster.enabled = true;
    }
}
