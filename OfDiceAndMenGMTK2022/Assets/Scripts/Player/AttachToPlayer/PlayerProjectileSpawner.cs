using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerProjectileSpawner : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private PlayerProjectile _playerProjectilePrefab;
    [SerializeField] private Transform _shotSpawn;
    #endregion



    #region Fields
    PlayerData _playerData;
    private ObjectPool<PlayerProjectile> _playerProjectilePool;
    #endregion



    #region Properties
    public ObjectPool<PlayerProjectile> PlayerProjectilePool { get => _playerProjectilePool; }
    public Transform ShotSpawn { get => _shotSpawn; }
    #endregion



    #region Unity Callback Methods
    private void Awake()
    {
        _playerData = GetComponent<Player>().PlayerData;

        _playerProjectilePool = new ObjectPool<PlayerProjectile>(CreatePlayerProjectilePrefab, OnTakeProjectileFromPool, OnReturnProjectileToPool, OnDestroyProjectile, false, 30);
    }
    #endregion



    #region Private Methods
    //need differnt Create Projectile Method For Each Projectile:
    private PlayerProjectile CreatePlayerProjectilePrefab()
    {
        PlayerProjectile projectile = Instantiate(_playerProjectilePrefab, _playerData.PlayerProjectile_ShotSpawn, Quaternion.Euler(0f, 0f, 0f));
        projectile.gameObject.SetActive(false);
        projectile.SetPool(_playerProjectilePool);

        return projectile;
    }

    private void OnTakeProjectileFromPool(PlayerProjectile projectile)
    {
        projectile.gameObject.SetActive(true);
        projectile.transform.position = _shotSpawn.position;
        projectile.transform.rotation = _shotSpawn.rotation;
        projectile.Fire();
    }

    private void OnReturnProjectileToPool(PlayerProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyProjectile(PlayerProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }
    #endregion
}
