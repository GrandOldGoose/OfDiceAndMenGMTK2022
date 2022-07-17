using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerYcoordinate : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private Transform _playerTransform;
    #endregion


    #region Unity Callback Methods
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);
    }
    #endregion
}
