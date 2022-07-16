using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _neverUnloadScene;
    [SerializeField] private string _playSceneName;

    public void PlayGame()
    {
        MySceneManager.Instance.LoadNewScene(MySceneManager.Instance.Scene1, true, false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
