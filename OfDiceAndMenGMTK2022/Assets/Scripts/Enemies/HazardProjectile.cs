using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HazardProjectile : Projectile
{
    #region Editor Fields
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private int _projectileDamage;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private Rigidbody2D _projectileRigidbody;
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

        if (_timeAlive >= _projectileLifeTime) { DisableProjectile(); }
    }
    #endregion



    #region Public Inherited Methods
    public override void SetPool(IObjectPool<Projectile> pool) => _pool = pool;
    public override void Fire()
    {
        _timeAlive = 0f;
        if (_instanceEffect != null) Instantiate(_instanceEffect, transform.position, transform.rotation);
        _projectileRigidbody.velocity = Vector2.zero;
        _projectileRigidbody.AddForce(transform.right * _projectileSpeed);
    }
    #endregion



    #region Private Methods
    private void DisableProjectile()
    {
        if (_impactEffect != null) { Instantiate(_impactEffect, transform.position, transform.rotation); }
        _projectileRigidbody.velocity = Vector2.zero;
        if (_pool != null) { _pool.Release(this); }
        else Destroy(this.gameObject);
    }
    #endregion



    #region Collision Callbacks
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {

        IHittable hittable = hitInfo.gameObject.GetComponent<IHittable>();

        if (hitInfo.gameObject.CompareTag("PlayerHitBox"))
        {
            IDamageable damageable = hitInfo.gameObject.GetComponentInParent<IDamageable>();
            IKnockbackable knockbackable = hitInfo.gameObject.GetComponentInParent<IKnockbackable>();
            hittable = hitInfo.gameObject.GetComponentInParent<IHittable>();

            if (damageable != null) { damageable.TakeDamage(_projectileDamage, DamageType.ENEMY); }
            if (knockbackable != null) { knockbackable.ApplyKnockbackForce(_projectileRigidbody.velocity.normalized * _knockbackForce); }
        }

        if (hittable != null) { DisableProjectile(); }
    }
    #endregion
}
