using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBall : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidBody;

    private int _hit;

    private int _damage;

    public void Throw(float speed, float angle, int hit, int damage, bool right)
    {
        _hit = hit;
        _damage = damage;
        var vector = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        if (!right) vector.x = -vector.x;
        _rigidBody.AddForce(vector * speed, ForceMode2D.Impulse);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.tag;
        if (tag == "Enemy")
        {
            var enemy = collision.transform.GetComponent<Enemy>();
            enemy?.OnDamage(_hit, _damage);
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
