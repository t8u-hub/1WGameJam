using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TitleSceneView : SceneViewBase
{ 

}

public class TitleSceneController : SceneControllerBase<TitleSceneView>
{
    protected override BgmAudioamager.BgmType GetBgmType() => BgmAudioamager.BgmType.Title;
}
