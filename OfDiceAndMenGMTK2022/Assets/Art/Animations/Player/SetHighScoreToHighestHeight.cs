using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHighScoreToHighestHeight : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private UI_HighScore _highScoreUI;
    [SerializeField] private UI_HighScore _highScoreUI2;
    #endregion


    #region Fields
    private float _highScore = 0f;
    #endregion


    #region Unity Callback Methods
    private void Update()
    {
        if(transform.position.y > _highScore) 
        { 
            _highScore = transform.position.y;
            _highScoreUI.UpdateScoreUI(_highScore);
            _highScoreUI2.UpdateScoreUI(_highScore);
        }
    }
    #endregion
}
