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

    public void AttackNormal(int hit, int damage, Vector2 collisitonSize)
    {
        ResetCollider();

        // 範囲
        _boxCollider.size = collisitonSize;
        _boxCollider.offset = new Vector2(-collisitonSize.x / 2, collisitonSize.y / 2);

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
    public void AttackHorizontalMove(int hit, int damage, Vector2 collisitonSize, float lastTime)
    {
        ResetCollider();

        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = collisitonSize;
        _boxCollider.offset = new Vector2(0, collisitonSize.y / 2);

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = lastTime;
    }

    /// <summary>
    /// 遠距離攻撃
    /// </summary>
    public void AttackMiddleDistance(int hit, int damage, Transform parent, Vector3 offset, float ballSpeed, float ballAngle)
    {
        var ball = GameObject.Instantiate<AttackBall>(_attackBall, parent);
        ball.transform.position = transform.position + offset;
        ball.Throw(ballSpeed, ballAngle, hit, damage, transform.parent.localScale.x < 0);
    }

    /// <summary>
    /// 広範囲攻撃
    /// </summary>
    public void AttackLongRange(int hit, int damage, Vector2 collisitonSize)
    {
        ResetCollider();

        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = collisitonSize;
        _boxCollider.offset = new Vector2(-collisitonSize.x / 2, collisitonSize.y / 2);

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = .1f;
    }

    /// <summary>
    /// 降下攻撃
    /// </summary>
    public void AttackVerticalMove(int hit, int damage, Vector2 collisitonSize)
    {
        ResetCollider();

        // 使うコライダー
        _boxCollider.enabled = true;

        // 範囲
        _boxCollider.size = collisitonSize;
        _boxCollider.offset = new Vector2(0, collisitonSize.y / 2);

        // ダメージとヒット数
        _damageAmount = damage;
        _hitCount = hit;

        // 持続時間
        _attackLastTime = .1f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            var enemy = collision.transform.GetComponent<Enemy>();
            var damage = (int)(BattleManager.Instance.PlayerAttackCoef *(float)_damageAmount);
            enemy?.OnDamage(_hitCount, damage);
        }
    }


    public void FinishAttack()
    {
        _attackLastTime = 0;
    }
}
