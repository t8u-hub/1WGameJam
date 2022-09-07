

public static class CsvDefine
{
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

    public static class WaveEnemyData
    {
        public const string PATH = "wave_enemy";

        public const string WAVE_ID = "wave";
        public const string ENEMY_ID = "enemyid";
        public const string APPEAR_COUNT = "num";
        public const string SPAWN_POINT = "point";
    }

}
