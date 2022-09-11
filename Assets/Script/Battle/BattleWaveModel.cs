using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// エネミーのリスポーン情報
/// </summary>
public class EnemySpawnInfo
{
    public int EnemyId { get; }
    public int AppearNum { get; }
    public int SpawnPoint { get; }
    public int DropItemId { get; }

    public EnemySpawnInfo(int id, int num, int point)
    {
        EnemyId = id;
        AppearNum = num;
        SpawnPoint = point;
    }
}

/// <summary>
/// バトルのWave管理用クラス
/// </summary>
public class BattleWaveModel
{
    public class Wave
    {
        public int Id;
        public CsvDefine.WaveData.WaveType WaveType;
        public float AttacCoef;
        public float GaugeCoef;
        public List<int> TypeValueList;
    }

    private int _currentWaveId;
    public int CurrentWave => _currentWaveId;

    private Wave _currentWaveData;

    public Wave CurrentWaveData => _currentWaveData;

    private List<Wave> _waveList;

    private Dictionary<int /* waveId */, List<EnemySpawnInfo>> _waveEnemyDict;

    private float _currentWaveTime = 0;

    public bool AllWaveEnd => _allWaveEnd;
    private bool _allWaveEnd = false;

    public BattleWaveModel()
    {
        // Wave一覧とどんな条件でWaveが終わるか情報
        var waveData = new CsvReader().Create(CsvDefine.WaveData.PATH).CsvData;
        _waveList = waveData.Select(csvData => new Wave
        {
            Id = csvData[CsvDefine.WaveData.WAVE_ID],
            WaveType = (CsvDefine.WaveData.WaveType)csvData[CsvDefine.WaveData.WAVE_TYPE],
            AttacCoef = csvData[CsvDefine.WaveData.INFLATION_COEF] / CsvDefine.INT2FLOAT,
            GaugeCoef = csvData[CsvDefine.WaveData.GAUGE_COEF] / CsvDefine.INT2FLOAT,
            TypeValueList = new List<int>()
            {
                csvData[CsvDefine.WaveData.VALUE_1],
                csvData[CsvDefine.WaveData.VALUE_2],
                csvData[CsvDefine.WaveData.VALUE_3],
                csvData[CsvDefine.WaveData.VALUE_4],
                csvData[CsvDefine.WaveData.VALUE_5],
            },
        }).ToList();


        // Waveごとに出現させる敵の情報
        var waveEnemyDataList = new CsvReader().Create(CsvDefine.WaveEnemyData.PATH).CsvData;
        _waveEnemyDict = new Dictionary<int, List<EnemySpawnInfo>>();

        foreach (var waveEnemyData in waveEnemyDataList)
        {
            var targetWave = waveEnemyData[CsvDefine.WaveEnemyData.WAVE_ID];
            var spawnInfo = new EnemySpawnInfo(
                waveEnemyData[CsvDefine.WaveEnemyData.ENEMY_ID],
                waveEnemyData[CsvDefine.WaveEnemyData.APPEAR_COUNT],
                waveEnemyData[CsvDefine.WaveEnemyData.SPAWN_POINT]);
            if (_waveEnemyDict.TryGetValue(targetWave, out var waveEnemyList))
            {
                // 既存のものに追加
                waveEnemyList.Add(spawnInfo);
            }
            else
            {
                // 新規に追加
                _waveEnemyDict.Add(targetWave, new List<EnemySpawnInfo>() { spawnInfo });
            }
        }

    }

    public void Initialize()
    {
        // WAVE1からはじめる
        StartWave(1);
    }

    public void UpdateWaveModel()
    {
        // 時間更新
        _currentWaveTime += Time.deltaTime;

        // Wave終了判定
        var waveEnd = false;

        switch (_currentWaveData.WaveType)
        {
            case CsvDefine.WaveData.WaveType.LEAST_TOTAL_ENEMY_COUNT:
                var currentEnemyCount = BattleManager.Instance.CurrentEnemyCount;
                if (currentEnemyCount <= _currentWaveData.TypeValueList[0])
                {
                    // 残りのエネミー数が定数以下ならウェーブ終了
                    waveEnd = true;
                }
                break;
            case CsvDefine.WaveData.WaveType.TIME_PAST:
                var limitTime = (float)_currentWaveData.TypeValueList[0] / CsvDefine.INT2FLOAT;
                if (_currentWaveTime >= limitTime)
                {
                    // ウェーブが始まってから一定時間経過していたら終了
                    waveEnd = true;
                }
                break;
            case CsvDefine.WaveData.WaveType.SUBDUE_ENEMY:
                var currentEnemyList = BattleManager.Instance.EnemyList;
                // リスト内のIdの敵キャラが一体もいなければウェーブ終了
                waveEnd = !currentEnemyList.Any(enemy => _currentWaveData.TypeValueList.Any(value => value == enemy.Id));
                break;
        }

        // Wave終了条件を満たしていたら次のウェーブへ移行
        if (waveEnd)
        {
            if (_currentWaveId >= _waveList.Count)
            {
                _allWaveEnd = true;
            }
            else
            {
                StartWave(_currentWaveId + 1);
            }
        }

    }

    private void StartWave(int waveId)
    {
        _currentWaveId = waveId;
        _currentWaveData = _waveList.Find(wave => wave.Id == waveId);
        if (_currentWaveData == null)
        {
            Debug.LogError("ウェーブのデータがない");
            return;
        }

        _currentWaveTime = 0f;

        // 1WAVE目のエネミーをスポーン
        BattleManager.Instance.SpawnEnemy(_waveEnemyDict[_currentWaveId]);
    }
}
