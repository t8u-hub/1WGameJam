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
        public float WaitTime;
    }

    class EnemyActionWeight
    {
        public int EnemyId;
        public int WeightMove;
        public int WeithtStop;
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


    private List<EnemyActionWeight> _enemyActionWeightList;

    public void CreateData()
    {
        // 敵のマスターデータ読む
        _enemyInfoList = new List<EnemyDefaultInfo>();
        var enemyDataCsv = new CsvReader().Create(CsvDefine.EnemyData.PATH).CsvData;
        foreach (var enemyData in enemyDataCsv)
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
                WaitTime = enemyData[CsvDefine.EnemyData.WAIT_TIME] / 1000f, // ミリ秒で数値が入ってる
            };
            _enemyInfoList.Add(enemyInfo);
        }

        // ウェーブごとでかかる倍率のマスターデータ読む
        _enemyCoefDict = new Dictionary<int, (float DamegeCoef, float SpeedCoef)>();
        var enemyCoefCsv = new CsvReader().Create(CsvDefine.EnemyCoef.PATH).CsvData;
        foreach (var coefData in enemyCoefCsv)
        {
            var wave = coefData[CsvDefine.EnemyCoef.WAVE];
            var speedCoef = (float)coefData[CsvDefine.EnemyCoef.SPEED_COEF] / CsvDefine.INT2FLOAT;
            var damamgeCoef = (float)coefData[CsvDefine.EnemyCoef.DAMEGE_COEF] / CsvDefine.INT2FLOAT;
            _enemyCoefDict.Add(wave, (damamgeCoef, speedCoef));
        }

        _enemyActionWeightList = new CsvReader().Create(CsvDefine.EnemyAction.PATH).CsvData
            .Select(data => new EnemyActionWeight
            {
                EnemyId = data[CsvDefine.EnemyAction.ENEMY_ID],
                WeightMove = data[CsvDefine.EnemyAction.MOVE_WEIGHT] / 100,
                WeithtStop = data[CsvDefine.EnemyAction.STOP_WEIGHT] / 100,
            })
            .ToList();
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

        var masterEnemyAction = _enemyActionWeightList.Find(enemy => enemy.EnemyId == info.EnemyId);
        if (masterEnemyAction == null)
        {
            Debug.LogError($"enemy_action_data.csvにIdが{info.EnemyId}のデータがない");
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
        var index = (masterEnemyData.DropItemId == 0) ? type - 1 : type + 4;
        var enemyPrefab = _enemyPrefabArray[index];
        var spawnPoint = _enemySpawnPointArray[info.SpawnPoint - 1]; // csvデータは1始まりなのでIndexは-１する
        var parameter = new Enemy.Parameter
        {
            Id = info.EnemyId,
            HitPoint = masterEnemyData.Hp, //?
            MoveSpeed = masterEnemyData.Speed * coefData.SpeedCoef,
            AttackPower = masterEnemyData.Attack * coefData.DamegeCoef,
            DropItemId = masterEnemyData.DropItemId,
            MoveWeight = masterEnemyAction.WeightMove,
            StopWeight = masterEnemyAction.WeithtStop,
            WaitTime = masterEnemyData.WaitTime,
        };

        Enemy enemy;
        if (type == 5)
        {
            enemy = SpawnFlying(enemyPrefab as EnemyFlying, parameter);
        }
        else if (type == 3)
        {
            enemy = SpawnFlyAndThrow(enemyPrefab as EnemyFlyAndThrow, parameter);
        }
        else
        {
            enemy = SpawnNormal(enemyPrefab, parameter);
        }

        enemy.transform.position = spawnPoint.position + new Vector3(Random.Range(-100f, 100f) , Random.Range(0f, 100f), 0f);

        return enemy;
    }

    public Enemy SpawnNormal(Enemy prefab, Enemy.Parameter parameter)
    {
        var enemy = Enemy.CreateObject(prefab, parameter, _enemyPrefabRoot);
        return enemy;
    }

    public Enemy SpawnFlying(EnemyFlying prefab, Enemy.Parameter parameter)
    {
        var enemy = EnemyFlying.CreateObject(prefab, parameter, _enemyPrefabRoot);
        return enemy;
    }

    public Enemy SpawnFlyAndThrow(EnemyFlyAndThrow prefab, Enemy.Parameter parameter)
    {
        var enemy = EnemyFlyAndThrow.CreateObject(prefab, parameter, _enemyPrefabRoot);
        return enemy;
    }
}
