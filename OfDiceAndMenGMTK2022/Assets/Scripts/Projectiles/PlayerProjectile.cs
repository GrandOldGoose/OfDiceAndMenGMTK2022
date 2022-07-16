using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerProjectile : Projectile
{
    #region Editor Fields
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Collider2D _projectileCollider;
    [SerializeField] private GameObject _instanceEffect;
    [SerializeField] private GameObject _impactEffect;
    #endregion



    #region Fields
    private IObjectPool<Projectile> _pool;
    private float _timeAlive = 0f;
    #endregion



    #region Unity Callback Methods
    private void Update()
    {
        _timeAlive += Time.deltaTime;

        if (_timeAlive >= _playerData.PlayerProjectile_ProjectileLifetime) { DisableBullet(); }
    }
    #endregion



    #region Public Inherited Methods
    public override void SetPool(IObjectPool<Projectile> pool) => _pool = pool;
    public override void Fire()
    {
        _timeAlive = 0f;
        if (_instanceEffect != null) Instantiate(_instanceEffect, transform.position, transform.rotation);
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(transform.right * _playerData.PlayerProjectile_ProjectileSpeed);
    }
    #endregion



    #region Private Methods
    private void DisableBullet()
    {
        if (_impactEffect != null) { Instantiate(_impactEffect, transform.position, transform.rotation); }
        _rigidbody2D.velocity = Vector2.zero;
        if (_pool != null) { _pool.Release(this); }
        else Destroy(this.gameObject);
    }
    #endregion



    #region Collision Callbacks
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        IDamageable damageable = hitInfo.gameObject.GetComponent<IDamageable>();
        IHittable hittable = hitInfo.gameObject.GetComponent<IHittable>();

        if (hitInfo.gameObject.CompareTag("Enemy")) { if (damageable != null) { damageable.TakeDamage(_playerData.PlayerProjectile_ProjectileDamage, DamageType.PLAYER_PROJECTILE); DisableBullet(); } }
        if (hittable != null) { DisableBullet(); }
    }
    #endregion
}
