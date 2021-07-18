using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject loginUI;
    public GameObject registerUI;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else if (instance!=null)
        {
            Debug.Log("Instance alr exists, destroying obj");
            Destroy(this);
        }
    }

    public void ClearScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

    public void LoginScreen()
    {
        ClearScreen();
        loginUI.SetActive(true);
    }

    public void RegisterScreen()
    {
        ClearScreen();
        registerUI.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
