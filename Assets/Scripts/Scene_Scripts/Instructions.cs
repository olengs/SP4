using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    public GameObject mobileControls;
    public GameObject pcControls;
    // Start is called before the first frame update
    void Start()
    {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR
        mobileControls.SetActive(true);
#else
        pcControls.SetActive(true);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
