using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class RaiseEvents : MonoBehaviour, IOnEventCallback
{
    public const byte INTEVENT = 1;
    public delegate void IntEvent(int eventdata);
    public static event IntEvent _intevent;

    public const byte SPAWNLASER = 2;
    public delegate void SpawnLaserEvent(Laser eventdata);
    public static event SpawnLaserEvent _spawnLaserEvent;

    public const byte LIFEBAR = 3;
    public delegate void LifeBarEvent(int eventdata);
    public static event LifeBarEvent _lifebarEvent;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == INTEVENT)
        {
            //Explode Callback
            _intevent?.Invoke((int)photonEvent.CustomData);
        }
        if(eventCode == SPAWNLASER)
        {
            _spawnLaserEvent?.Invoke((Laser)photonEvent.CustomData);
        }
        if(eventCode == LIFEBAR)
        {
            _lifebarEvent?.Invoke((int)photonEvent.CustomData);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
