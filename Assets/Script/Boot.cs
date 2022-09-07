using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boot : MonoBehaviour
{
    void Start()
    {
        SceneManager.Instance.BootScene(SceneDefine.Scene.Game);   
    }
}
