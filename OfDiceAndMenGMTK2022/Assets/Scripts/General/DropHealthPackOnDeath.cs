using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHealthPackOnDeath : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] HealthPack _healthPack;
    [SerializeField] int _healthAmount;
    #endregion

    #region Public Methods
    public void CreateHealthPack()
    {
        _healthPack.HealAmount = _healthAmount;
        Instantiate(_healthPack, transform.position, transform.rotation);
    }
    #endregion
}

