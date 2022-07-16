using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HighScore : MonoBehaviour
{

    ////////////SerializeField Var Defs:
    [SerializeField] private Text textUI;




    ////////////Private Var Defs:
    private int currentScore = 0;




    ////////////Unity Functions:
    private void Update()
    {
        UpdateScoreUI();
    }



    ////////////private Functions:
    private void UpdateScoreUI()
    {
        currentScore = GameManager.Instance.HighScore.CurrentScore;
        textUI.text = currentScore.ToString();
    }

}