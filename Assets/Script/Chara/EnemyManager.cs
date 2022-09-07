using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    private Transform _targetTransform;

    private Parameter _parameter;

    public class Parameter
    {
        public int HitPoint;
        public float MoveSpeed;
        public float AttackPower;
        public int dropItemId;
    }

    public static EnemyManager CreateObject(EnemyManager prefab, Parameter paramter, Transform parentTransform)
    {
        var enemyManager = GameObject.Instantiate<EnemyManager>(prefab, parentTransform);
        enemyManager._parameter = paramter;
        enemyManager.transform.localPosition = Vector3.zero;

        return enemyManager;
    }

    /// <summary>
    /// 移動の目的地Transformを設定
    /// </summary>
    public void SetTargetTransform(Transform transform)
    {
        _targetTransform = transform;
    }

    public void Update()
    {
        
    }
}
