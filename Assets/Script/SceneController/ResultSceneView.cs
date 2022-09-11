using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResultSceneView : SceneViewBase
{

}

public class ResultSceneController : SceneControllerBase<ResultSceneView>
{
    protected override BgmAudioamager.BgmType GetBgmType() => BgmAudioamager.BgmType.Result;
}
