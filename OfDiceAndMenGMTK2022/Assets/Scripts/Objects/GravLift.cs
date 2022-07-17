using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravLift : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private Vector2 _forceApplied;
    [SerializeField] private Animator _animator;
    #endregion



    #region Properties
    public Vector2 ForceApplied { get => _forceApplied; }
    #endregion

    #region Private Methods
    private IEnumerator waitABit()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("playerContact", false);
    }
    #endregion


    #region Unity Collision Callbacks
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.CompareTag("Player"))
        {

            IGravLiftable grav = collision.gameObject.GetComponent<IGravLiftable>();
                if (grav != null) { grav.ApplyGravForce(_forceApplied); _animator.SetBool("playerContact", true); StartCoroutine(waitABit()); }
            
        }
    }
    #endregion
}
