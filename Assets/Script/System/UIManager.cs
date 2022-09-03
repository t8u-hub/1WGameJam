using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Canvas _systemUiCanvas;

    /// <summary>
    /// 指定されたprefabをキャンバス以下に生成
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
}
