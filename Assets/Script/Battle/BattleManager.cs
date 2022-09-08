using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoBehaviour
{

    protected static BattleManager _instance = null;
    public static BattleManager Instance => _instance;


    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private Transform _playerSpawnPoint;

    [SerializeField]
    private EnemySpawner _enemySpawner;

    private Transform _playerTransform;

    /// <summary>
    /// 獲得済のアイテムID
    /// </summary>
    private List<int> GetItemIdList = new List<int>();

    /// <summary>
    /// 現在の必殺技ゲージの値
    /// </summary>
    public int CurrentGauge { get; private set; }

    /// <summary>
    /// 現在のプレイヤーHP
    /// </summary>
    public int CurrentHp { get; private set; }

    /// <summary>
    /// 現在のスコア
    /// </summary>
    public int CurrentScore { get; private set; }

    /// <summary>
    /// 表示中の敵一覧
    /// </summary>
    private List<Enemy> _enemyList = new List<Enemy>();

    private BattleWaveModel _battleWaveModel;

    bool _playGame = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _battleWaveModel = new BattleWaveModel();
            _playGame = false;

            _enemySpawner.CreateData();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (!_playGame)
        {
            return;
        }

        _battleWaveModel.UpdateWaveModel();
    }

    public void GameStart()
    {
        // プレイヤー生成
        var playerObj = GameObject.Instantiate(_playerPrefab, _playerSpawnPoint);
        playerObj.transform.localPosition = Vector3.zero;
        _playerTransform = playerObj.transform;

        // ウェーブ初期化
        _battleWaveModel.Initialize();

        _playGame = true;
    }

    /// <summary>
    /// エネミーをスポーンさせる
    /// </summary>
    /// <param name="spawnInfoList"></param>
    public void SpawnEnemy(List<EnemySpawnInfo> spawnInfoList)
    {
        foreach (var spawnInfo in spawnInfoList)
        {
            for (int i = 0; i < spawnInfo.AppearNum; i++)
            {
                var enemy = _enemySpawner.SpawnEnemy(_battleWaveModel.CurrentWave, spawnInfo);
                enemy.SetTargetTransform(_playerTransform);
                _enemyList.Add(enemy);
            }
        }
    }

    public void OnEnemyKilled(Enemy enemy)
    {
        if (_enemyList.Contains(enemy))
        {
            _enemyList.Remove(enemy);
        }
    }
}
