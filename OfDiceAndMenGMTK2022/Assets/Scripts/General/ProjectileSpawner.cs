using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _projectileSpawn;
    #endregion



    #region Fields
    private ObjectPool<Projectile> _projectilePool;
    #endregion



    #region Properties
    public ObjectPool<Projectile> ProjectilePool { get => _projectilePool; }
    #endregion



    #region Unity Callback Methods
    private void Awake()
    {
        _projectilePool = new ObjectPool<Projectile>(CreateProjectilePrefab, OnTakeProjectileFromPool, OnReturnProjectileToPool, OnDestroyProjectile, false, 30);
    }
    #endregion



    #region Private Methods
    private Projectile CreateProjectilePrefab()
    {
        Projectile projectile = Instantiate(_projectilePrefab);
        projectile.gameObject.SetActive(false);
        projectile.SetPool(_projectilePool);

        return projectile;
    }

    private void OnTakeProjectileFromPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
        projectile.transform.position = _projectileSpawn.position;
        projectile.transform.rotation = _projectileSpawn.rotation;
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
