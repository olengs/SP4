using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Cinemachine;

namespace Photon.Pun.Demo.Asteroids
{
    public class GameManagerScript : MonoBehaviourPunCallbacks
    {
        public static GameManagerScript Instance = null;

        public Text InfoText;

        public GameObject wayPoints;

        //public GameEnding gameEnding;

        public GameObject Enemies;
        public GameObject Players;
        public GameObject[] Prefabs;

        public Camera maincamera;

        public PlayerBar playerbar;
        public PlayerBar playerbar2;

        //ObjectPool objectPool;
        int playerNumber;

        public static bool isGameLoaded = false;

        public GameDataStorage GDS;
        public float Wave = 0;
        public float waveTimer = 0;
        public float spawnWaveTime = 2f;
        #region UNITY

        public void Awake()
        {
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public void Start()
        {
            Debug.Log("start");
            Hashtable props = new Hashtable
            {
                {GameScript.PLAYER_LOADED_LEVEL, true },
                {GameScript.PLAYER_LOADED_GAME, false }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool != null && this.Prefabs != null)
            {
                foreach (GameObject prefab in this.Prefabs)
                {
                    pool.ResourceCache.Add(prefab.name, prefab);
                }
            }
            //GDS.init();

            //objectPool = ObjectPool.instance;
            //objectPool.Init();
        }

        public void Update()
        {
            int numberOfEnemies = (int)Mathf.Pow(2 * Wave, 1.5f);
            if(waveTimer < spawnWaveTime)
            {
                waveTimer += Time.deltaTime;
            }
            else
            {
                waveTimer = 0;
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("MasterClient");
                    SpawnEnemies("enemy1");
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region COROUTINES


        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

/*        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {

            }
        }*/

        public override void OnPlayerLeftRoom(Realtime.Player otherPlayer)
        {
            CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(GameScript.PLAYER_LIVES))
            {
                CheckEndOfGame();
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }


            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start

            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(GameScript.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    // not all players loaded yet. wait:
                    Debug.Log("setting text waiting for players! ", this.InfoText);
                    InfoText.text = "Waiting for other players...";
                }
            }

        }

        #endregion

        // called by OnCountdownTimerIsExpired() when the timer ended
        private void StartGame()
        {
            Debug.Log("StartGame!");
            
            Vector3 position = new Vector3(-8.58f, 1f, -12.11f);
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            GameObject player = PhotonNetwork.Instantiate("Player", position, rotation, 0);  // avoid this call on rejoin (ship was network instantiated before)
            PhotonView photonView = player.GetComponent<PhotonView>();
           

            playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            //player.GetComponent<Player>().playerID = playerNumber;
            if (photonView.IsMine)
            {
                Debug.Log("run photonView.IsMine code");
                maincamera.GetComponent<CameraMovement>().player = player;
              
                playerbar.player = player;
                playerbar.mainViewID = photonView.ViewID;
                playerbar2.mainViewID = photonView.ViewID;
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("MasterClient");
                //SpawnEnemies("enemy1");
            }

            Hashtable props = new Hashtable
            {
                {GameScript.PLAYER_LOADED_LEVEL, true },
                {GameScript.PLAYER_LOADED_GAME, true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

/*            while (!CheckAllPlayerLoadedGame())
            {
                Debug.Log("waiting for all players to load game");
            }*/

            playerbar.Start();
            isGameLoaded = true;
            Debug.Log("max hp upgrade count: " + GameDataStorage.max_health);
            Debug.Log("max energy upgrade count: " + GameDataStorage.max_energy);
            Debug.Log("max energy_recharge upgrade count: " + GameDataStorage.energy_recharge);
            Debug.Log("max laser length upgrade count: " + GameDataStorage.laser_range);
        }

        public bool CheckAllPlayerLoadedGame()
        {
            foreach (Realtime.Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedGame;
                if (p.CustomProperties.TryGetValue(GameScript.PLAYER_LOADED_GAME, out playerLoadedGame))
                {
                    if ((bool)playerLoadedGame)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;

        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Realtime.Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;
                if (p.CustomProperties.TryGetValue(GameScript.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private void CheckEndOfGame()
        {

        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }
        private void SpawnEnemies(string tag)
        {
            Vector3[] positionArray = new Vector3[]
            {
                new Vector3(15f,2f,8f),
                new Vector3(23f, 2f, -22f),
                new Vector3(-14f, 2f, -51f),
                new Vector3(-18f, 2f, -62f),
                new Vector3(62f, 2f, -46f),
                new Vector3(71f, 2f, 12f),
            };

            Debug.Log("Spawning da enemi");

            //for (int i = 0; i < objectPool.enemyPoolDictionary[tag].Count; ++i)
            //{
            //    GameObject obj = FetchEnemies(tag);

            //    TestEnemyPathfinding pathfinding = obj.GetComponent<TestEnemyPathfinding>();

            //    pathfinding._patrolPoints.Add(wayPoints.transform.GetChild(i * 2));
            //    pathfinding._patrolPoints.Add(wayPoints.transform.GetChild(i * 2 + 1));

            //    Debug.Log("spawning Enemies " + i);
            //}
            GameObject obj;
            TestEnemyPathfinding pathfinding = null;
            for (int i = 0; i < 6; ++i)
            {
                int temp = UnityEngine.Random.Range(2, 4);

                for(int j = 0; j < temp; ++j)
                {
                    int random = UnityEngine.Random.Range(0, wayPoints.transform.childCount - 1);
                    
                    if(j == 0)
                    {
                        obj = PhotonNetwork.Instantiate("enemy1", wayPoints.transform.GetChild(random).position, Quaternion.identity, 0);
                        pathfinding = obj.GetComponent<TestEnemyPathfinding>();
                    }

                    for (int k = 0; k < pathfinding._patrolPoints.Count; ++k)
                    {
                        if(pathfinding._patrolPoints[k] == wayPoints.transform.GetChild(random))
                        {
                            k = 0;
                            random = UnityEngine.Random.Range(0, wayPoints.transform.childCount - 1);
                        }
                    }
                    pathfinding._patrolPoints.Add(wayPoints.transform.GetChild(random));
                }

                Debug.Log("spawning Enemies " + i);
            }
        }

        //public GameObject FetchEnemies(string tag)
        //{
        //    if (!objectPool.enemyPoolDictionary.ContainsKey(tag))
        //    {
        //        Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
        //        return null;
        //    }

        //    for (int i = 0; i < objectPool.enemyPoolDictionary[tag].Count; ++i)
        //    {
        //        if (!objectPool.enemyPoolDictionary[tag][i].activeInHierarchy)
        //        {
        //            objectPool.enemyPoolDictionary[tag][i].SetActive(true);
        //            return objectPool.enemyPoolDictionary[tag][i];
        //        }
        //    }

        //    return null;
        //}

        public void SpawnHealthPickUp(bool fake)
        {
            GameObject obj = PhotonNetwork.Instantiate("HpPickup", wayPoints.transform.GetChild(Random.Range(0, wayPoints.transform.childCount - 1)).position, Quaternion.identity, 0);
            obj.GetComponent<HealthPickUp>().fake = fake;
        }

        public void NextWave()
        {

        }
    }
}
