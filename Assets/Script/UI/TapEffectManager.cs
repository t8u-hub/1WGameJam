using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapEffectManager : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _tapEffectParticle;

    [SerializeField]
    private ParticleSystem _defaultTapEffectParticle;

    [SerializeField]
    private Camera _camera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var touchPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = touchPosition;

            _defaultTapEffectParticle.Emit(1);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                _tapEffectParticle.Emit(7);
            }
        }
    }
}

