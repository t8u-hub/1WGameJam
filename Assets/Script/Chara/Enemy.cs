using UnityEngine;


public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform _imageTransform;

    [SerializeField]
    private DamageText _damageText;

    [SerializeField]
    private Animation _destroyAnimation;

    private static readonly Vector3 LEFT = Vector3.left;
    private static readonly Vector3 RIGHT = Vector3.right;
    private static readonly Vector3 ZERO = Vector3.zero;
    private static readonly Vector3 UP = Vector3.up;

    private static readonly Vector3 GRAVITY_ACCELERATION = new Vector3(0f, -9.8f, 0f) * 80;
    private Vector3 _previousPosition;
    private Vector3 _acceleration = Vector3.zero;
    private Vector3 _speed = Vector3.zero;

    public int Id => _parameter.Id;
    public int DropItemId => _parameter.DropItemId;

    bool _moveRight = false;
    bool _moveLeft = false;
    bool _chase = false;
    bool _canAttack = false;

    // スポーンした直後だけ宙に浮いている想定
    bool _isGround = false;

    private Transform _targetTransform;

    private Parameter _parameter;

    private int _hp;

    /// <summary>
    /// 現在のモーションを続ける時間
    /// </summary>
    private float _remainMoveTime = 0f;

    public class Parameter
    {
        public int Id;
        public int HitPoint;
        public float MoveSpeed;
        public float AttackPower;
        public int DropItemId;
    }

    public static Enemy CreateObject(Enemy prefab, Parameter paramter, Transform parentTransform)
    {
        var enemyManager = GameObject.Instantiate<Enemy>(prefab, parentTransform);
        enemyManager._parameter = paramter;
        enemyManager.transform.localPosition = Vector3.zero;
        enemyManager._hp = paramter.HitPoint;

        if (paramter.DropItemId != 0)
        {
            // アイテムをドロップする敵は気持ちサイズを大きくする仮処理
            var rectTransform = enemyManager.transform as RectTransform;
            rectTransform.sizeDelta = rectTransform.sizeDelta * 1.5f;
        }

        return enemyManager;
    }

    /// <summary>
    /// 移動の目的地Transformを設定
    /// </summary>
    public void SetTargetTransform(Transform transform)
    {
        _targetTransform = transform;
    }

    public void Update()
    {
        if (_remainMoveTime > 0f)
        {
            // 現在のモーションを続ける場合は時間を更新
            _remainMoveTime = _remainMoveTime - Time.deltaTime;

            // ターゲット追いかけ中のときは方向も更新
            if (_chase)
            {
                var isTargetRight = transform.position.x < _targetTransform.position.x;
                _moveRight = isTargetRight;
                _moveLeft = !isTargetRight;
            }

            return;
        }

        _remainMoveTime = 1f;
        _chase = false;
        _moveLeft = false;
        _moveRight = false;

        if (_canAttack)
        {
            // 攻撃する
            Debug.Log("こうげき");
            return;
        }

        // TODO: 確率と時間はCSVから読む
        var rand = Random.Range(0, 100);
        if (rand < 50)
        {
            // 停止 or 旋回処理

        }
        else
        {
            // プレイヤーに近寄る
            _chase = true;
        }

    }

    public void FixedUpdate()
    {
        if (_hp <= 0)
        {
            return;
        }

        _previousPosition = transform.position;
        var nextPosition = _previousPosition;

        _acceleration = ZERO;


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

        // 左移動
        if (_moveLeft)
        {
            nextPosition += LEFT * _parameter.MoveSpeed;
            _imageTransform.transform.localScale = new Vector3(1, 1, 1);
        }

        // 右移動
        if (_moveRight)
        {
            nextPosition += RIGHT * _parameter.MoveSpeed;
            _imageTransform.transform.localScale = new Vector3(-1, 1, 1);
        }

        // 画面外に出ないよう横の移動制限
        if (Mathf.Abs(nextPosition.x) > PlayerPositionController.X_MOVE_RANGE)
        {
            nextPosition.x = (nextPosition.x > 0) ?
                PlayerPositionController.X_MOVE_RANGE : -PlayerPositionController.X_MOVE_RANGE;
        }

        transform.position = nextPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == PlayerPositionController.GROUND)
        {
            _isGround = true;
            var pos = this.transform.position;
            pos.y = collision.transform.position.y;
            this.transform.position = pos;
        }
    }

    public void OnDamage(int hitCount, int amount)
    {
        _hp -= amount * hitCount;
        for (int i = 0; i < hitCount; i++)
        {
            var damageText = GameObject.Instantiate<DamageText>(_damageText, transform);
            damageText.PlayDamage(amount);
        }

        if (_hp <= 0)
        {

            _destroyAnimation.Play();
        }
    }
    
    public void OnDeadAnimationEnd()
    {
        BattleManager.Instance.OnEnemyKilled(this);
        Destroy(this.gameObject);
    }
}
