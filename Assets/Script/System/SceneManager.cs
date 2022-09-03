using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SceneManager : MonoSingleton<SceneManager>
{

    private SceneDefine.Scene _currentScene;

    public void ChangeScene(SceneDefine.Scene nextScene)
    {
        StartCoroutine(ChangeSceneCoroutine(nextScene));
    }

    private IEnumerator ChangeSceneCoroutine(SceneDefine.Scene nextScene)
    {
        var currentSceneController = SceneDefine.SCENE_CONTROLLER_DICT[_currentScene];
        var nextSceneController = SceneDefine.SCENE_CONTROLLER_DICT[nextScene];

        // 前のシーンの終了処理
        yield return currentSceneController.OnEndScene();

        // シーン入れ替え
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene.ToString());
        _currentScene = nextScene;

        yield return null;

        var scenetView = GameObject.Find(nextScene.ToString()).GetComponent<SceneViewBase>();
        nextSceneController.SetView(scenetView);

        // 次のシーンの初期化処理
        yield return nextSceneController.OnLoadScene();
    }

    public void BootScene(SceneDefine.Scene nextScene)
    {
        StartCoroutine(BootSceneCoroutine(nextScene));
    }

    private IEnumerator BootSceneCoroutine(SceneDefine.Scene scene)
    {

        var nextSceneController = SceneDefine.SCENE_CONTROLLER_DICT[scene];

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
        _currentScene = scene;

        yield return null;

        var sceneObject = GameObject.Find(scene.ToString());
        var scenetView = sceneObject.GetComponent<SceneViewBase>();
        nextSceneController.SetView(scenetView);

        // 次のシーンの初期化処理
        yield return nextSceneController.OnLoadScene();
    }

    /// <summary>
    /// シーン名からシーンコントローラーを取得
    /// </summary>
    public SceneData GetSceneController(string sceneName)
    {
        if (!Enum.TryParse(sceneName, out SceneDefine.Scene scene))
        {
            Debug.LogError("シーンビュー名とシーンコントローラー名が対応していない");
            return null;
        }

        return SceneDefine.SCENE_CONTROLLER_DICT[scene];
    }
}

public class SceneDefine
{
    public enum Scene
    {
        Title,
        Tutorial,
        Game,
    }

    public static readonly Dictionary<Scene, SceneData> SCENE_CONTROLLER_DICT = new Dictionary<Scene, SceneData>
    {
        { Scene.Title, new TitleSceneController() },
        { Scene.Tutorial, new TutorialSceneController() },
        { Scene.Game, new GameSceneController() },
    };
}