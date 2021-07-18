using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElements : MonoBehaviour
{

    public TMP_Text usernameText;
    public TMP_Text wavesText;

    public void NewScoreElement(string _username, int _waves)
    {
        usernameText.text = _username;
        wavesText.text = _waves.ToString();
    }
}
