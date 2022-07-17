using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject pauseMenuUI;
    #endregion



    #region Events
    public static event Action OnGamePause;
    #endregion



    #region Fields
    public static bool GameIsPaused = false;
    #endregion



    #region Unity Callback Methods
    private void Update()
    {
        if (!GameIsPaused)
            Time.timeScale = 1f;
    }
    #endregion



    #region Public Methods
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        inGameUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        OnGamePause?.Invoke();
    }


    public void Pause()
    {
        if (GameIsPaused) { Resume(); }
        else
        {
            pauseMenuUI.SetActive(true);
            inGameUI.SetActive(false);
            Time.timeScale = 0f;
            GameIsPaused = true;
            OnGamePause?.Invoke();
        }
    }

    public void QuitGame()
    {
        Resume();
        Application.Quit();
    }
    #endregion
}
