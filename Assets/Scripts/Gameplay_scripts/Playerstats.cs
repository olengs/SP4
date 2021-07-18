#region using directives
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
#endregion
public class Playerstats : MonoBehaviour
{
    #region variables
    public float hp;
    public float energy;
    public int ID;

    private const float recharge_amount = 20f;
    public const float min_energy_value = 20f;

    public float rechargetime;
    private const float recharge_cooldown = 0.1f;
    private const float penalised_recharge_cooldown = 0.1f;

    private const float energy_use = 20f;
    bool laserOn;
    public bool isDamaged;
    public PhotonView photonView;

    #endregion

    #region init
    // Start is called before the first frame update
    void Start()
    {
        hp = 100; //to be changed to max value
        energy = 100;
        rechargetime = 0;
        laserOn = false;
        isDamaged = false;
    }

    #endregion

    #region update stats
    // Update is called once per frame
    void Update()
    {
        // Nothing for now
        if (laserOn)
        {
            energy -= energy_use * Time.deltaTime;
            if(energy <= 0)
            {
                OffLaser();
            }
        }
        else
        {
            if (rechargetime <= 0)
            {
                energy = Mathf.Min(energy + recharge_amount * Time.deltaTime,100);
            }
            rechargetime -= Time.deltaTime;
        }
        photonView.RPC("Updateplayerstats", RpcTarget.Others, this.energy, this.hp, this.rechargetime, this.ID);
    }

    public void OnLaser()
    {
        laserOn = true;
    }

    public void OffLaser()
    {
        laserOn = false;
        rechargetime = energy <= 0 ? penalised_recharge_cooldown : recharge_cooldown;
    }
    #endregion

    #region rpc functions
    [PunRPC]
    public void Updateplayerstats(float new_energy, float new_health, float new_rechargetime, int _playerID)
    {
        if (photonView == null || photonView.IsMine)
        {
            return;
        }
        this.energy = new_energy;
        this.hp = new_health;
        this.rechargetime = new_rechargetime;
    }
    #endregion
}
