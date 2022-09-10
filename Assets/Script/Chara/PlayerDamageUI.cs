using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageUI : MonoBehaviour
{
    private const float MAX_DAMAGE = 30;
    private const float DAMAGE_PAR_ICON = MAX_DAMAGE / 5;

    [SerializeField]
    private Image[] _damageIconArray;

    [SerializeField]
    private Animation _animation;

    // Update is called once per frame
    void Update()
    {
        if (BattleManager.Instance == null)
        {
            return;
        }

        if (BattleManager.Instance != null && BattleManager.Instance.StopUpdate)
        {
            return;
        }

        var curDamage = BattleManager.Instance.ScaledTotalDamage;
        bool animation = false;

        for(var i = 0; i < 5; i++)
        {
            var alpha = 1f;
            if (curDamage >= DAMAGE_PAR_ICON * (i + 1))
            {
                // 全点灯
                alpha = 1;
            }
            else if (curDamage < DAMAGE_PAR_ICON * i)
            {
                // 全消灯
                alpha = 0;
            }
            else
            {
                alpha = (curDamage - DAMAGE_PAR_ICON * i) / DAMAGE_PAR_ICON;
            }

            var color = _damageIconArray[i].color;
            color.a = alpha;
            _damageIconArray[i].color = color;

            if (i == 4 && alpha > 0f)
            {
                animation = true;
            }
        }

        if (animation && !_animation.isPlaying)
        {
            _animation.Play();
        }
        else if (!animation && _animation.isPlaying)
        {
            _animation.Stop();
            _animation.Rewind();
        }
    }
}
