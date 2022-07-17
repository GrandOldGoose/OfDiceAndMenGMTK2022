using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadScene : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] string _sceneName;
    #endregion


    #region Unity Collision Callback Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MySceneManager.Instance.UnloadScene(_sceneName);
        }
    }
    #endregion
}
