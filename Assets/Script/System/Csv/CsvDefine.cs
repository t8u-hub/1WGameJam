

public static class CsvDefine
{
    public static float INT2FLOAT = 10000f;
    public static class WaveData
    {
        public const string PATH = "wave_data";

        public const string WAVE_ID = "wave";
        public const string WAVE_TYPE = "type";
        /// <summary> ウェーブの進行による攻撃力の上がり幅 </summary>
        public const string INFLATION_COEF = "inflation_coef";
        /// <summary> ウェーブの進行によるゲージ獲得量の下がり幅 </summary>
        public const string GAUGE_COEF = "gage_coef";
        /// <summary> WAVE_TYPEごとに設定される値 </summary>
        public const string VALUE_1 = "value1";
        public const string VALUE_2 = "value2";
        public const string VALUE_3 = "value3";
        public const string VALUE_4 = "value4";
        public const string VALUE_5 = "value5";
        public enum WaveType
        {
            LEAST_TOTAL_ENEMY_COUNT = 1,    // 敵の総数がvalue1以下
            TIME_PAST = 2,                  // 一定時間経過
            SUBDUE_ENEMY = 3,               // 特定エネミーの全討伐
        }
    }

    /// <summary>
    /// Waveごとに出てくるエネミーのデータ
    /// </summary>
    public static class WaveEnemyData
    {
        public const string PATH = "wave_enemy";

        public const string WAVE_ID = "wave";
        public const string ENEMY_ID = "enemyid";
        public const string APPEAR_COUNT = "num";
        public const string SPAWN_POINT = "point";
    }

    /// <summary>
    /// エネミーのデータ
    /// </summary>
    public static class EnemyData
    {
        public const string PATH = "enemy_data";

        public const string ENEMY_ID = "enemy_id";
        /// <summary> 敵のタイプ（ウマ、カエルetc...） </summary>
        public const string ENEMY_TYPE = "type";
        public const string HP = "hp";
        public const string ATTACK = "attack";
        public const string HIT_NUM = "hit";
        public const string SPEED = "speed";
        public const string DROP_ITEM_ID = "item";
    }

    /// <summary>
    /// ウェーブに応じて敵のステータスにかかる係数
    /// </summary>
    public static class EnemyCoef
    {
        public const string PATH = "enemy_coef";

        public const string WAVE = "wave";
        public const string DAMEGE_COEF = "dmgcoef";
        public const string SPEED_COEF = "speedcoef";
    }

    /// <summary>
    /// 武器マスターデータ
    /// </summary>
    public static class WeaponData
    {
        public const string PATH = "weapon_data";

        public const string WEAPON_ID = "id";
        public const string ATTACK_TYPE = "type";
        public const string WEAPON_LEVEL = "level";
        public const string DAMAGE = "damage";
        public const string HIT = "hit";
    }

    /// <summary>
    /// 攻撃アクションマスターデータ
    /// </summary>
    public static class ActionData
    {
        public const string PATH = "action_data";

        public const string ATTACK_TYPE = "action_id";
        public const string RECAST_TIME = "recast_time";

        /// <summary> 攻撃アクション中にジャンプできるか </summary>
        public const string CAN_JUMP = "available_action_jump";
        /// <summary> 攻撃アクション中に左右移動できるか </summary>
        public const string CAN_MOVE = "available_action_move";
        /// <summary> 攻撃アクション中にさらに攻撃アクション１を続けられるか </summary>
        public const string CAN_ACTION_1 = "available_action_1";
        /// <summary> 攻撃アクション中にさらに攻撃アクション２を続けられるか </summary>
        public const string CAN_ACTION_2 = "available_action_2";
        /// <summary> 攻撃アクション中にさらに攻撃アクション３を続けられるか </summary>
        public const string CAN_ACTION_3 = "available_action_3";
        /// <summary> 攻撃アクション中にさらに攻撃アクション４を続けられるか </summary>
        public const string CAN_ACTION_4 = "available_action_4";
        /// <summary> 攻撃アクション中にさらに攻撃アクション５を続けられるか </summary>
        public const string CAN_ACTION_5 = "available_action_5";

        /// <summary>
        /// 攻撃アクションの種類
        /// </summary>
        public enum AttackType
        {
            None = 0,
            Normal = 1,             // 正面攻撃
            HorizontalMove = 2,     // 移動攻撃
            MiddleDistance = 3,     // 遠距離攻撃
            LongRange = 4,          // 広範囲攻撃
            VerticalMove = 5,       // 降下攻撃
        }
    }
}
