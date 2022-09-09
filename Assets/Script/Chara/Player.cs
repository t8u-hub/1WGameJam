using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    private void SetAllActionRestrictionState(bool enable)
    {
        foreach (var actionEnableState in _actionEnableStateDict)
        {
            actionEnableState.Value.SetRestriction(enable);
        }
    }

    void Update()
    {
        if (_weaponList == null || _attackList == null)
        {
            return;
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
                _attackMotionTime = .1f;
                _playerAttack.AttackNormal(weapon.Hit, weapon.Damage);
                SetActionEnableState(CsvDefine.ActionData.AttackType.Normal, ActionType.NormalAttack);
            }
        }
        else if (_actionEnableStateDict[ActionType.HorizontalMoveAttack].CanExec && Input.GetKeyDown(KeyCode.X))
        {
            // 移動攻撃
            var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.HorizontalMove];
            if (weapon != null)
            {
                _attackMotionTime = .5f;
                SetActionEnableState(CsvDefine.ActionData.AttackType.HorizontalMove, ActionType.HorizontalMoveAttack);
                _playerAttack.AttackHorizontalMove(weapon.Hit, weapon.Damage);
                _playerPositionController.DoHorizontalMoveAttack(() =>
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
                _attackMotionTime = .1f;
                _playerAttack.AttackMiddleDistance(weapon.Hit, weapon.Damage, transform.parent);
                SetActionEnableState(CsvDefine.ActionData.AttackType.MiddleDistance, ActionType.MiddleDistanceAttack);
            }
        }
        else if (_actionEnableStateDict[ActionType.LongRangeAttack].CanExec && Input.GetKeyDown(KeyCode.V))
        {
            // 広範囲攻撃
            var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.LongRange];
            if (weapon != null)
            {
                _attackMotionTime = .1f;
                _playerAttack.AttackLongRange(weapon.Hit, weapon.Damage);
                SetActionEnableState(CsvDefine.ActionData.AttackType.LongRange, ActionType.LongRangeAttack);
            }
        }
        else if (_actionEnableStateDict[ActionType.VerticalMoveAttack].CanExec && Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 降下攻撃できるのは滞空中だけ
            if (!_playerPositionController.IsGround)
            {
                // 広範囲攻撃
                var weapon = _equipWeaponDict[CsvDefine.ActionData.AttackType.VerticalMove];
                if (weapon != null)
                {
                    SetActionEnableState(CsvDefine.ActionData.AttackType.VerticalMove, ActionType.VerticalMoveAttack);
                    // 100秒降下し続けることはないと思うので...という最悪なコード
                    _attackMotionTime = 100f;
                    _playerPositionController.DoVerticalMoveAttack(() =>
                    {
                        _playerAttack.AttackVerticalMove(weapon.Hit, weapon.Damage);
                        // 着地してから固定秒数つづく
                        _attackMotionTime = 0.1f;
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
    }

    public void GetItem(int itemId)
    {
        var weapon = _weaponList.Find(weapon => weapon.Id == itemId);
        if (weapon == null)
        {
            Debug.LogError($"存在しない武器ID{itemId}");
            return;
        }

        var attackType = (CsvDefine.ActionData.AttackType)weapon.Type;
        var currentEquipWeapon = _equipWeaponDict[attackType];
        if (currentEquipWeapon == null || currentEquipWeapon?.Level < weapon.Level)
        {
            _equipWeaponDict[attackType] = weapon;
            // CsvDefine.ActionData.AttackType からActionTypeへキャストするため＋1する
            _actionEnableStateDict[(ActionType)weapon.Type + 1].ReleaseAction();
        }

    }
}
