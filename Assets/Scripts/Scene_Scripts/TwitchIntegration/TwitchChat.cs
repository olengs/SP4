using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.Asteroids;

public class TwitchChat : MonoBehaviour
{
    private string[] commands = new string[] {"#drophp", "#fakehp"};
    private int[] commandPoll = new int[] { 0, 0 };
    private TcpClient twitchClient = null;
    private StreamReader reader;
    private StreamWriter writer;
    private string username, oauthToken, channelName;
    private float fpollTimer = 0.0f;
    private bool bpollActive = false;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("testing");
        //Connect();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("testing");
        if (twitchClient != null)
        {
            if (!twitchClient.Connected)
            {
                Connect();
            }
            else if (SceneManager.GetActiveScene().name == "GameScene")
            {
                //ReadChat();
                if (bpollActive)
                {
                    fpollTimer += Time.deltaTime;
                    if (fpollTimer >= 120.0f)
                    {
                        bpollActive = false;
                        fpollTimer = 0.0f;
                        PollEnd();
                    }
                }
                else
                {
                    fpollTimer += Time.deltaTime;
                    if (fpollTimer >= 60.0f)
                    {
                        fpollTimer = 0.0f;
                        bpollActive = true;
                        SendPollBeginMsg();
                    }
                }
            }
            ReadChat();
        }
    }

    public bool CheckConnected()
    {
        if (twitchClient != null)
        {
            return true;
        }
        return false;
    }
    public void TryConnect(string _username, string _channelName, string _oauthToken)
    {
        username = _username;
        channelName = _channelName;
        oauthToken = _oauthToken;
        Connect();

    }

    public void Disconnect()
    {
        twitchClient = null;
    }

    private void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        Debug.Log(twitchClient.Connected);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        if (oauthToken.StartsWith("oauth:"))
        {
            oauthToken = oauthToken.Substring(6);
        }
        writer.WriteLine("PASS oauth:" + oauthToken);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();

        var message = reader.ReadLine();
        if (message == null)
        {
            twitchClient = null;
            Destroy(this);
        }
        print(message);
        if (message.Contains(":Invalid"))
        {
            twitchClient = null;
            Destroy(this);
        }
        if (twitchClient != null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void ReadChat()
    {
        //print("reading chat");
        //print(twitchClient.Available);
        //if (twitchClient.Available > 0)
        //{
        //    var message = reader.ReadLine();
        //    print(message);
        //}
        if (twitchClient.Available > 0)
        {
            print("reading chat2");
            var message = reader.ReadLine();
            print(message);
            if (message.Contains("PRIVMSG"))
            {
                // Get user's name
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(1, splitPoint - 1);

                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                print(chatName + ": " + message);
                if (bpollActive)
                {
                    if (message[0] == '#')
                    {
                        for (int i = 0; i < commands.Length; ++i)
                        {
                            if (message == commands[i])
                            {
                                ++commandPoll[i];
                                break;
                            }
                        }
                    }
                }
            }
            print(message);
        }
    }
    private void SendPollBeginMsg()
    {
        string msg = "PRIVMSG #" + channelName + " :Poll has begun! Use commands: ";
        for (int i = 0; i < commands.Length; ++i)
        {
            if (i != 0)
            {
                msg += ", ";
            }
            msg += commands[i];
        }
        msg += " to help or hinder " + channelName;
        writer.WriteLine(msg);
        writer.Flush();
        print("wrote");
    }

    private void PollEnd()
    {
        List<int> indexOfLargest = new List<int> { };
        int largest = 0;
        for (int i = 0; i < commandPoll.Length; ++i)
        {
            if (commandPoll[i] != 0)
            {
                if (commandPoll[i] > largest)
                {
                    largest = commandPoll[i];
                    indexOfLargest.Clear();
                    indexOfLargest.Add(i);
                }
                else if (commandPoll[i] == largest)
                {
                    indexOfLargest.Add(i);
                }
                commandPoll[i] = 0;
            }
        }
        if (indexOfLargest.Count != 0)
        {
            foreach (int index in indexOfLargest)
            {
                switch (index)
                {
                    case 0:
                        // add drophp command
                        GameManagerScript.Instance.SpawnHealthPickUp(false);
                        break;
                    case 1:
                        // add fakehp command
                        GameManagerScript.Instance.SpawnHealthPickUp(true);
                        break;
                }
            }
        }
        string msg = "PRIVMSG #" + channelName + " :Poll has ended! ";
        if (indexOfLargest.Count != 0)
        {
            for (int i = 0; i < indexOfLargest.Count; ++i)
            {
                if (i != 0)
                {
                    msg += ", ";
                }
                msg += commands[indexOfLargest[i]];
            }
            if (indexOfLargest.Count > 1)
            {
                msg += " have won the poll";
            }
            else
            {
                msg += " has won the poll";
            }
        }
        else
        {
            msg += " there were no votes";
        }
        writer.WriteLine(msg);
        writer.Flush();
        print("wrote");
    }
}
