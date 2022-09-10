using UnityEngine;

public class EnemyAttackBall : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidBody;

    private float _damage;

    public void Throw(Vector3 dir, float damage)
    {
        _damage = damage;
        _rigidBody.AddForce(dir * 200, ForceMode2D.Impulse);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.tag;
        if (tag == "Player" && BattleManager.Instance != null)
        {
            BattleManager.Instance.EnemyAttack(_damage, transform.position.x);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}
