using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBall : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidBody;

    private int _hit;

    private int _damage;

    public void Throw(int hit, int damage, bool right)
    {
        _hit = hit;
        _damage = damage;
        var vector = right ? new Vector2(1, 1) : new Vector2(-1, 1);
        _rigidBody.AddForce(vector * 300, ForceMode2D.Impulse);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.tag;
        if (tag == "Enemy")
        {
            var enemy = collision.transform.GetComponent<Enemy>();
            enemy?.OnDamage(1, _damage);

            _hit--;
            if (_hit == 0)
            {
                Destroy(this.gameObject);
            }
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
