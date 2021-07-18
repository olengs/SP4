using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    public static GameObject[] AllWaypoints;
    // Start is called before the first frame update
    void Start()
    {
        AllWaypoints = new GameObject[transform.childCount];
        for (int i = 0; i < AllWaypoints.Length; ++i)
        {
            AllWaypoints[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
