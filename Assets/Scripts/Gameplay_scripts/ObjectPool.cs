using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class ObjectPool : MonoBehaviour
{
    public List<Pool> pools;
    public Dictionary<string, List<GameObject>> enemyPoolDictionary;
    public static ObjectPool instance = null;
    // Enemy Positions
    Vector3[] positionArray = new Vector3[]
    {
        new Vector3(15f,1.3f,8f),
        new Vector3(23f, 1.3f, -22f),
        new Vector3(-14f, 1.3f, -51f),
        new Vector3(-18f, 1.3f, -62f),
        new Vector3(62f, 1.3f, -46f),
        new Vector3(71f, 1.3f, 12f),
    };
    private void Awake()
    {
        instance = this;
    }
    public void Init()
    {
        if(enemyPoolDictionary == null)
        {
            #region Testing
            enemyPoolDictionary = new Dictionary<string, List<GameObject>>();

            foreach (Pool pool2 in pools)
            {
                List<GameObject> enemyList = new List<GameObject>();

                for (int i = 0; i < pool2.size; ++i)
                {
                    GameObject obj = PhotonNetwork.InstantiateRoomObject("enemy1", positionArray[i], Quaternion.identity) as GameObject;
                    obj.SetActive(false);
                    enemyList.Add(obj);
                }

                enemyPoolDictionary.Add(pool2.tag, enemyList);
            }
            #endregion
        }
    }
}
