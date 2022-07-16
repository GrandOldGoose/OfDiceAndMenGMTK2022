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
    private ObjectPool<Projectile> _playerProjectilePool;
    #endregion



    #region Properties
    public ObjectPool<Projectile> PlayerProjectilePool { get => _playerProjectilePool; }
    public Transform ShotSpawn { get => _shotSpawn; }
    #endregion



    #region Unity Callback Methods
    private void Awake()
    {
        _playerData = GetComponent<Player>().PlayerData;

        _playerProjectilePool = new ObjectPool<Projectile>(CreatePlayerProjectilePrefab, OnTakeProjectileFromPool, OnReturnProjectileToPool, OnDestroyProjectile, false, 30);
    }
    #endregion



    #region Private Methods
    //need differnt Create Projectile Method For Each Projectile:
    private Projectile CreatePlayerProjectilePrefab()
    {
        Projectile projectile = Instantiate(_playerProjectilePrefab, _playerData.PlayerProjectile_ShotSpawn, Quaternion.Euler(0f, 0f, 0f));
        projectile.gameObject.SetActive(false);
        projectile.SetPool(_playerProjectilePool);

        return projectile;
    }

    private void OnTakeProjectileFromPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
        projectile.transform.position = _shotSpawn.position;
        projectile.transform.rotation = _shotSpawn.rotation;
        projectile.Fire();
    }

    private void OnReturnProjectileToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
    #endregion
}
