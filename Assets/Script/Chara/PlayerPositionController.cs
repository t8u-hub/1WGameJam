using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラ位置コントローラー　キャラクターにアタッチする
/// </summary>
/// 
public class PlayerPositionController : MonoBehaviour
{
    enum State
    {
        Default,
        HorizontalMoveAttack,
        VerticalMoveAttack,
    }

    [SerializeField, Range(0f, 5.0f)]
    private float _velocity = 1f;

    [SerializeField, Range(10000f, 30000.0f)]
    private float _jumpPower = 1f;

    [SerializeField]
    private Transform _imageTransform;

    private static readonly Vector3 GRAVITY_ACCELERATION = new Vector3(0f, -9.8f, 0f) * 80;
    private static readonly Vector3 LEFT = Vector3.left;
    private static readonly Vector3 RIGHT = Vector3.right;
    private static readonly Vector3 ZERO = Vector3.zero;
    private static readonly Vector3 UP = Vector3.up;
    public static readonly string GROUND = "Ground";
    public static float X_MOVE_RANGE = 400;

    private State _state = State.Default;

    private Vector3 _previousPosition;
    private Vector3 _acceleration = Vector3.zero;
    private Vector3 _speed = Vector3.zero;

    bool _moveRight = false;
    bool _moveLeft = false;
    bool _jump = false;

    private bool _isGround = false;
    public bool IsGround => _isGround;

    public bool IsMove => _moveLeft || _moveRight;

    /// <summary> 攻撃の移動時間（横移動攻撃で使用） </summary>
    private float _attackMoveTime = 0f;

    /// <summary> 着地コールバック（盾移動攻撃で使用） </summary>
    private System.Action _callback = null;

    // Update is called once per frame
    void FixedUpdate()
    {
        _previousPosition = transform.position;
        var nextPosition = _previousPosition;

        if (_state == State.HorizontalMoveAttack)
        {
            if (_attackMoveTime > 0f)
            {
                nextPosition = nextPosition + _speed * Time.fixedDeltaTime;

                // 画面外に出ないよう横の移動制限
                if (Mathf.Abs(nextPosition.x) > X_MOVE_RANGE)
                {
                    nextPosition.x = (nextPosition.x > 0) ? X_MOVE_RANGE : -X_MOVE_RANGE;
                }

                transform.position = nextPosition;
                _attackMoveTime -= Time.fixedDeltaTime;
                return;
            }
            else
            {
                _callback?.Invoke();
                _state = State.Default;
                _speed = ZERO;
            }
        }

        if (_state == State.VerticalMoveAttack)
        {
            nextPosition = nextPosition + _speed * Time.fixedDeltaTime;
            transform.position = nextPosition;
            _attackMoveTime -= Time.fixedDeltaTime;
            return;
        }

        _acceleration = ZERO;

        // ジャンプ加速度
        if (_jump)
        {
            _jump = false;

            if (_isGround)
            {
                _isGround = false;
                _acceleration += Vector3.up * _jumpPower;
            }
        }

        // 重力加速度の適用
        var isAppryGravity = !_isGround;
        if (isAppryGravity)
        {
            _acceleration += GRAVITY_ACCELERATION;
        }
        else
        {
            // 重力を適用しないときはそもそも下に落ちないようにする
            _speed.y = 0;
        }

        // 物理演算による移動
        _speed = _speed + _acceleration * Time.fixedDeltaTime;
        nextPosition = nextPosition + _speed * Time.fixedDeltaTime;

        // キー入力による左移動
        if (_moveLeft)
        {
            nextPosition += LEFT * _velocity;
            _imageTransform.transform.localScale = Player.IMG_DEFAULT;
        }

        // キー入力による右移動
        if (_moveRight)
        {
            nextPosition += RIGHT * _velocity;
            _imageTransform.transform.localScale = Player.IMG_FLIP;
        }

        // 画面外に出ないよう横の移動制限
        if (Mathf.Abs(nextPosition.x) > X_MOVE_RANGE)
        {
            nextPosition.x = (nextPosition.x > 0) ? X_MOVE_RANGE : -X_MOVE_RANGE;
        }

        transform.position = nextPosition;
    }

    public void SetPlayerMove(bool moveRight, bool moveLeft, bool jump)
    {
        _moveLeft = moveLeft;
        _moveRight = moveRight;

        if (!_jump)
        {
            _jump = jump;
        }

        if ((_moveLeft || _moveRight || _jump) && _state != State.Default)
        {
            _callback?.Invoke();
            _state = State.Default;
            _speed = ZERO;
        }
    }

    public void DoHorizontalMoveAttack(float time, float speed, System.Action stopCallback)
    {
        _callback = stopCallback;
        _attackMoveTime = time;
        _speed = _imageTransform.localScale.x > 0 ? Vector3.left : Vector3.right;
        _speed *= speed;
        _state = State.HorizontalMoveAttack;
    }

    public void DoVerticalMoveAttack(float speed, System.Action groundCallback)
    {
        _callback = groundCallback;
        _state = State.VerticalMoveAttack;
        _speed = Vector3.down * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnterOrStay(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnterOrStay(collision);
    }

    private void OnTriggerEnterOrStay(Collider2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == GROUND)
        {
            _isGround = true;
            var pos = this.transform.position;
            pos.y = collision.transform.position.y;
            this.transform.position = pos;

            if (_state == State.VerticalMoveAttack)
            {
                _callback?.Invoke();
                _callback = null;
                _speed = ZERO;
                _state = State.Default;
            }
        }
    }

    public void ResetVerticalSpeed()
    {
        _speed.y = _speed.y > 0f ? 0f : _speed.y;
    }

    public void SetPlayerDirection(bool right)
    {
        _imageTransform.localScale = right ? Player.IMG_FLIP : Player.IMG_DEFAULT;
    }

}
