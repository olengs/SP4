using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TwitchIntegrationUI : MonoBehaviour
{
    public GameObject twitchChat = null;
    public GameObject twitchChatPrefab;
    public TMP_InputField username;
    public TMP_InputField channel;
    public TMP_InputField oauthtoken;
    public TMP_Text status;

    // Start is called before the first frame update
    void Start()
    {
        if (twitchChat != null)
        {
            if (twitchChat.GetComponent<TwitchChat>().CheckConnected())
            {
                status.text = "Connected";
                status.color = new Color(0, 1, 0);
            }
            else
            {
                status.text = "Not Connected";
                status.color = new Color(1, 0, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (twitchChat != null)
        {
            if (twitchChat.GetComponent<TwitchChat>().CheckConnected())
            {
                status.text = "Connected";
                status.color = new Color(0, 1, 0);
            }
            else
            {
                status.text = "Not Connected";
                status.color = new Color(1, 0, 0);
            }
        }
    }

    public void GetTokenButtonPressed()
    {
        Application.OpenURL("https://twitchapps.com/tmi/");
    }

    public void ConnectButtonPressed()
    {
        twitchChat = Instantiate(twitchChatPrefab);
        twitchChat.GetComponent<TwitchChat>().TryConnect(username.text, channel.text, oauthtoken.text);
        if (twitchChat.GetComponent<TwitchChat>().CheckConnected())
        {
            status.text = "Connected";
            status.color = new Color(0, 1, 0);
        }
        else
        {
            twitchChat = null;
        }
    }

    public void DisconnectButtonPressed()
    {
        if (twitchChat != null)
        {
            twitchChat.GetComponent<TwitchChat>().Disconnect();
            Destroy(twitchChat);
            twitchChat = null;
        }
        status.text = "Not Connected";
        status.color = new Color(1, 0, 0);
    }

    public void BackButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
