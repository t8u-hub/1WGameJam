using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSePlayer : MonoBehaviour
{
    private void PlaySE()
    {
        SeAudioManager.Instance.Play(SeAudioManager.SeType.Result);
    }
}
