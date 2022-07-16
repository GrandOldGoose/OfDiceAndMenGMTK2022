using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardProjectileLauncher : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private ProjectileSpawner _projectileSpawner;
    [SerializeField] private float _timeBeforeFirstProjectile;
    [SerializeField] private float _timeBetweenProjectiles;
    #endregion


    #region Unity Callback Methods
    private void Start()
    {
        InvokeRepeating(nameof(FireProjectile), _timeBeforeFirstProjectile, _timeBetweenProjectiles);
    }
    #endregion



    #region Private Methods
    private void FireProjectile()
    {
        _projectileSpawner.ProjectilePool.Get();
    }
    #endregion
}
