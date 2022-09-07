using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager 
{

    private bool _endGame = false;
    private bool _enableSpaewn;
    private float _intervalTime = 0f;

    public EnemySpawnManager()
    {
        BattleManager.Instance.StartCoroutine(StartSpawn());
        _enableSpaewn = true;
    }

    private IEnumerator StartSpawn()
    {
        while (!_endGame)
        {
            yield return null;

            if (!_enableSpaewn)
            {
                continue;
            }

            _intervalTime += Time.deltaTime;
            if (_intervalTime > 5f)
            {
                BattleManager.Instance.SpawnEnemy();
                _intervalTime = 0;
            }
        }

    }

  
}
