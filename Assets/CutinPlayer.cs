using UnityEngine.UI;
using UnityEngine;

public class CutinPlayer : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Image _image;


    public void PlayCutin(int level)
    {
        Debug.Log("再生");
        animator.Play("New State");
    }

    public void ChangeSprite()
    {

    }

    public void OnEndAnimation()
    {
        Destroy(this.gameObject);
    }


}
