using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// 必殺技ゲージの上限値
    /// </summary>
    public static float MAX_GAUGE_VALUE = 100f;

    public bool StopUpdate { get; private set; } = false;

    protected static BattleManager _instance = null;
    public static BattleManager Instance => _instance;

    /// <summary>
    /// 必殺技が使えるかどうか
    /// </summary>
    public bool CanUseSpecialAttack => CurrentGauge >= MAX_GAUGE_VALUE;

    [SerializeField]
    private Player _playerPrefab;

    [SerializeField]
    private Transform _playerSpawnPoint;

    [SerializeField]
    private EnemySpawner _enemySpawner;

    private Player _player;

    /// <summary>
    /// 獲得済のアイテムID
    /// </summary>
    private List<int> GetItemIdList = new List<int>() { 101 };

    /// <summary>
    /// 現在の必殺技ゲージの値
    /// </summary>
    public float CurrentGauge { get; private set; }

    /// <summary>
    /// 現在のプレイヤー累積ダメージ (実際の値の1/1000)
    /// </summary>
    public float ScaledTotalDamage { get; private set; }

    /// <summary>
    /// 現在のスコア
    /// </summary>
    public int CurrentScore { get; private set; }

    /// <summary>
    /// 現在スポーンしている総エネミー数
    /// </summary>
    public int CurrentEnemyCount { get; private set; }


    public int MoveRaneg
    {
        get
        {
            if (_player.CharaLevel == 1)
            {
                return 400;
            } 
            else if (_player.CharaLevel == 2)
            {
                return 480;
            }
            else
            {
                return 560;
            }
        }
    }

    public float PlayerAttackCoef => _battleWaveModel.CurrentWaveData.AttacCoef;

    /// <summary>
    /// 表示中の敵一覧
    /// </summary>
    private List<Enemy> _enemyList = new List<Enemy>();
    public IEnumerable<Enemy> EnemyList => _enemyList;

    private BattleWaveModel _battleWaveModel;

    bool _playGame = false;

    private GameUi _gameUi;

    private float _noDamageTimer;

    private float _pastTime;

    private int _specialAttackNum = 0;

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
        if (!_playGame || StopUpdate)
        {
            return;
        }

        _pastTime += Time.deltaTime;

        _noDamageTimer += Time.deltaTime;
        if(_noDamageTimer > 3f && ScaledTotalDamage > 0)
        {
            // 仮　1秒で.２回復する想定
            var recoverAmount = .2f * Time.deltaTime;
            ScaledTotalDamage = Mathf.Max(0, ScaledTotalDamage - recoverAmount);
        }

        _battleWaveModel.UpdateWaveModel();
        if (_battleWaveModel.AllWaveEnd)
        {
            var param = new ResultTempData.Parameter
            {
                IsClear = true,
                FinalCharaLevel = _player.CharaLevel,
                Score = CurrentScore,
                ItemIdList = GetItemIdList,
                SpecialAttackNum = _specialAttackNum
            };
            StartCoroutine(GameFinish(param, new Color32(255, 255, 255, 255)));
        }


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {

            var param = new ResultTempData.Parameter
            {
                IsClear = false,
                FinalCharaLevel = 3,
                Score = CurrentScore,
                ItemIdList = GetItemIdList,
                SpecialAttackNum = _specialAttackNum
            };
            StartCoroutine(GameFinish(param, new Color32(255, 255, 255, 255)));
        }
#endif
    }

    public void GameStart(GameUi gameUi)
    {
        _gameUi = gameUi;

        // プレイヤー生成
        var playerObj = GameObject.Instantiate<Player>(_playerPrefab, _playerSpawnPoint);
        playerObj.transform.localPosition = Vector3.zero;
        _player = playerObj;

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
                enemy.SetTargetTransform(_player.transform);
                _enemyList.Add(enemy);
            }
        }

        CurrentEnemyCount = _enemyList.Count;
    }

    public void OnEnemyKilled(Enemy enemy)
    {
        if (_enemyList.Contains(enemy))
        {
            _enemyList.Remove(enemy);
            CurrentEnemyCount = _enemyList.Count;

            // アイテムをドロップする敵が死んだときの処理
            if (enemy.DropItemId != 0)
            {
                var isLevelUp = _player.GetItem(enemy.DropItemId);

                if (!GetItemIdList.Contains(enemy.DropItemId))
                {
                    GetItemIdList.Add(enemy.DropItemId);
                }
                _gameUi.UpdateItemUiIfNeed(enemy.DropItemId);

                // レベルが上がるときの処理
                if (isLevelUp)
                {
                    OnLevelUp();
                }
            }
        }
    }

    public void OnLevelUp()
    {
        var sceneManager = SceneManager.Instance.GetCurrentSceneData() as GameSceneController;
        if (sceneManager == null)
        {
            Debug.LogError("シーンコントローラーが見つからない");
            return;
        }

        _player.LevelUp();
        sceneManager.ChangeStageLevel(_player.CharaLevel);
    }


    /// <summary>
    /// ゲージ上昇量
    /// </summary>
    /// 引数はエネミーが受けたダメージ
    public void PlayerAttack(int damage, bool isSpecialAttack)
    {
        // スコア上昇
        {
            var specialScale = isSpecialAttack ? 2 : 1;
            if (_pastTime < 300f) // 経過時間が5分未満なら1倍
            {
                CurrentScore += (int)(specialScale * damage);
            }
            else if (_pastTime < 600f)
            {
                var timeScale = (_pastTime - 300f) / 300f;
                CurrentScore += (int)(specialScale * timeScale * damage);
            }
            else // 経過時間が10分未満なら.5倍
            {
                CurrentScore += (int)(specialScale * damage * .5f);
            }
        }

        var prevGauge = CurrentGauge;

        // ゲージ上昇
        CurrentGauge += (float)damage / _battleWaveModel.CurrentWaveData.GaugeCoef; // 値がめちゃ大きくなりそうなのでスケールかけとく

        if (CurrentGauge >= MAX_GAUGE_VALUE)
        {
            CurrentGauge = MAX_GAUGE_VALUE;
            if (prevGauge < MAX_GAUGE_VALUE)
            {
                _gameUi.PlayGaugeAnim();
                SeAudioManager.Instance.Play(SeAudioManager.SeType.GaugeMax);
            }
        }
    }

    /// <summary>
    /// 敵の攻撃（プレイヤーが敵から攻撃を受けたときの処理）
    /// </summary>
    public void EnemyAttack(float damage, float posX)
    {
        if (_player.InNoDamageTime)
        {
            return;
        }

        SeAudioManager.Instance.Play(SeAudioManager.SeType.Damage);
        _player.OnDamage(posX);
        ScaledTotalDamage += damage / 1000f; // 値がめちゃくちゃ大きくなりそうなのでスケールしておく

        if (ScaledTotalDamage > 30)
        {
            StopUpdate = true;
            // ゲームオーバー
            var param = new ResultTempData.Parameter
            {
                IsClear = false,
                FinalCharaLevel = _player.CharaLevel,
                Score = CurrentScore,
                ItemIdList = GetItemIdList,
                SpecialAttackNum = _specialAttackNum
            };
            StartCoroutine(GameFinish(param, new Color32(255, 211, 232, 255)));
        }

        _noDamageTimer = 0;
    }

    public void SpecialAttack()
    {
        _specialAttackNum += 1;
        StartCoroutine(SpecialAttackInner());
    }

    private IEnumerator SpecialAttackInner()
    {
        StopUpdate = true;

        SeAudioManager.Instance.Play(SeAudioManager.SeType.SpecialAttack);
        _gameUi.PlayCutin(_player.CharaLevel);
        yield return new WaitForSecondsRealtime(1.2f);

        StopUpdate = false;

        foreach(var enemy in _enemyList)
        {
            enemy.OnDamage(1, (int)(2000 * _battleWaveModel.CurrentWaveData.AttacCoef), true);
        }
        CurrentGauge = 0;
    }

    private IEnumerator GameFinish(ResultTempData.Parameter param, Color32 fadeColor)
    {
        yield return new WaitForSecondsRealtime(1f);
        ResultTempData.Instance.SetData(param);
        UIManager.Instance.SetFadeColor(fadeColor);
        SceneManager.Instance.ChangeScene(SceneDefine.Scene.Result);
    }
}
