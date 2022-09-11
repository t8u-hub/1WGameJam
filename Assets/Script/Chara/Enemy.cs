using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    protected enum State
    {
        Idle,    // 静止
        Attack,     // 攻撃
        Damaging,     // 被ダメ
        Move,       // プレイヤーに向かって動く

        Default,       // 巡回 絵はIdleと一緒
        Knockback,    // ノックバック（被ダメ開始後１Fだけなる）絵はdamagingと一緒
        WaitAttack,     // 攻撃待機
    }

    /// <summary>
    /// 基本Stateの順で配列に登録すること
    /// </summary>
    [SerializeField]
    protected Sprite[] _imageArray;

    [SerializeField]
    protected Transform _imageTransform;

    [SerializeField]
    private DamageTextPlayer _damageText;

    [SerializeField]
    private Animation _destroyAnimation;

    [SerializeField]
    protected EnemyAttackArea _attackArea;

    [SerializeField]
    protected Image _image;

    private static readonly Vector3 LEFT = Vector3.left;
    private static readonly Vector3 RIGHT = Vector3.right;
    protected static readonly Vector3 ZERO = Vector3.zero;
    private static readonly Vector3 UP = Vector3.up;

    protected Vector3 IMG_DEFAULT = new Vector3(1, 1, 1);
    protected Vector3 IMG_FLIP = new Vector3(-1, 1, 1);

    private static readonly Vector3 GRAVITY_ACCELERATION = new Vector3(0f, -9.8f, 0f) * 80;
    private static readonly Vector3 KNOCK_BACK_SPEED = new Vector3(300f, 0, 0);
    protected Vector3 _previousPosition;
    protected Vector3 _acceleration = Vector3.zero;
    protected Vector3 _speed = Vector3.zero;

    public int Id => _parameter.Id;
    public int DropItemId => _parameter.DropItemId;

    private bool _moveRight = false;
    private bool _moveLeft = false;
    bool _chase = false;

    // スポーンした直後だけ宙に浮いている想定
    private bool _isGround = false;

    protected Transform _targetTransform;

    protected Parameter _parameter;

    protected int _hp;

    protected State _state;

    /// <summary>
    /// 現在のモーションを続ける時間
    /// </summary>
    protected float _remainMoveTime = 0f;

    public class Parameter
    {
        public int Id;
        public int HitPoint;
        public float MoveSpeed;
        public float AttackPower;
        public int DropItemId;

        public int MoveWeight;
        public int StopWeight;

        public float WaitTime;
    }

    public static Enemy CreateObject(Enemy prefab, Parameter paramter, Transform parentTransform)
    {
        var enemyManager = GameObject.Instantiate<Enemy>(prefab, parentTransform);
        enemyManager._parameter = paramter;
        enemyManager.transform.localPosition = Vector3.zero;
        enemyManager._hp = paramter.HitPoint;

        enemyManager.OnCreated();
        return enemyManager;
    }

    /// <summary>
    /// 移動の目的地Transformを設定
    /// </summary>
    public void SetTargetTransform(Transform transform)
    {
        _targetTransform = transform;
    }

    /// <summary>
    /// いまのモーションに残り時間がまだあるときのUpdate処理
    /// </summary>
    /// <returns> 残り時間を無視して次のモーションを抽選する必要があればtrue </returns>
    public virtual bool UpdateExistsRemainMotionTime()
    {
        // ターゲット追いかけ中のときは方向も更新
        if (_chase)
        {
            var isTargetRight = transform.position.x < _targetTransform.position.x;
            _moveRight = isTargetRight;
            _moveLeft = !isTargetRight;
        }

        // 被ダメ中or攻撃中or攻撃待機中　あるいはプレイヤーが攻撃できない領域にいるならこのあとは特になにもしない
        // 逆に、被ダメor攻撃中以外でプレイヤーが攻撃できる領域にいる場合は強制で次の状態抽選を走らせる
        if (!_isGround || _state == State.Damaging || _state == State.Attack || _state == State.WaitAttack || !_attackArea.IsInArea)
        {
            return false;
        }

        return true;
    }

    public void Update()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.StopUpdate)
        {
            return;
        }

        if (_state == State.Knockback)
        {
            // ノックバック中は状態更新が何もできないようにする
            _chase = false;
            _moveLeft = false;
            _moveRight = false;
            return;
        }

        if (_remainMoveTime > 0f)
        {
            // 現在のモーションを続ける場合は時間を更新
            _remainMoveTime = _remainMoveTime - Time.deltaTime;

            if (!UpdateExistsRemainMotionTime())
            {
                return;
            }
        }

        _remainMoveTime = 1f;
        _chase = false;
        _moveLeft = false;
        _moveRight = false;
        _speed = ZERO;

        // 次の状態を抽選

        // 攻撃待機状態が満了したら攻撃開始
        if (_state == State.WaitAttack)
        {
            Attack();
            return;
        }

        // 攻撃可能だったら攻撃待機状態へ
        if (StartAttackWaitTimeIfCan())
        {
            return;
        }

        // TODO: 確率と時間はCSVから読む
        var rand = Random.Range(0, _parameter.MoveWeight + _parameter.StopWeight);
        if (rand < _parameter.StopWeight)
        {
            // 停止 or 旋回処理
            OnStartDefaultMotion();
        }
        else
        {
            // プレイヤーに近寄る
            _chase = true;
            OnStartChaseMotion();
        }

    }

    protected virtual void OnStartDefaultMotion()
    {
        _state = State.Idle;
        _image.sprite = _imageArray[(int)_state];
    }


    protected virtual void OnStartChaseMotion()
    {
        _state = State.Move;
        _image.sprite = _imageArray[(int)_state];
    }

    protected virtual void OnCreated()
    {
        _state = State.Idle;
        _image.sprite = _imageArray[(int)_state];
    }

    protected virtual void Attack()
    {
        _state = State.Attack;
        _image.sprite = _imageArray[(int)_state];
        BattleManager.Instance.EnemyAttack(_parameter.AttackPower, transform.position.x);
    }

    /// <summary>
    /// 攻撃可能なら攻撃
    /// </summary>
    /// <returns>後ろの処理をスキップしたかったらtrue</returns>
    protected virtual bool StartAttackWaitTimeIfCan()
    {
        if (_attackArea.IsInArea)
        {
            // デフォ敵は着地していないと攻撃できない
            if (_isGround && Player.Instance != null && !Player.Instance.InAttacking)
            {
                _state = State.WaitAttack;
                _image.sprite = _imageArray[(int)State.Move];
                _remainMoveTime = _parameter.WaitTime;
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

    public void FixedUpdate()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.StopUpdate)
        {
            return;
        }

        FixedUpdateInner();
    }

    public virtual void FixedUpdateInner()
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
            nextPosition += LEFT * _parameter.MoveSpeed;
            _imageTransform.transform.localScale = IMG_DEFAULT;
        }

        // 右移動
        if (_moveRight)
        {
            nextPosition += RIGHT * _parameter.MoveSpeed;
            _imageTransform.transform.localScale = IMG_FLIP;
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

    public void OnDamage(int hitCount, int amount, bool isSpecial = false)
    {
        _hp -= amount * hitCount;

        var damageText = GameObject.Instantiate<DamageTextPlayer>(_damageText, transform);
        damageText.PlayDamageTextAnimation(amount, hitCount, _imageTransform.transform.localScale.x < 0);

        SeAudioManager.Instance.Play(SeAudioManager.SeType.EnemyDamage);

        // 与えたダメージ量だけゲージ上昇
        BattleManager.Instance.PlayerAttack(hitCount * amount, isSpecial);
        if (_hp <= 0)
        {
            _destroyAnimation.Play();
            _state = State.Damaging; // 死ぬ場合はノックバックしない
        }
        else
        {
            _state = State.Knockback;
        }

        _image.sprite = _imageArray[(int)State.Damaging];
    }

    public void OnDeadAnimationEnd()
    {
        BattleManager.Instance.OnEnemyKilled(this);
        Destroy(this.gameObject);
    }
}
