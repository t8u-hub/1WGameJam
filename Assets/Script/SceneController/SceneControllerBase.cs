using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneData
{
    public virtual IEnumerator OnEndScene()
    {
        yield break;
    }

    public virtual IEnumerator OnLoadScene()
    {
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
}

public class SceneViewBase : MonoBehaviour 
{

}