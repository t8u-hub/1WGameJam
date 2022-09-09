using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    class EnemyDefaultInfo
    {
        public int Id;
        public int Type;
        public int Hp;
        public int Attack;
        public int Hit;
        public float Speed;
        public int DropItemId;
    }

    [SerializeField]
    private Transform _enemyPrefabRoot;

    [SerializeField]
    private Enemy[] _enemyPrefabArray;

    [SerializeField]
    private Transform[] _enemySpawnPointArray;

    /// <summary>
    /// ウェーブごとに敵キャラにかかるステータス補正の倍率
    /// </summary>
    private Dictionary<int, (float DamegeCoef, float SpeedCoef)> _enemyCoefDict;

    /// <summary>
    /// ステータス補正のない状態のエネミー情報
    /// </summary>
    private List<EnemyDefaultInfo> _enemyInfoList;

    public void CreateData()
    {
        // 敵のマスターデータ読む
        _enemyInfoList = new List<EnemyDefaultInfo>();
        var enemyDataCsv = new CsvReader().Create(CsvDefine.EnemyData.PATH).CsvData;
        foreach(var enemyData in enemyDataCsv)
        {
            var enemyInfo = new EnemyDefaultInfo
            {
                Id = enemyData[CsvDefine.EnemyData.ENEMY_ID],
                Type = enemyData[CsvDefine.EnemyData.ENEMY_TYPE],
                Hp = enemyData[CsvDefine.EnemyData.HP],
                Attack = enemyData[CsvDefine.EnemyData.ATTACK],
                Hit = enemyData[CsvDefine.EnemyData.HIT_NUM],
                Speed = (float)enemyData[CsvDefine.EnemyData.SPEED] / CsvDefine.INT2FLOAT,
                DropItemId = enemyData[CsvDefine.EnemyData.DROP_ITEM_ID],
            };
            _enemyInfoList.Add(enemyInfo);
        }

        // ウェーブごとでかかる倍率のマスターデータ読む
        _enemyCoefDict = new Dictionary<int, (float DamegeCoef, float SpeedCoef)>();
        var enemyCoefCsv = new CsvReader().Create(CsvDefine.EnemyCoef.PATH).CsvData;
        foreach(var coefData in enemyCoefCsv)
        {
            var wave = coefData[CsvDefine.EnemyCoef.WAVE];
            var speedCoef = (float)coefData[CsvDefine.EnemyCoef.SPEED_COEF] / CsvDefine.INT2FLOAT;
            var damamgeCoef = (float)coefData[CsvDefine.EnemyCoef.DAMEGE_COEF] / CsvDefine.INT2FLOAT;
            _enemyCoefDict.Add(wave, (damamgeCoef, speedCoef));
        }
    }

    /// <summary>
    /// エネミーをスポーンする
    /// </summary>
    /// <returns>成功したらtrueを返す</returns>
    public Enemy SpawnEnemy(int wave, EnemySpawnInfo info)
    {
        var masterEnemyData = _enemyInfoList.Find(enemy => enemy.Id == info.EnemyId);
        if (masterEnemyData == null)
        {
            Debug.LogError($"enemy_data.csvにIdが{info.EnemyId}のデータがない");
            return null;
        }

        if (!_enemyCoefDict.TryGetValue(wave, out var coefData))
        {
            Debug.LogError($"enemy_coef.csvに{wave}wave目の倍率データがない");
            return null;
        }

        if (info.SpawnPoint > _enemySpawnPointArray.Length)
        {
            Debug.LogError($"prefab側にスポーン位置{info.SpawnPoint}の設定がない");
            return null;
        }

        // スポーン処理
        var type = masterEnemyData.Type;
        var index = (masterEnemyData.DropItemId == 0)? type - 1 : type + 4;
        var enemyPrefab = _enemyPrefabArray[index]; // TODO: Idに沿った敵を選ぶ
        var spawnPoint = _enemySpawnPointArray[info.SpawnPoint - 1]; // csvデータは1始まりなのでIndexは-１する
        var parameter = new Enemy.Parameter
        {
            Id = info.EnemyId,
            HitPoint = masterEnemyData.Hp, //?
            MoveSpeed = masterEnemyData.Speed * coefData.SpeedCoef,
            AttackPower = masterEnemyData.Attack * coefData.DamegeCoef,
            DropItemId = masterEnemyData.DropItemId,
        };

        var enemy = Enemy.CreateObject(enemyPrefab, parameter, _enemyPrefabRoot);
        enemy.transform.position = spawnPoint.position;

        return enemy;
    }
}
