using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : Enemy
{
    [SerializeField]
    private Animation _defaultAnimation;

    public override void FixedUpdateInner()
    {
        if (_hp <= 0)
        {
            return;
        }

        _previousPosition = transform.position;
        var nextPosition = _previousPosition;

        if (_state == State.Knockback)
        {
            var knockBackRight = transform.position.x > _targetTransform.position.x;
            _speed = knockBackRight ? KNOCK_BACK_SPEED : -KNOCK_BACK_SPEED;

            _state = State.Damaging;
            _remainMoveTime = .5f;
        }
        else
        {
            _acceleration = ZERO;
        }


        // ダメージを受けてる間は横のスピードを経過時間で制御
        if (_state == State.Damaging)
        {
            var temp = _speed;
            temp.x = 0.9f * temp.x;
            _speed = temp;
        }

        // 物理演算による移動
        _speed = _speed + _acceleration * Time.fixedDeltaTime;
        nextPosition = nextPosition + _speed * Time.fixedDeltaTime;

        // 左移動
        if (_moveLeft)
        {
        }

        // 右移動
        if (_moveRight)
        {
        }

        nextPosition += Vector3.one * _parameter.MoveSpeed;
        _imageTransform.transform.localScale = new Vector3(1, 1, 1);

        // 画面外に出ないよう横の移動制限
        if (Mathf.Abs(nextPosition.x) > PlayerPositionController.X_MOVE_RANGE)
        {
            nextPosition.x = (nextPosition.x > 0) ?
                PlayerPositionController.X_MOVE_RANGE : -PlayerPositionController.X_MOVE_RANGE;
        }

        transform.position = nextPosition;
    }
}
