using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using allgameColor;
using Photon.Pun;
public class EnemyColor : MonoBehaviour
{
    private PhotonView photonView;
    public Renderer renderer;
    public CGameColors.GameColors color;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            color = (CGameColors.GameColors)UnityEngine.Random.Range(0, (int)CGameColors.GameColors.GC_TOTAL - 1);
            photonView.RPC("SetColor", RpcTarget.All, (int)color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void SetColor(int color)
    {   
        renderer.material.color = CGameColors.getDefColor((CGameColors.GameColors)color);
    }
}
