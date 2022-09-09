using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D _boxCollider;

    [SerializeField]
    private AttackBall _attackBall;

    /// <summary>
    /// 攻撃が継続する時間
    /// </summary>
    private float _attackLastTime = 0f;

    /// <summary>
    /// 与えるダメージ量
    /// </summary>
    private int _damageAmount = 0;

    /// <summary>
    /// ヒット数
    /// </summary>
    private int _hitCount = 0;

    private void Awake()
    {
        _boxCollider.enabled = false;
    }

    public void Update()
    {
        if (_attackLastTime > 0f)
        {
            _attackLastTime -= Time.deltaTime;
            return;
        }

        _boxCollider.enabled = false;
        _damageAmount = 0;
        _hitCount = 0;
    }

    public void ResetCollider()
    {
        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = new Vector2(0, 0);
        _boxCollider.offset = new Vector2(0, 0);

        _boxCollider.enabled = false;
    }


    /// <summary>
    /// 通常攻撃
    /// </summary>

    public void AttackNormal(int hit, int damage)
    {
        ResetCollider();

        // 範囲
        _boxCollider.size = new Vector2(40, 60);
        _boxCollider.offset = new Vector2(-20, 0);

        // 使うコライダー
        _boxCollider.enabled = true;

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = .1f;
    }


    /// <summary>
    /// 移動攻撃
    /// </summary>
    public void AttackHorizontalMove(int hit, int damage)
    {
        ResetCollider();

        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = new Vector2(40, 60);
        _boxCollider.offset = new Vector2(0, 0);

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = .5f;
    }

    /// <summary>
    /// 遠距離攻撃
    /// </summary>
    public void AttackMiddleDistance(int hit, int damage, Transform parent)
    {
        var ball = GameObject.Instantiate<AttackBall>(_attackBall, parent);
        ball.transform.position = transform.position + new Vector3(0, 18, 0);
        ball.Throw(hit, damage, transform.parent.localScale.x < 0);
    }

    /// <summary>
    /// 広範囲攻撃
    /// </summary>
    public void AttackLongRange(int hit, int damage)
    {
        ResetCollider();

        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = new Vector2(200, 60);
        _boxCollider.offset = new Vector2(-100, 0);

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = .1f;
    }

    /// <summary>
    /// 降下攻撃
    /// </summary>
    public void AttackVerticalMove(int hit, int damage)
    {
        ResetCollider();

        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = new Vector2(240, 60);
        _boxCollider.offset = new Vector2(0, 0);

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = .1f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" )
        {
            var enemy = collision.transform.GetComponent<Enemy>();
            enemy?.OnDamage(_hitCount, _damageAmount);
        }
    }


    public void FinishAttack()
    {
        _attackLastTime = 0;
    }
}
