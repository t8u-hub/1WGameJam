using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SceneData
{
    protected abstract BgmAudioamager.BgmType GetBgmType();

    public virtual IEnumerator OnEndScene()
    {
        yield break;
    }

    public virtual IEnumerator OnLoadScene()
    {
        BgmAudioamager.Instance.PlayBgm(GetBgmType());
        yield break;
    }

    public abstract void SetView(SceneViewBase view);
}

public abstract class SceneControllerBase<T> : SceneData where T : SceneViewBase
{
    protected T _view;


    public override void SetView(SceneViewBase view)
    {
        _view = view as T;
    }

    public override IEnumerator OnLoadScene()
    {
        if (_view.UiBase != null)
        {
            var uiBase = UIManager.Instance.CreateUi(_view.UiBase.gameObject).GetComponent<UiBase>();
            uiBase.Initialize();
        }

        yield return base.OnLoadScene();
    }

    public override IEnumerator OnEndScene()
    {
        UIManager.Instance.CleanAllUi();
        yield return base.OnEndScene();
    }
}

public class SceneViewBase : MonoBehaviour 
{
    [SerializeField]
    private UiBase _uiBase;
    public UiBase UiBase => _uiBase;
}