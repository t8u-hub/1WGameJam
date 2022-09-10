using UnityEngine;

public class EnemyFlying : Enemy
{
    // 基準高さ
    private float _defaultHeight = 75f;

    private bool _defaultMoveRight;

    public static new EnemyFlying CreateObject(Enemy prefab, Parameter paramter, Transform parentTransform)
    {
        var enemyFlying = GameObject.Instantiate<EnemyFlying>(prefab as EnemyFlying, parentTransform);
        enemyFlying._parameter = paramter;
        enemyFlying.transform.localPosition = Vector3.zero;
        enemyFlying._hp = paramter.HitPoint;

        enemyFlying.OnCreated();
        return enemyFlying;
    }

    public override void FixedUpdateInner()
    {
        if (_hp <= 0)
        {
            return;
        }

        var targetDir = (_targetTransform.position - transform.position).normalized;
        var imgScale = targetDir.x < 0 ? IMG_DEFAULT : IMG_FLIP;
        _imageTransform.localScale = imgScale;

        _previousPosition = transform.position;

        switch (_state)
        {
            case State.Knockback:
                _speed = -targetDir * 300;
                _state = State.Damaging;
                _remainMoveTime = .5f;
                break;

            case State.Damaging:
                _speed = _speed * 0.9f;
                break;

            case State.Default: // 巡回
                // 右に行くか左に行くか
                var horizontalVector = _defaultMoveRight ? Vector3.right : Vector3.left;

                // 必要なら上へのベクトルも足す
                var verticalVector = (transform.position.y < _defaultHeight) ? Vector3.up :
                                     (transform.position.y > _defaultHeight) ? Vector3.down : Vector3.zero;

                // CSV指定の速度スケールをかける
                var movedir = (horizontalVector + verticalVector).normalized;
                _speed = movedir * _parameter.MoveSpeed / Time.fixedDeltaTime;

                break;

            case State.Move:
                _speed = targetDir * _parameter.MoveSpeed / Time.fixedDeltaTime;
                break;

            default:
                _speed = ZERO;
                break;
        }

        // ダメージを受けてる間は横のスピードを経過時間で制御
        if (_state == State.Damaging)
        {
            _speed = _speed * 0.9f;
        }

        var nextPosition = _previousPosition + _speed * Time.deltaTime;

        // 画面外に出ないよう横の移動制限
        if (Mathf.Abs(nextPosition.x) > PlayerPositionController.X_MOVE_RANGE)
        {
            nextPosition.x = (nextPosition.x > 0) ?
                PlayerPositionController.X_MOVE_RANGE : -PlayerPositionController.X_MOVE_RANGE;

            // 反転
            _defaultMoveRight = !_defaultMoveRight;
        }

        if((nextPosition.y - _defaultHeight) * (_previousPosition.y - _defaultHeight) < 0)
        {
            nextPosition.y = _defaultHeight;
        }

        transform.position = nextPosition;
    }

    /// <summary>
    /// 攻撃可能なら攻撃
    /// </summary>
    /// <returns>後ろの処理をスキップしたかったらtrue</returns>
    protected override bool StartAttackWaitTimeIfCan()
    {
        if (_attackArea.IsInArea)
        {
            if (Player.Instance != null && !Player.Instance.InAttacking)
            {
                _state = State.WaitAttack;
                _image.sprite = _imageArray[(int)State.Idle];
                return true;
            }
            else
            {
                _state = State.Idle;
                _image.sprite = _imageArray[(int)_state];
                return true;
            }
        }

        return false;
    }

    protected override void OnCreated()
    {
        // はじめ右に行くか左に行くかを乱数で生成
        _defaultMoveRight = Random.Range(0, 2) == 0;
        _defaultHeight = Random.Range(50f, 150f);

        _state = State.Default;
        _image.sprite = _imageArray[(int)State.Idle];
    }

    protected override void OnStartDefaultMotion()
    {
        _state = State.Default;
        _image.sprite = _imageArray[(int)State.Idle];
    }

    protected override void OnStartChaseMotion()
    {
        _state = State.Move;
        _image.sprite = _imageArray[(int)State.Idle];
    }


    /// <summary>
    /// いまのモーションに残り時間がまだあるときのUpdate処理
    /// </summary>
    /// <returns> 残り時間を無視して次のモーションを抽選する必要があればtrue </returns>
    public override bool UpdateExistsRemainMotionTime()
    {
        // 被ダメor攻撃中or攻撃待機中 以外でプレイヤーが攻撃できる領域にいる場合は強制で次の状態抽選を走らせる
        if (_state != State.Damaging && _state != State.Attack &&  _state != State.WaitAttack && _attackArea.IsInArea)
        {
            return true;
        }

        return false;
    }
}
