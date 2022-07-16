using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Projectile : MonoBehaviour
{
    public abstract void SetPool(IObjectPool<Projectile> pool);
    public abstract void Fire();
}
