using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _tapEffectParticle;

    [SerializeField]
    private Camera _camera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            _tapEffectParticle.transform.position = pos;
            _tapEffectParticle.Emit(7);
        }
    }
}
