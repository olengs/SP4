#region using directives
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
#endregion

public class PlayerBar : MonoBehaviour
{
    #region variables
    public GameObject player;
    public Player playerscript;
    public PhotonView photonView;
    public HealthBarScript hpbar;
    public EnergyBarScript energybar;
    public int mainViewID;
    #endregion

    #region init
    // Start is called before the first frame update
    public void Start()
    {
        if(player != null)
        {
            init();
        }
    }
    void init()
    {
        photonView = player.GetComponent<PhotonView>();
        hpbar = GetComponentInChildren<HealthBarScript>();
        energybar = GetComponentInChildren<EnergyBarScript>();
        hpbar.playerstats = player.GetComponent<Playerstats>();
        energybar.playerstats = player.GetComponent<Playerstats>();
        playerscript = player.gameObject.GetComponent<Player>();
        RaiseSendPlayerEvent();
    }
    #endregion

    #region PUN_CALLS
    public void RaiseSendPlayerEvent()
    {
        object data = photonView.ViewID;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEvents.INTEVENT, data, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEnable()
    {
        RaiseEvents._intevent += AddPlayerToScript;
    }

    public void OnDisable()
    {
        RaiseEvents._intevent -= AddPlayerToScript;
    }

    void AddPlayerToScript(int playerID)
    {
        if(playerID == mainViewID)
        {
            return;
        }
        if(player == null)
        {
            Debug.Log(playerID);
            Debug.Log("Adding player to second script");
            GameObject[] arr = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in arr)
            {
                if(go.GetComponent<PhotonView>().ViewID == playerID)
                {
                    this.player = go;
                    init();
                    break;
                }
            }
            if(this.player == null)
            {
                Debug.Log("cannot find player with ID" + playerID);
            }

        }
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }
        energybar.color = playerscript.laserColor;
    }
    #endregion
}
