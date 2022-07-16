using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    INITIAL,
    ENEMY,
    FALL, SPIKE,
    PLAYER_PROJECTILE
}

public interface IDamageable
{
    public void TakeDamage(int damageAmount, DamageType damageType);
}
