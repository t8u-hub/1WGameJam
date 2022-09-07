using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラ位置コントローラー　キャラクターにアタッチする
/// </summary>
/// 
public class CharacterPositionController : MonoBehaviour
{
    [SerializeField, Range(0f, 5.0f)]
    private float _velocity = 1f;

    [SerializeField, Range(10000f, 30000.0f)]
    private float _jumpPower = 1f;

    [SerializeField]
    private Transform _imageTransform;

    private static readonly Vector3 LEFT = Vector3.left;
    private static readonly Vector3 RIGHT = Vector3.right;
    private static readonly Vector3 ZERO = Vector3.zero;
    private static readonly Vector3 UP = Vector3.up;

    public static float X_MOVE_RANGE = 400;

    private float _groundHeight;

    public static readonly string GROUND = "Ground";
    private bool _isGround = false;

    private int _maxJumpNum = 2;
    private int _currentJumpNum = 0;

    private static readonly Vector3 GRAVITY_ACCELERATION = new Vector3(0f, -9.8f, 0f) * 80;
    private Vector3 _previousPosition;
    private Vector3 _acceleration = Vector3.zero;
    private Vector3 _speed = Vector3.zero;

    bool _moveRight = false;
    bool _moveLeft = false;
    bool _jump = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        _previousPosition = transform.position;
        var nextPosition = _previousPosition;

        _acceleration = ZERO;

        // ジャンプ加速度
        if (_jump)
        {
            _jump = false;
            _isGround = false;
            _acceleration += Vector3.up * _jumpPower;
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
            _imageTransform.transform.localScale = new Vector3(1, 1, 1);
        }

        // キー入力による右移動
        if (_moveRight)
        {
            nextPosition += RIGHT * _velocity;
            _imageTransform.transform.localScale = new Vector3(-1, 1, 1);
        }

        // 画面外に出ないよう横の移動制限
        if (Mathf.Abs(nextPosition.x) > X_MOVE_RANGE)
        {
            nextPosition.x = (nextPosition.x > 0) ? X_MOVE_RANGE : -X_MOVE_RANGE;
        }

        transform.position = nextPosition;
    }

    public void Update()
    {
        // ジャンプ加速度
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            _jump = true;
        }

        _moveLeft = Input.GetKey(KeyCode.LeftArrow);
        _moveRight = Input.GetKey(KeyCode.RightArrow);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == GROUND)
        {
            _isGround = true;
            var pos = this.transform.position;
            pos.y = collision.transform.position.y;
            this.transform.position = pos;
        }
    }
}
