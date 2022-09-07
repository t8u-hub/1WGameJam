using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyObject : MonoBehaviour
{
    public enum RigidBodyType
    {
        LeftWall,
        RightWall,
        Floor,
    }

    [SerializeField]
    RigidBodyType _rigidBodyType = RigidBodyType.Floor;

    private bool _touchPlayer = false;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SetObjectOutside(collision.transform as RectTransform, collision.transform.GetComponent<BoxCollider2D>().size);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SetObjectOutside(collision.transform as RectTransform, collision.transform.GetComponent<BoxCollider2D>().size);
        }
    }

    private void SetObjectOutside(RectTransform targetTransform, Vector2 collisionSize)
    {
        var selfPosition = this.transform.position;
        var targetPosition = targetTransform.position;

        switch (_rigidBodyType)
        {
            case RigidBodyType.LeftWall:
                targetPosition.x = this.transform.position.x + (targetTransform.localScale.x * collisionSize.x / 2f);
                if (targetTransform.position.x < targetPosition.x)
                {
                    targetTransform.position = targetPosition;
                }
                return;
            case RigidBodyType.RightWall:
                targetPosition.x = this.transform.position.x - (targetTransform.localScale.x * collisionSize.x / 2f);
                if (targetTransform.position.x > targetPosition.x)
                {
                    targetTransform.position = targetPosition;
                }
                return;
            case RigidBodyType.Floor:
                targetPosition.y = this.transform.position.y + (targetTransform.localScale.y * collisionSize.y / 2f);
                if (targetTransform.position.y < targetPosition.y)
                {
                    targetTransform.position = targetPosition;
                }
                return;
        }
    }
}
