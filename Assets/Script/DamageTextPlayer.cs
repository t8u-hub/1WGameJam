using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextPlayer : MonoBehaviour
{
    private static readonly Vector3 OFFSET = new Vector3(18f, 10f, 0f);
    private static readonly Vector3 OFFSET_FLIP = new Vector3(-18f, 10f, 0f);

    [SerializeField]
    DamageText _damageText;

    public void PlayDamageTextAnimation(List<int> damageList, bool flip  = false, Vector3? pos = null)
    {
        Vector3 position = pos ?? Vector3.zero;

        var damageText = GameObject.Instantiate<DamageText>(_damageText, transform);
        damageText.transform.localPosition = position;
        var hit = damageList.Count;
        damageText.PlayDamage(damageList[0],
            () =>
            {
                if (hit > 1)
                {
                    var offset = flip ? OFFSET_FLIP : OFFSET;
                    offset.y *= Random.Range(.5f, 1.5f);
                    offset.x *= Random.Range(.9f, 1.1f);
                    damageList.RemoveAt(0);
                    PlayDamageTextAnimation(damageList, flip, position + offset);
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
