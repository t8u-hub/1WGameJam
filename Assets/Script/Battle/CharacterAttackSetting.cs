using UnityEngine;

[CreateAssetMenu(fileName = "CharaAttackSetting", menuName = "ScriptableObjects/CharacterAttackSetting", order = 2)]
public class CharacterAttackSetting : ScriptableObject
{
    [System.Serializable]
    public class NormalAttack
    {
        [SerializeField, Header("当たり判定の範囲")]
        public Vector2 HitArea = new Vector2(40, 60);

        [SerializeField, Header("攻撃後硬直時間")]
        public float RigidityTime = 0.3f;
    }

    [System.Serializable]
    public class HorizontalMoveAttack
    {
        [SerializeField, Header("当たり判定の範囲")]
        public Vector2 HitArea = new Vector2(40, 60);

        [SerializeField, Header("移動の最長継続時間")]
        public float MaxLastTime = 0.5f;

        [SerializeField, Header("移動速度")]
        public float MoveSpeed = 600f;
    }

    [System.Serializable]
    public class MiddleDistanceAttack
    {
        [SerializeField, Header("ボール射出位置のオフセット")]
        public Vector3 HitArea = new Vector3(0, 18, 0);

        [SerializeField, Header("射出角度")]
        public float ThrowPich = 45f;

        [SerializeField, Header("射出速度")]
        public float ThrowSpeed = 450f;

        [SerializeField, Header("攻撃後硬直時間")]
        public float RigidityTime = 0.3f;
    }

    [System.Serializable]
    public class LongRangeAttack
    {
        [SerializeField, Header("当たり判定の範囲")]
        public Vector2 HitArea = new Vector2(40, 60);

        [SerializeField, Header("攻撃後硬直時間")]
        public float RigidityTime = 0.4f;
    }

    [System.Serializable]
    public class VerticalMoveAttack
    {
        [SerializeField, Header("当たり判定の範囲")]
        public Vector2 HitArea = new Vector2(240, 60);

        [SerializeField, Header("攻撃後硬直時間")]
        public float RigidityTime = 0.2f;

        [SerializeField, Header("移動速度")]
        public float MoveSpeed = 400f;
    }

    [System.Serializable]
    public class AttackInfo
    {
        [SerializeField, Header("正面攻撃")]
        public NormalAttack NormalAttack = new NormalAttack();
        [SerializeField, Header("移動攻撃")]
        public HorizontalMoveAttack HorizontalMoveAttack = new HorizontalMoveAttack();
        [SerializeField, Header("投擲攻撃")]
        public MiddleDistanceAttack MiddleDistanceAttack = new MiddleDistanceAttack();
        [SerializeField, Header("範囲攻撃")]
        public LongRangeAttack LongRangeAttack = new LongRangeAttack();
        [SerializeField, Header("降下攻撃")]
        public VerticalMoveAttack VerticalMoveAttack = new VerticalMoveAttack();
    }

    [SerializeField]
    private AttackInfo[] _attackInfoArray;

    [SerializeField, Header("被ダメ後の硬直時間")]
    private float _rigidityTime = .4f;
    public float RighdityTime => _rigidityTime;

    [SerializeField, Header("被ダメ後の無敵時間（硬直時間込み）")]
    private float _noDamageTime = 1.4f;
    public float NoDamageTime => _noDamageTime;

    public NormalAttack GetNormalAttackInfo(int charaPhase = 0)
    {
        return _attackInfoArray[charaPhase].NormalAttack;
    }

    public HorizontalMoveAttack GetHorizontalMoveAttackInfo(int charaPhase = 0)
    {
        return _attackInfoArray[charaPhase].HorizontalMoveAttack;
    }

    public MiddleDistanceAttack GetMiddleDistanceAttackInfo(int charaPhase = 0)
    {
        return _attackInfoArray[charaPhase].MiddleDistanceAttack;
    }

    public LongRangeAttack GetLongRangeAttackInfo(int charaPhase = 0)
    {
        return _attackInfoArray[charaPhase].LongRangeAttack;
    }

    public VerticalMoveAttack GetVerticalMoveAttackInfo(int charaPhase = 0)
    {
        return _attackInfoArray[charaPhase].VerticalMoveAttack;
    }
}
