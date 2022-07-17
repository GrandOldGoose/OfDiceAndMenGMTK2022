using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HighScore : MonoBehaviour
{

    ////////////SerializeField Var Defs:
    [SerializeField] private TMP_Text textUI;




    ////////////Private Var Defs:
    private float currentScore = 0;



    ////////////private Functions:
    public void UpdateScoreUI(float newBestHeight)
    {
        currentScore = newBestHeight;
        textUI.text = currentScore.ToString();
    }

}