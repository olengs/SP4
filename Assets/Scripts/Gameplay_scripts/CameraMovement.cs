using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        Offset = new Vector3(0, 50f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
            this.transform.position = player.transform.position + Offset;
    }
}
