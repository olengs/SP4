using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDespawn : MonoBehaviour
{
    public float duration = 1f;
    private void Start()
    {
        Destroy(gameObject, duration);
    }
}
