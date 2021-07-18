#region using directives
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using allgameColor;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
#endregion

public class Player : MonoBehaviour
{
    #region variables
    public int playerID;
    private float moveSpeed = 12f;
    private Camera mainCamera;
    public GameObject laser;
    public GameObject laserPrefab;
    public CGameColors.GameColors laserColor;
    private Rigidbody rigidbody;
    public float raydist;
    public GameObject movementjoystick;
    public GameObject directionjoystick;
    private MeshRenderer renderer;
    public Playerstats playerstats;

    private AudioSource audioSource;

    public PhotonView photonView;
    public GameObject Players;
    #endregion

    #region init
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        GameObject temp = GameObject.Find("HealthBar");
        temp = GameObject.Find("EnergyBar");
        renderer = GetComponentInChildren<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        photonView = GetComponent<PhotonView>();
        playerID = photonView.ViewID;
        playerstats = GetComponent<Playerstats>();
        playerstats.ID = this.playerID;
        playerstats.photonView = this.photonView;
        laserColor = 0;

        Players = GameObject.Find("Player");
        transform.SetParent(Players.transform);
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        renderer.material.color = CGameColors.getDefColor(laserColor);

        if (!photonView.IsMine) return;

        CheckforMovement();
        UpdateMousePos();

        //raydist = Laser.max_laserlen;
        CheckMousePresses();
        CheckColorKeyPresses();
    }
    #endregion

    #region player movement and direction
    void CheckforMovement()
    {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR
        Vector3 moveInput = new Vector3(movementjoystick.GetComponent<Joystick>().moveDirection.x, 0f, movementjoystick.GetComponent<Joystick>().moveDirection.y);
        Vector3 moveVelocity = moveInput * moveSpeed;
        rigidbody.velocity = moveVelocity;
#else
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        Vector3 moveVelocity = moveInput * moveSpeed;
        rigidbody.velocity = moveVelocity;

#endif
    }

    void UpdateMousePos()
    {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR
        transform.LookAt(new Vector3(directionjoystick.GetComponent<Joystick>().moveDirection.x + transform.position.x, transform.position.y, directionjoystick.GetComponent<Joystick>().moveDirection.y +  + transform.position.z));
#else
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            rayLength = (pointToLook - transform.position).magnitude * 0.5f;
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.cyan);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
#endif
    }

    #endregion

    #region mouseclicks
    public void CheckMousePresses()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (laser == null)
            {
                photonView.RPC("EnableLaser", RpcTarget.All);
            }
            audioSource.Play();
        }
        if (Input.GetMouseButton(0))
        {
            if (laser == null)
            {
                if (playerstats.energy > Playerstats.min_energy_value)
                {
                    photonView.RPC("EnableLaser", RpcTarget.All);
                    audioSource.Play();
                }
                return;
            }
            else
            {
                photonView.RPC("UpdateLaser", RpcTarget.All);
            }
        }
        if (Input.GetMouseButtonUp(0) || playerstats.energy <= 0)
        {
            photonView.RPC("DisableLaser", RpcTarget.All);
            audioSource.Stop();
        }
    }

    [PunRPC]
    void EnableLaser()
    {
        playerstats.OnLaser();
        laser = LaserPool.SharedInstance.FetchLaser();
        laser.GetComponent<Laser>().color = laserColor;
        laser.GetComponent<Laser>().origin = transform.position;
        laser.GetComponent<Laser>().dir = transform.forward;
        laser.GetComponent<Laser>().laserlen = Laser.max_laserlen;
        //laser.GetComponent<Laser>().laserlen = raydist;
        laser.GetComponent<Laser>().attachedtoplayer = true;
    }
    
    [PunRPC]
    void UpdateLaser()
    {
        if (playerstats.energy <= 0)
        {
            photonView.RPC("DisableLaser", RpcTarget.All);
            return;
        }
        Ray wallCheckRay = new Ray();
        wallCheckRay.origin = transform.position;
        wallCheckRay.direction = transform.forward;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(wallCheckRay, 10);
        Laser laz = laser.GetComponent<Laser>();
        float raylen = Laser.max_laserlen;
        foreach(RaycastHit hit in hits)
        {
            if (hit.distance <= Laser.max_laserlen / 0.5f && hit.collider != laser.GetComponentInChildren<Collider>())
            {
                if (hit.collider.tag == "LaserCollider")
                {
                    raylen = laz.laserlen;
                    Debug.Log("ray hit laser");
                    break;
                }
                raylen = hit.distance * 0.5f;
                break;
            }
            else
            {
                raylen = Laser.max_laserlen;
            }
        }
        if (raydist < 0)
        {
            Debug.Log("raycast dist < 0");
            return;
        }
        laser.GetComponent<Laser>().color = laserColor;
        laser.GetComponent<Laser>().origin = transform.position;
        laser.GetComponent<Laser>().dir = transform.forward;
        laser.GetComponent<Laser>().attachedtoplayer = true;
        laser.GetComponent<Laser>().laserlen = raylen;
    }
    [PunRPC]
    void DisableLaser()
    {
        playerstats.OffLaser();
        if (laser == null)
            return;
        Laser theLaser = laser.GetComponent<Laser>();
        theLaser.transform.position = new Vector3(-5000, -5000, -5000);
        //theLaser.laserlen = Laser.max_laserlen;
        if (theLaser.JoinedLaser != null)
        {
            theLaser.JoinedLaser.GetComponent<Laser>().parent[0] = null;
            theLaser.JoinedLaser.GetComponent<Laser>().parent[1] = null;
            theLaser.JoinedLaser.SetActive(false);
            theLaser.JoinedLaser = null;
            Debug.Log("joinedLaser destroyed");
        }
        laser.SetActive(false);
        laser = null;
    }
#endregion

    #region colorChange
    void CheckColorKeyPresses()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            photonView.RPC("changeColor", RpcTarget.All, CGameColors.GameColors.GC_RED);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            photonView.RPC("changeColor", RpcTarget.All, CGameColors.GameColors.GC_BLUE);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("changeColor", RpcTarget.All, CGameColors.GameColors.GC_YELLOW);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            photonView.RPC("changeColor", RpcTarget.All, CGameColors.GameColors.GC_GREEN);
        }
        
    }

    [PunRPC]
    public void changeColor(CGameColors.GameColors color)
    {
        this.laserColor = color;
    }
    #endregion

    #region collision
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            playerstats.hp -= 10f;
            playerstats.isDamaged = true;
        }
        else if(other.tag == "HealthPickUp")
        {
            if(other.GetComponent<HealthPickUp>().fake)
            {
                playerstats.hp -= 10f;
                playerstats.isDamaged = true;
            }
            else
            {
                playerstats.hp += 10f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
    }

    #endregion
}
