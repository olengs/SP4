using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPool : MonoBehaviour
{
    public static LaserPool SharedInstance;
    public List<GameObject> pooledLasers;
    public int laserCount;
    public GameObject laserPrefab;
    public GameObject container;
    private void Awake()
    {
        SharedInstance = this;
    }
    void Start()
    {
        pooledLasers = new List<GameObject>();
        GameObject temp;

        for(int i = 0; i < laserCount; ++i)
        {
            temp = Instantiate(laserPrefab);
            temp.SetActive(false);
            temp.transform.SetParent(container.transform);
            pooledLasers.Add(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public GameObject FetchLaser()
    {
        for (int i = 0; i < pooledLasers.Count; ++i)
        {
            if(!pooledLasers[i].activeInHierarchy)
            {
                pooledLasers[i].SetActive(true);
                return pooledLasers[i];
            }
        }
        GameObject temp;
        temp = Instantiate(laserPrefab);
        temp.GetComponentInChildren<Collider>().gameObject.tag = "spawnedlaser";
        temp.SetActive(false);
        temp.transform.SetParent(container.transform);
        pooledLasers.Add(temp);
        return FetchLaser();
    }
}
