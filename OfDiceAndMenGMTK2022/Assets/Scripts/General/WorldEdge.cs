using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEdge : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private Transform[] _respawnPoints;
    [SerializeField] private PlayerData _playerData;
    #endregion



    #region Collision Callbacks
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lead"))
        {
            int whichRespawnPointIsClosed = FindClosestSpawnPoint(collision);

            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            IRespawnable respawnable = collision.gameObject.GetComponent<IRespawnable>();

            if (damageable != null) { damageable.TakeDamage(_playerData.DamageAndAffectHandler_FallDamage, DamageType.FALL); }
            if (respawnable != null) { respawnable.Respawn(_respawnPoints[whichRespawnPointIsClosed]); }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
    #endregion



    #region Private Methods
    private int FindClosestSpawnPoint(Collider2D collision)
    {
        float testSmallest = 0.0f;
        float currentSmallest = 10000.0f;
        int whichRespawnPointIsClosed = 0;

        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            testSmallest = Mathf.Abs(collision.transform.position.x - _respawnPoints[i].transform.position.x);

            if (testSmallest < currentSmallest)
            {
                currentSmallest = testSmallest;
                whichRespawnPointIsClosed = i;
            }
        }

        return whichRespawnPointIsClosed;
    }
    #endregion
}
