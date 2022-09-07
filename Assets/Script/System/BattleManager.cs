using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoSingleton<BattleManager>
{

    protected static BattleManager _instance = null;
    public static BattleManager Instance => _instance;


    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private Transform _playerSpawnPoint;

    [SerializeField]
    private Enemy[] _enemyPrefabArray;

    [SerializeField]
    private Transform[] _enemySpawnPointArray;

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

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void GameStart()
    {
        var playerObj = GameObject.Instantiate(_playerPrefab, _playerSpawnPoint);
        playerObj.transform.localPosition = Vector3.zero;
        _playerTransform = playerObj.transform;

        var enemySpawnManager = new BattleWaveModel();
    }

    public void SpawnEnemy()
    {
        var enemyPrefab = _enemyPrefabArray[0];
        var spawnPoint = _enemySpawnPointArray[0];
        var parameter = new Enemy.Parameter
            {
                HitPoint = 10,
                MoveSpeed = 1,
                AttackPower = 1,
            };

        var enemy = Enemy.CreateObject(enemyPrefab, parameter, spawnPoint);
        enemy.SetTargetTransform(_playerTransform);
        _enemyList.Add(enemy);
    }
}
