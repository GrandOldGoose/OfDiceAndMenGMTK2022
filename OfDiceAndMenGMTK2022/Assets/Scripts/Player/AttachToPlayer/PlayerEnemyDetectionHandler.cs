using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyDetectionHandler : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private CircleCollider2D _enemyDetectionCollider;
    #endregion



    #region Fields
    private PlayerData _playerData;
    private LayerMask _enemyLayerMask, _flyingEnemyLayerMask;
    private Collider2D[] _enemiesInRangeList;
    private ContactFilter2D _contactFilter2D;

    private int _enemiesInRange = 0;
    #endregion



    #region Properties
    public int EnemiesInRangeCount { get => _enemiesInRange; }
    public Collider2D EnemyDetectionCollider { get => _enemyDetectionCollider; }
    #endregion



    #region Unity Callback Methods
    private void Awake()
    {
        _playerData = GetComponent<Player>().PlayerData;

        _enemyDetectionCollider.radius = _playerData.EnemyDetectionHandler_Radius;

        _enemyLayerMask = LayerMask.GetMask("Enemy");
        _flyingEnemyLayerMask = LayerMask.GetMask("FyingEnemy");

        _contactFilter2D.useTriggers = true;
        _contactFilter2D.SetLayerMask(_enemyLayerMask | _flyingEnemyLayerMask);
        _contactFilter2D.useLayerMask = true;


        _enemiesInRangeList = new Collider2D[16];
    }

    private void Start()
    {
        InvokeRepeating(nameof(AreEnemiesInRange), 0f, _playerData.EnemyDetectionHandler_CheckRate);
    }
    #endregion



    #region Private Methods
    private void AreEnemiesInRange()
    {
        _enemiesInRange = _enemyDetectionCollider.OverlapCollider(_contactFilter2D, _enemiesInRangeList);
    }
    #endregion
}
