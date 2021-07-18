using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject movementjoystick;
    public GameObject directionjoystick;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = rigidbody.velocity;
        //Move Front/Back
        if (movementjoystick.GetComponent<Joystick>().moveDirection.y != 0)
        {
           velocity.y = movementjoystick.GetComponent<Joystick>().moveDirection.y;
        }

        //Move Left/Right
        if (movementjoystick.GetComponent<Joystick>().moveDirection.x != 0)
        {
            velocity.x = movementjoystick.GetComponent<Joystick>().moveDirection.x;
        }
         rigidbody.velocity = velocity;
    }
}
