using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerUIHandler : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private UI_PlayerHealth _playerHealth;
    #endregion



    #region Public Methods
    public void SetUIHealth(int amount)
    {
        _playerHealth.SetHealth(amount);
    }
    #endregion
}

