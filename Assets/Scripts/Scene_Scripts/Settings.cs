using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider music;
    public Slider sfx;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("musicvol", 0.5f);
        PlayerPrefs.SetFloat("sfxvol", 0.5f);

        music.value = PlayerPrefs.GetFloat("musicvol");
        sfx.value = PlayerPrefs.GetFloat("sfxvol");

    }

    // Update is called once per frame
    void Update()
    {
       
       
    }
    public void pressBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void setMusic()
    {
        PlayerPrefs.SetFloat("musicvol",music.value);
    }

    public void setSFX()
    {
        PlayerPrefs.SetFloat("sfxvol", sfx.value);
    }
}
