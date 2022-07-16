using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    #region Editor Fields
    [System.Serializable]
    public class EnemyScoreValues
    {
        public string name;
        public int score;
    }

    public EnemyScoreValues[] enemyScoreValues;
    #endregion


    #region Properties
    public int CurrentScore { get => currentScore; }
    #endregion



    #region Fields
    private int currentScore = 0;
    #endregion



    #region Public Methods
    public void AddScore(string enemyName)
    {
        for (int i = 0; i < enemyScoreValues.Length; i++)
        {
            if (enemyName == enemyScoreValues[i].name + "(Clone)" || enemyName == enemyScoreValues[i].name + " (Clone)" || enemyName == enemyScoreValues[i].name)
            {
                currentScore += enemyScoreValues[i].score;
                return;
            }
        }
        Debug.LogError(enemyName + "not in list");
    }
    #endregion
}
