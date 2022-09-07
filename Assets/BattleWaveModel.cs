using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnInfo
{
    public int EnemyId { get; }
    public int AppearNum { get; }
    public int SpawnPoint { get; }

    public EnemySpawnInfo(int id, int num, int point)
    {
        EnemyId = id;
        AppearNum = num;
        SpawnPoint = point;
    }
}


public class BattleWaveModel
{
    private int _totalWaveCount;

    private CsvReader _waveData;

    private Dictionary<int /* waveId */, List<EnemySpawnInfo>> _waveEnemyDict;

    public BattleWaveModel()
    {
        // Wave一覧とどんな条件でWaveが終わるか情報
        _waveData = new CsvReader();
        _waveData.Create(CsvDefine.WaveData.PATH);
        _totalWaveCount = _waveData.CsvData.Count;

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

        // 実装うまくいってるか確認用

        for(var i = 1; i <= _totalWaveCount; i++)
        {
            Debug.Log($"wave{i} に出す敵");
            var data = _waveEnemyDict[i];
            foreach(var spawnData in data)
            {
                Debug.Log($"id{spawnData.EnemyId}　数{spawnData.AppearNum}　場所{spawnData.SpawnPoint}");
            }
        }
    }
}
