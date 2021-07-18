using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    private Vector2 touchOffset;
    private Vector2 currentTouchPos;
    private int touchID;
    private Vector2 defaultpos;
    private Vector2 defaultlocalpos;
    private bool isActive;
    public Vector2 moveDirection;

    void Awake()
    {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR

#else
        transform.parent.gameObject.SetActive(false);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultpos = transform.position;
        defaultlocalpos = transform.localPosition;
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Handle joystick movement
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR
        //Mobile touch input
        for (var i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began)
            {
                MobileButtonsCheck(new Vector2(touch.position.x, Screen.height - touch.position.y), touch.fingerId);
            }

            if (touch.phase == TouchPhase.Moved )
            {
                if(isActive && touchID == touch.fingerId)
                {
                    currentTouchPos = touch.position;
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                MobileButtonStop(touch.fingerId);
            }
        }
#else
        //Desktop mouse input for editor testing
        if (Input.GetMouseButtonDown(0))
        {
            MobileButtonsCheck(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y), -1);
        }

        if (Input.GetMouseButtonUp(0))
        {
            MobileButtonStop(-1);
        }

       currentTouchPos = Input.mousePosition;
#endif

        //Moving
        if (isActive)
        {
            transform.localPosition = new Vector3(currentTouchPos.x - touchOffset.x - transform.position.x, currentTouchPos.y - touchOffset.y - transform.position.y);
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -100.000f * transform.localScale.x, 100.000f * transform.localScale.x), Mathf.Clamp(transform.localPosition.y, -100.000f * transform.localScale.y, 100.000f * transform.localScale.y), transform.localPosition.z);
            moveDirection.x = transform.localPosition.x;
            moveDirection.y = transform.localPosition.y;
            Debug.Log(moveDirection.x);
            if (Mathf.Abs(moveDirection.x) < 19)
            {
                moveDirection.x = 0;
            }
            else
            {
                moveDirection.x = Mathf.Clamp(moveDirection.x / 100.000f * transform.localScale.x, -1.000f, 1.000f);
            }

            if (Mathf.Abs(moveDirection.y) < 19)
            {
                moveDirection.y = 0;
            }
            else
            {
                moveDirection.y = Mathf.Clamp(moveDirection.y / 100.000f * transform.localScale.y, -1.000f, 1.000f);
            }
        }
        else
        {
            transform.localPosition = new Vector3(defaultlocalpos.x, defaultlocalpos.y); 
            moveDirection = Vector2.zero;
        }
    }

    //Here we check if the clicked/tapped position is inside the joystick button
    void MobileButtonsCheck(Vector2 touchPos, int _touchID)
    {
        //Move controller
        if (Vector2.Distance(defaultpos, new Vector2(touchPos.x, Screen.height - touchPos.y)) <= 100.0f  * transform.localScale.x && !isActive)
        {
            isActive = true;
            touchOffset = new Vector2(touchPos.x - defaultpos.x, Screen.height - touchPos.y - defaultpos.y);
            currentTouchPos = new Vector2(touchPos.x, Screen.height - touchPos.y);
            touchID = _touchID;
        }
    }

    //Here we release the previously active joystick if we release the mouse button/finger from the screen
    void MobileButtonStop(int _touchID)
    {
        if (isActive && touchID == _touchID)
        {
            isActive = false;
            touchOffset = Vector2.zero;
            touchID = -1;
        }
    }
}
