using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravLift : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private Vector2 _forceApplied;
    #endregion



    #region Properties
    public Vector2 ForceApplied { get => _forceApplied; }
    #endregion


    #region Unity Collision Callbacks
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
                IGravLiftable grav = collision.gameObject.GetComponent<IGravLiftable>();
                if (grav != null) { grav.ApplyGravForce(_forceApplied); }
        }
    }
    #endregion
}
