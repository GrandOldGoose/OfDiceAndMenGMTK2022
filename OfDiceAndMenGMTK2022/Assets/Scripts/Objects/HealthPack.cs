using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private int _healAmount = 0;
    #endregion


    #region Properites
    public int HealAmount { get => _healAmount; set => _healAmount = value; }
    #endregion


    #region Collision Callback Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lead"))
        {
            IHealthPackable healthPackable = collision.gameObject.GetComponent<IHealthPackable>();
            if (healthPackable != null) { healthPackable.AddHealth(_healAmount); }

            Destroy(this.gameObject);
        }
    }
    #endregion
}
