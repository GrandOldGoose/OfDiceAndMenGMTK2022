using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private int _damage;
    [SerializeField] private Vector2 _knockbackForce;
    #endregion


    #region Collision Callbacks
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.CompareTag("PlayerHitBox"))
        {
            IDamageable damageable = hitInfo.gameObject.GetComponentInParent<IDamageable>();
            IKnockbackable knockbackable = hitInfo.gameObject.GetComponentInParent<IKnockbackable>();

            if (damageable != null) { damageable.TakeDamage(_damage, DamageType.SPIKE); }
            if (knockbackable != null) { knockbackable.ApplyKnockbackForce(new Vector2(Random.Range(-_knockbackForce.x, _knockbackForce.x), _knockbackForce.y)); }
        }
    }
    #endregion
}
