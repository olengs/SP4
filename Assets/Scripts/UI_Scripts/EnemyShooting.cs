using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    private float moveSpeed = 5.0f;
    public GameObject bulletPrefab;
    private Rigidbody m_rigid;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        player = GameObject.Find("DummyPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            GameObject proj = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
            Rigidbody p = proj.GetComponent<Rigidbody>();
            p.velocity = transform.forward * moveSpeed;
        }

        Vector3 direction;
        direction = player.transform.position - transform.position;

        transform.LookAt(new Vector3(direction.x, transform.position.y, direction.z));
    }
}
