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
    private EnemyManager[] _enemyPrefabArray;

    [SerializeField]
    private Transform[] _enemySpawnPointArray;

    private Transform _playerTransform;

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

        var enemySpawnManager = new EnemySpawnManager();
    }

    public void SpawnEnemy()
    {
        var enemyPrefab = _enemyPrefabArray[0];
        var spawnPoint = _enemySpawnPointArray[0];
        var parameter = new EnemyManager.Parameter
            {
                HitPoint = 10,
                MoveSpeed = 1,
                AttackPower = 1,
            };

        var enemy = EnemyManager.CreateObject(enemyPrefab, parameter, spawnPoint);
        enemy.SetTargetTransform(_playerTransform);
    }
}
