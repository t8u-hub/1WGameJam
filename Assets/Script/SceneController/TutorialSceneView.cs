using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialSceneView : SceneViewBase
{

}

public class TutorialSceneController : SceneControllerBase<TutorialSceneView>
{
    protected override BgmAudioamager.BgmType GetBgmType() => BgmAudioamager.BgmType.Result;
}
