using UnityEngine.UI;
using UnityEngine;

public class CutinPlayer : MonoBehaviour
{
    private static readonly string[] pathArray =
    {
        "Chara/Lv1/0110_special_1",
        "Chara/Lv1/0111_special_2",
        "Chara/Lv2/0210_special_1",
        "Chara/Lv2/0211_special_2",
        "Chara/Lv3/0310_special_1",
        "Chara/Lv3/0311_special_2",
    };

    private static readonly string[] bgPathArray =
{
        "Chara/Lv1/special_attack_bg_01",
        "Chara/Lv2/special_attack_bg_02",
        "Chara/Lv3/special_attack_bg_03",
    };

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Image _bgImage;

    int _level;

    public void PlayCutin(int level)
    {
        _level = level;
        _image.sprite = Resources.Load<Sprite>(pathArray[(_level - 1) * 2]);
        _bgImage.sprite = Resources.Load<Sprite>(bgPathArray[level - 1]);
        animator.Play("New State");
    }

    public void ChangeSprite()
    {
        _image.sprite = Resources.Load<Sprite>(pathArray[(_level - 1) * 2 + 1]);
    }

    public void OnEndAnimation()
    {
        Destroy(this.gameObject);
    }
}
