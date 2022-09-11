using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoSingleton<Player>
{
    public static readonly Vector3 IMG_DEFAULT = new Vector3(1, 1, 1);
    public static readonly Vector3 IMG_FLIP = new Vector3(-1, 1, 1);

    #region Class, enum

    class Weapon
    {
        public int Id;
        public int Type;
        public int Level;
        public int Damage;
        public int Hit;
    }

    class Attack
    {
        public CsvDefine.ActionData.AttackType AttackType { get; }
        public float RecastTime { get; }
        public bool CanJump { get; }
        public bool CanMove { get; }
        public bool CanNormalAttack { get; }
        public bool CanHorizontalMoveAttack { get; }
        public bool CanMiddleDistanceAttack { get; }
        public bool CanLongRangeAttack { get; }
        public bool CanVerticalMoveAttack { get; }

        public Attack(Dictionary<string, int> master)
        {
            AttackType = (CsvDefine.ActionData.AttackType)master[CsvDefine.ActionData.ATTACK_TYPE];
            RecastTime = master[CsvDefine.ActionData.RECAST_TIME] / CsvDefine.INT2FLOAT;
            CanJump = master[CsvDefine.ActionData.CAN_JUMP] == 1;
            CanMove = master[CsvDefine.ActionData.CAN_MOVE] == 1;
            CanNormalAttack = master[CsvDefine.ActionData.CAN_ACTION_1] == 1;
            CanHorizontalMoveAttack = master[CsvDefine.ActionData.CAN_ACTION_2] == 1;
            CanMiddleDistanceAttack = master[CsvDefine.ActionData.CAN_ACTION_3] == 1;
            CanLongRangeAttack = master[CsvDefine.ActionData.CAN_ACTION_4] == 1;
            CanVerticalMoveAttack = master[CsvDefine.ActionData.CAN_ACTION_5] == 1;
        }
    }

    // Note: 2段ジャンプ防止はPlayerPositionController側
    // ここの判定は「入力を受け付けるか」
    class ActionEnableState
    {
        /// <summary> そのモーションが解放されているか(未実装) </summary>
        private bool _isReleased = false;

        /// <summary> リキャストタイムか </summary>
        private bool _isInRecastTime;

        /// <summary> ほかのモーションの制約にひっかかっていないか </summary>
        private bool _isRestriction;

        public bool CanExec => _isReleased && !_isInRecastTime && !_isRestriction;

        /// <summary>
        /// 他モーションとのかねあいの制約を設定する
        /// </summary>
        public void SetRestriction(bool isRestriction)
        {
            _isRestriction = isRestriction;
        }

        /// <summary>
        /// リキャストを開始する
        /// </summary>
        public void StartRecastTime(float time)
        {
            _isInRecastTime = true;
            UIManager.Instance.StartCoroutine(StartRecastTimeImpl(time));
        }

        private IEnumerator StartRecastTimeImpl(float time)
        {
            yield return new WaitForSeconds(time);
            _isInRecastTime = false;
        }

        public void ReleaseAction()
        {
            _isReleased = true;
        }
    }

    private enum ActionType
    {
        Move,
        Jump,
        NormalAttack,
        HorizontalMoveAttack,
        MiddleDistanceAttack,
        LongRangeAttack,
        VerticalMoveAttack,
    }

    enum ActionSpriteState
    {
        Stop,
        Move,
        Jump,
        Attack,
        Damage,
    }

    #endregion

    private Dictionary<ActionType, ActionEnableState> _actionEnableStateDict = new Dictionary<ActionType, ActionEnableState>()
    {
        {ActionType.Move, new ActionEnableState() },
        {ActionType.Jump, new ActionEnableState() },
        {ActionType.NormalAttack, new ActionEnableState() },
        {ActionType.HorizontalMoveAttack, new ActionEnableState() },
        {ActionType.MiddleDistanceAttack, new ActionEnableState() },
        {ActionType.LongRangeAttack, new ActionEnableState() },
        {ActionType.VerticalMoveAttack, new ActionEnableState() },
    };

    [SerializeField]
    private CharacterAttackSetting _attackSetting;

    /// <summary>
    /// 見た目制御系
    /// </summary>
    [SerializeField]
    PlayerSpriteDataSetting _spriteSettingData;

    [SerializeField]
    private Image _image;


    /// <summary>
    /// 動作制御系
    /// </summary>
    [SerializeField]
    private PlayerPositionController _playerPositionController;

    [SerializeField]
    private PlayerAttack _playerAttack;

    // マスターデータ
    private List<Weapon> _weaponList;
    private List<Attack> _attackList;

    // 所持アイテムデータ
    private Dictionary<CsvDefine.ActionData.AttackType, Weapon> _equipWeaponDict;

    /// <summary>
    /// 今の攻撃モーションの残り時間
    /// </summary>
    private float _attackMotionTime = 0f;

    private ActionSpriteState _spriteState = ActionSpriteState.Stop;

    // 必殺技使用判定用ぱらめた

    // 長押し判定の有効時間
    private float _spaceLongTapTime = 0f;

    /// <summary>
    /// スペースキーを使ってジャンプ中ならtrue
    /// </summary>
    private bool _jumpWithSpace = false;

    //////////////

    /// <summary>
    /// 被ダメ時などの無敵時間
    /// </summary>
    private float _noDamageTime = 0f;

    /// <summary>
    /// レベル
    /// </summary>
    private int _charaLevel = 1;
    public int CharaLevel => _charaLevel;

    public bool InAttacking
    {
        get
        {
            return _attackMotionTime > 0f;
        }
    }

    public bool InNoDamageTime => _noDamageTime > 0f;

    public void LevelUp()
    {
        if (_charaLevel < 3)
        {
            _charaLevel += 1;
        }

        ResetSprite();
    }

    private void Awake()
    {
        var weaponCsv = new CsvReader().Create(CsvDefine.WeaponData.PATH).CsvData;
        _weaponList = weaponCsv.Select(csvData => new Weapon
        {
            Id = csvData[CsvDefine.WeaponData.WEAPON_ID],
            Type = csvData[CsvDefine.WeaponData.ATTACK_TYPE],
            Level = csvData[CsvDefine.WeaponData.WEAPON_LEVEL],
            Damage = csvData[CsvDefine.WeaponData.DAMAGE],
            Hit = csvData[CsvDefine.WeaponData.HIT],
        }).ToList();

        var actionCsv = new CsvReader().Create(CsvDefine.ActionData.PATH).CsvData;
        _attackList = actionCsv.Select(csvData => new Attack(csvData)).ToList();

        _equipWeaponDict = new Dictionary<CsvDefine.ActionData.AttackType, Weapon>()
        {
            // 武器ID101は初期装備
            {CsvDefine.ActionData.AttackType.Normal, _weaponList.Find(weapon => weapon.Id == 101) },
            {CsvDefine.ActionData.AttackType.HorizontalMove, null },
            {CsvDefine.ActionData.AttackType.MiddleDistance, null },
            {CsvDefine.ActionData.AttackType.LongRange, null },
            {CsvDefine.ActionData.AttackType.VerticalMove, null },
        };

        // 移動、ジャンプ、通常攻撃は最初からできる
        _actionEnableStateDict[ActionType.Move].ReleaseAction();
        _actionEnableStateDict[ActionType.Jump].ReleaseAction();
        _actionEnableStateDict[ActionType.NormalAttack].ReleaseAction();

    }

    private void SetActionEnableState(CsvDefine.ActionData.AttackType attackType, ActionType actionType)
    {
        var attack = _attackList.Find(attack => attack.AttackType == attackType);

        _actionEnableStateDict[ActionType.Jump].SetRestriction(!attack.CanJump);
        _actionEnableStateDict[ActionType.Move].SetRestriction(!attack.CanMove);
        _actionEnableStateDict[ActionType.NormalAttack].SetRestriction(!attack.CanNormalAttack);
        _actionEnableStateDict[ActionType.HorizontalMoveAttack].SetRestriction(!attack.CanHorizontalMoveAttack);
        _actionEnableStateDict[ActionType.MiddleDistanceAttack].SetRestriction(!attack.CanMiddleDistanceAttack);
        _actionEnableStateDict[ActionType.LongRangeAttack].SetRestriction(!attack.CanLongRangeAttack);
        _actionEnableStateDict[ActionType.VerticalMoveAttack].SetRestriction(!attack.CanVerticalMoveAttack);

        _actionEnableStateDict[actionType].StartRecastTime(attack.RecastTime);
    }

    private void SetAllActionRestrictionState(bool restrict)
    {
        foreach (var actionEnableState in _actionEnableStateDict)
        {
            actionEnableState.Value.SetRestriction(restrict);
        }
    }

    void Update()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.StopUpdate)
        {
            return;
        }

#if UNITY_EDITOR

        Debug_ReleaseItem();

#endif

        if (_weaponList == null || _attackList == null)
        {
            return;
        }

        if (_noDamageTime > 0f)
        {
            // NOTE：無敵が1秒前提のハードコーディング
            if (_noDamageTime > (_attackSetting.NoDamageTime - _attackSetting.RighdityTime))
            {
                _noDamageTime -= Time.deltaTime;
                if (_noDamageTime < (_attackSetting.NoDamageTime - _attackSetting.RighdityTime))
                {
                    //  硬直解除
                    SetAllActionRestrictionState(false);

                    // 画像を被弾状態から戻す
                    ResetSprite();
                }
                else
                {
                    return;
                }
            }
            else
            {
                _noDamageTime -= Time.deltaTime;
            }
        }


        if (_attackMotionTime > 0f)
        {
            _attackMotionTime -= Time.deltaTime;

            if (_attackMotionTime <= 0f)
            {
                // すべてのモーションの他アニメーションによる制約を削除
                SetAllActionRestrictionState(false);
            }
        }


        // 攻撃
        if (_actionEnableStateDict[ActionType.NormalAttack].CanExec && Input.GetKeyDown(KeyCode.Z))
        {
            // 通常攻撃
            var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.Normal];
            if (weapon != null) 
            {
                // 画像を攻撃状態へ
                _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Attack, _charaLevel, 1);
                _image.transform.localScale = IMG_DEFAULT;
                _spriteState = ActionSpriteState.Attack;

                var info = _attackSetting.GetNormalAttackInfo();
                _attackMotionTime = info.RigidityTime;
                _playerAttack.AttackNormal(weapon.Hit, weapon.Damage, info.HitArea);
                SetActionEnableState(CsvDefine.ActionData.AttackType.Normal, ActionType.NormalAttack);

                SeAudioManager.Instance.Play(SeAudioManager.SeType.Attack);
            }
        }
        else if (_actionEnableStateDict[ActionType.HorizontalMoveAttack].CanExec && Input.GetKeyDown(KeyCode.X))
        {
            // 移動攻撃
            var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.HorizontalMove];
            if (weapon != null)
            {
                // 画像を攻撃状態へ
                _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Attack, _charaLevel, 2);
                _spriteState = ActionSpriteState.Attack;
                _image.transform.localScale = IMG_DEFAULT;

                var info = _attackSetting.GetHorizontalMoveAttackInfo();
                _attackMotionTime = info.MaxLastTime ;
                SetActionEnableState(CsvDefine.ActionData.AttackType.HorizontalMove, ActionType.HorizontalMoveAttack);
                _playerAttack.AttackHorizontalMove(weapon.Hit, weapon.Damage, info.HitArea, info.MaxLastTime);

                SeAudioManager.Instance.Play(SeAudioManager.SeType.Horizontal);

                _playerPositionController.DoHorizontalMoveAttack(info.MaxLastTime, info.MoveSpeed, () =>
                {
                    if (_attackMotionTime > 0)
                    {
                        _attackMotionTime = 0f;
                        SetAllActionRestrictionState(false);
                        _playerAttack.FinishAttack();
                    }
                });
            }
        }
        else if (_actionEnableStateDict[ActionType.MiddleDistanceAttack].CanExec && Input.GetKeyDown(KeyCode.C))
        {
            // 遠距離投擲攻撃
            var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.MiddleDistance];
            if (weapon != null)
            {
                // 画像を攻撃状態へ
                _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Attack, _charaLevel, 3);
                _image.transform.localScale = IMG_DEFAULT;
                _spriteState = ActionSpriteState.Attack;

                var info = _attackSetting.GetMiddleDistanceAttackInfo();
                _attackMotionTime = info.RigidityTime;
                _playerAttack.AttackMiddleDistance(weapon.Hit, weapon.Damage, transform.parent, info.HitArea, info.ThrowSpeed, info.ThrowPich);
                SetActionEnableState(CsvDefine.ActionData.AttackType.MiddleDistance, ActionType.MiddleDistanceAttack);

                SeAudioManager.Instance.Play(SeAudioManager.SeType.Middle);
            }
        }
        else if (_actionEnableStateDict[ActionType.LongRangeAttack].CanExec && Input.GetKeyDown(KeyCode.V))
        {
            // 広範囲攻撃
            var info = _attackSetting.GetLongRangeAttackInfo();
            var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.LongRange];
            if (weapon != null)
            {
                // 画像を攻撃状態へ
                _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Attack, _charaLevel, 4);
                _image.transform.localScale = IMG_FLIP;
                _spriteState = ActionSpriteState.Attack;

                _attackMotionTime = info.RigidityTime;
                _playerAttack.AttackLongRange(weapon.Hit, weapon.Damage, info.HitArea);
                SetActionEnableState(CsvDefine.ActionData.AttackType.LongRange, ActionType.LongRangeAttack);

                SeAudioManager.Instance.Play(SeAudioManager.SeType.Long);
            }
        }
        else if (_actionEnableStateDict[ActionType.VerticalMoveAttack].CanExec && Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 降下攻撃できるのは滞空中だけ
            if (!_playerPositionController.IsGround)
            {
                // 広範囲攻撃
                var info = _attackSetting.GetVerticalMoveAttackInfo();
                var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.VerticalMove];
                if (weapon != null)
                {
                    // 画像を攻撃状態へ
                    _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Attack, _charaLevel, 5);
                    _image.transform.localScale = IMG_DEFAULT;
                    _spriteState = ActionSpriteState.Attack;

                    SeAudioManager.Instance.Play(SeAudioManager.SeType.VerticalStart);

                    SetActionEnableState(CsvDefine.ActionData.AttackType.VerticalMove, ActionType.VerticalMoveAttack);
                    // 100秒降下し続けることはないと思うので...という最悪なコード
                    _attackMotionTime = 100f;
                    _playerPositionController.DoVerticalMoveAttack(info.MoveSpeed, () =>
                    {
                        SeAudioManager.Instance.Play(SeAudioManager.SeType.VerticalEnd);
                        _playerAttack.AttackVerticalMove(weapon.Hit, weapon.Damage, info.HitArea);
                        // 着地してから固定秒数つづく
                        _attackMotionTime = info.RigidityTime;
                    });
                }
            }
        }

        // 通常モーション
        var moveRight = _actionEnableStateDict[ActionType.Move].CanExec && Input.GetKey(KeyCode.RightArrow);
        var moveLeft = _actionEnableStateDict[ActionType.Move].CanExec && Input.GetKey(KeyCode.LeftArrow);
        var jump = _actionEnableStateDict[ActionType.Jump].CanExec &&
                        (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space));
        if (moveRight || moveLeft || jump)
        {
            // どれか一つでも入力が許容されてたら前の攻撃モーションは終わり（リキャストタイム以外の制約は消える）
            _attackMotionTime = 0f;
            SetAllActionRestrictionState(false);
            _playerAttack.FinishAttack();
        }

        _playerPositionController.SetPlayerMove(moveRight, moveLeft, jump);

        // 必殺技
        if (EnableSpecialAttack())
        {
            BattleManager.Instance.SpecialAttack();
        }

        if (_attackMotionTime <= 0f)
        {
            ResetSprite();
        }
    }

    /// <summary>
    /// 必殺技を使うかどうか
    /// </summary>
    private bool EnableSpecialAttack()
    {
        // スペースキー押下のジャンプ中か判定
        if (!_jumpWithSpace)
        {
            if (BattleManager.Instance.CanUseSpecialAttack &&   // 必殺技が使える
                _actionEnableStateDict[ActionType.Jump].CanExec && // 他との兼ね合い的にジャンプ可能
                Input.GetKeyDown(KeyCode.Space) && // スペースキー押下
                _playerPositionController.IsGround) // 物理的にもキャラがジャンプ可能
            {
                _spaceLongTapTime = 0f;
                _jumpWithSpace = true;
                return false;
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Space) || // スペースキーが放される
                _attackMotionTime > 0 ||         // 何か別のモーションが始まる
                _noDamageTime > 0)               // ダメージを受けても解除
            {
                _jumpWithSpace = false;
            }
        }

        if (_jumpWithSpace)
        {
            _spaceLongTapTime += Time.deltaTime;
            if (_spaceLongTapTime > .5f)
            {
                _spaceLongTapTime = 0f;
                _jumpWithSpace = false;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// アイテム獲得処理
    /// </summary>
    /// <returns> レベルアップするか </returns>
    public bool GetItem(int itemId)
    {
        var weapon = _weaponList.Find(weapon => weapon.Id == itemId);
        if (weapon == null)
        {
            Debug.LogError($"存在しない武器ID{itemId}");
            return false;
        }

        var attackType = (CsvDefine.ActionData.AttackType)weapon.Type;
        var currentEquipWeapon = _equipWeaponDict[attackType];
        if (currentEquipWeapon == null || currentEquipWeapon?.Level < weapon.Level)
        {
            _equipWeaponDict[attackType] = weapon;
            // CsvDefine.ActionData.AttackType からActionTypeへキャストするため＋1する
            _actionEnableStateDict[(ActionType)weapon.Type + 1].ReleaseAction();

            var maxWeaponLevel = _equipWeaponDict.Select(data =>
                {
                    if (data.Value == null) return 0;
                    return data.Value.Level;
                })
                .OrderByDescending(item => item)
                .First();

            // 最大の武器LvがいまのキャラLvより大きければレベルアップ
            return (_charaLevel < 3) && maxWeaponLevel > _charaLevel;
        }

        return false;
    }

    public void OnDamage(float posX)
    {
        _noDamageTime = _attackSetting.NoDamageTime;

        // 攻撃が飛んできた方向を向く
        _playerPositionController.SetPlayerDirection(posX >= transform.position.x);
        
        // 念のためリセット
        _attackMotionTime = 0f;

        // 被弾したらいったんすべてのモーションを制限
        _playerPositionController.ResetVerticalSpeed(); // これ以上ジャンプで飛び上がらないように制限
        _playerPositionController.SetPlayerMove(false, false, false);
        SetAllActionRestrictionState(true);

        // 画像を被弾状態に
        _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Damaged, _charaLevel);
        _image.transform.localScale = IMG_FLIP;
        _spriteState = ActionSpriteState.Damage;
    }

    private void Debug_ReleaseItem()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("デバッグ用機能　全モーション解放");
            _equipWeaponDict[CsvDefine.ActionData.AttackType.HorizontalMove] = _weaponList.Find(weapon => weapon.Id == 201);
            _equipWeaponDict[CsvDefine.ActionData.AttackType.MiddleDistance] = _weaponList.Find(weapon => weapon.Id == 301);
            _equipWeaponDict[CsvDefine.ActionData.AttackType.LongRange] = _weaponList.Find(weapon => weapon.Id == 401);
            _equipWeaponDict[CsvDefine.ActionData.AttackType.VerticalMove] = _weaponList.Find(weapon => weapon.Id == 501);

            // 移動、ジャンプ、通常攻撃は最初からできる
            _actionEnableStateDict[ActionType.HorizontalMoveAttack].ReleaseAction();
            _actionEnableStateDict[ActionType.MiddleDistanceAttack].ReleaseAction();
            _actionEnableStateDict[ActionType.LongRangeAttack].ReleaseAction();
            _actionEnableStateDict[ActionType.VerticalMoveAttack].ReleaseAction();
        }
    }

    private void ResetSprite()
    {
        if (!_playerPositionController.IsGround)
        {
            if (_spriteState != ActionSpriteState.Jump)
            {
                _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Jump, _charaLevel);
                _image.transform.localScale = IMG_FLIP;
                _spriteState = ActionSpriteState.Jump;
            }
            return;
        }

        if (_playerPositionController.IsMove)
        {
            if (_spriteState != ActionSpriteState.Move)
            {
                _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Run, _charaLevel);
                _image.transform.localScale = IMG_FLIP;
                _spriteState = ActionSpriteState.Move;
            }
            return;
        }

        if (_spriteState != ActionSpriteState.Stop)
        {
            _image.sprite = _spriteSettingData.GetSprite(PlayerSpriteDataSetting.Type.Normal, _charaLevel);
            _image.transform.localScale = IMG_FLIP;
            _spriteState = ActionSpriteState.Stop;
        }
    }
}
