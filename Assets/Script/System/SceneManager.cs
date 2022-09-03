using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SceneManager : MonoSingleton<SceneManager>
{

    private SceneDefine.Scene _currentScene;

    private bool _isChangeing = false;

    public void ChangeScene(SceneDefine.Scene nextScene)
    {
        StartCoroutine(ChangeSceneCoroutine(nextScene));
    }

    private IEnumerator ChangeSceneCoroutine(SceneDefine.Scene nextScene)
    {
        if (_isChangeing)
        {
            yield break;
        }

        _isChangeing = true;

        UIManager.Instance.LockGameCanvas();

        var currentSceneController = SceneDefine.SCENE_CONTROLLER_DICT[_currentScene];
        var nextSceneController = SceneDefine.SCENE_CONTROLLER_DICT[nextScene];

        var complete = false;
        var waitComplete = new WaitUntil(() => complete);
        UIManager.Instance.FadeOut(() => complete = true);
        yield return waitComplete;

        // �O�̃V�[���̏I������
        yield return currentSceneController.OnEndScene();

        // �V�[������ւ�
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene.ToString());
        _currentScene = nextScene;

        yield return null;

        var scenetView = GameObject.Find(nextScene.ToString()).GetComponent<SceneViewBase>();
        nextSceneController.SetView(scenetView);

        // ���̃V�[���̏���������
        yield return nextSceneController.OnLoadScene();

        UIManager.Instance.FadeIn(() => complete = true);
        yield return waitComplete;

        UIManager.Instance.UnlockGameCanvas();

        _isChangeing = false;
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

        // ���̃V�[���̏���������
        yield return nextSceneController.OnLoadScene();
    }

    /// <summary>
    /// �V�[��������V�[���R���g���[���[���擾
    /// </summary>
    public SceneData GetSceneController(string sceneName)
    {
        if (!Enum.TryParse(sceneName, out SceneDefine.Scene scene))
        {
            Debug.LogError("�V�[���r���[���ƃV�[���R���g���[���[�����Ή����Ă��Ȃ�");
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