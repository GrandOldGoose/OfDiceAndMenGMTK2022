using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardProjectileLauncher : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private Animator _animator;
    [SerializeField] private ProjectileSpawner _projectileSpawner;
    [SerializeField] private float _timeBeforeFirstProjectile;
    [SerializeField] private float _timeBetweenProjectiles;
    #endregion


    #region Unity Callback Methods
    private void Start()
    {
        InvokeRepeating(nameof(FireProjectile), _timeBeforeFirstProjectile, _timeBetweenProjectiles);
        InvokeRepeating(nameof(WaitABit), _timeBeforeFirstProjectile + 0.5f, _timeBetweenProjectiles);
    }
    #endregion



    #region Private Methods
    private void FireProjectile()
    {
        _animator.SetBool("fire", true);

        _projectileSpawner.ProjectilePool.Get();
    }


    private void WaitABit()
    {
        _animator.SetBool("fire", false);
    }
    #endregion
}
