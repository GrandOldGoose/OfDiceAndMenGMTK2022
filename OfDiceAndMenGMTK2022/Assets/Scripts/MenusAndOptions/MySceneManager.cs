using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    #region Singleton
    private static MySceneManager _instance;
    public static MySceneManager Instance { get { return _instance; } }
    #endregion


    #region Public Vars
    public string NeverUnloadScene;
    public string Scene1;
    public string Scene2;
    public string Scene3;
    public string Scene4;
    public string Scene5;
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
            SceneManager.LoadSceneAsync(Scene1, LoadSceneMode.Additive);
        }
    }
    #endregion



    #region Public Methods


    public void LoadNewScene(string newScene, bool setPlayerUp, bool disablePlayer)
    {
        if (!SceneManager.GetSceneByName(newScene).isLoaded) { SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive); Debug.Log("Load: " + newScene); }
        //if (setPlayerUp) { SetUpPlayer(); }
    }

    public void UnloadScene(string scene)
    {
        if (SceneManager.GetSceneByName(scene).isLoaded) { StartCoroutine(Unload(scene)); Debug.Log("Unload: " + scene); }
    }
    #endregion



    #region Private Methods
    private IEnumerator Unload(string scene)
    {
        yield return null;

        SceneManager.UnloadSceneAsync(scene);
    }

    /*
    private void SetUpPlayer()
    {
        GameManager.Instance.PlayerGameObject.SetActive(true);
        GameManager.Instance.InGameUI.SetActive(true);
    }
    */

    /*
    private void DisablePlayer()
    {
        var playerGameObject = GameManager.Instance.PlayerGameObject;
        playerGameObject.GetComponent<Player>().SetAllStatesInitial = true;

        GameManager.Instance.PlayerGameObject.SetActive(false);
        GameManager.Instance.InGameUI.SetActive(false);
    }
    */
    #endregion
}
