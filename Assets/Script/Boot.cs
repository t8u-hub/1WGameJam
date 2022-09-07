using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boot : MonoBehaviour
{
    /// <summary>
    /// ゲーム開始
    /// </summary>
    void Start()
    {
        SceneManager.Instance.BootScene(SceneDefine.Scene.Title);
    }
}
