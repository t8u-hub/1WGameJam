using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextPlayer : MonoBehaviour
{
    private static readonly Vector3 OFFSET = new Vector3(18f, 10f, 0f);
    private static readonly Vector3 OFFSET_FLIP = new Vector3(-18f, 10f, 0f);

    [SerializeField]
    DamageText _damageText;

    public void PlayDamageTextAnimation(int amount, int hit, bool flip  = false, Vector3? pos = null)
    {
        Vector3 position = pos ?? Vector3.zero;

        var damageText = GameObject.Instantiate<DamageText>(_damageText, transform);
        damageText.transform.localPosition = position;

        damageText.PlayDamage(amount,
            () =>
            {
                if (hit > 1)
                {
                    var offset = flip ? OFFSET_FLIP : OFFSET;
                    PlayDamageTextAnimation(amount, hit - 1, flip, position + offset);
                }
            },
            () =>
            { 
                if (hit == 1)
                {
                    Destroy(this.gameObject);
                }
            });
    }
}
