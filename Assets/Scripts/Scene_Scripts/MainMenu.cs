using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void pressPlay()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void pressSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void pressInst()
    {
        SceneManager.LoadScene("Settings");
    }

    public void pressLB()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void pressShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void pressTwitch()
    {
        SceneManager.LoadScene("TwitchIntegrationScene");
    }
}
