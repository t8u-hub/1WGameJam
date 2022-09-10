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

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Image _image;

    int _level;

    public void PlayCutin(int level)
    {
        _level = level;
        _image.sprite = Resources.Load<Sprite>(pathArray[(_level - 1) * 2]);
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
