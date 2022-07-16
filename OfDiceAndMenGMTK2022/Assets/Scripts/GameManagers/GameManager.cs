using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    #endregion



    #region Public Vars
    public GameObject PlayerGameObject;
    public GameObject InGameUI;
    public GameObject PauseMenuUI;
    public HighScore HighScore;

    public Transform PlayerSpawnPoint;
    #endregion


    #region Unity Callback Methods
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }


        HighScore = GetComponent<HighScore>();
    }
    #endregion
}
