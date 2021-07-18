using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;

public class Skin : MonoBehaviour
{
    public DatabaseReference DBreference;
    public GameObject confirmScreen;
    public bool isOwned;
    public bool isEquipped;
    public int price;
    public int skinID;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TMP_Text>().text = price.ToString();
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPress()
    {
        if (isOwned)
        {
            SetToEquipped();
            StartCoroutine(UpdateEquippedSkin());
        }
        else
        {
            confirmScreen.SetActive(true);
            confirmScreen.GetComponent<Confirmation>().skin = gameObject;
            confirmScreen.GetComponent<Confirmation>().OnOpen();
        }
    }
    public void OnBought()
    {
        SetToOwned();
        Skin[] skins = transform.parent.GetComponentsInChildren<Skin>();
        List<int> skinList = new List<int>();
        foreach (Skin skin in skins)
        {
            if (skin.isOwned)
            {
                skinList.Add(skin.skinID);
            }
        }
        StartCoroutine(UpdateSkinsOwned(skinList));
    }
    public void SetToOwned()
    {
        isOwned = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void SetToEquipped()
    {
        Skin[] skins = transform.parent.GetComponentsInChildren<Skin>();
        foreach (Skin skin in skins)
        {
            skin.transform.GetChild(1).gameObject.SetActive(false);
            skin.isEquipped = false;
            if (skin.isOwned && skin.skinID != skinID)
            {
                skin.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        isEquipped = true;
        PlayerPrefs.SetInt("equippedSkin", skinID);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);
    }

    private IEnumerator UpdateSkinsOwned(List<int> skinList)
    {
        var DBTask = DBreference.Child("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("skinsOwned").SetValueAsync(skinList);
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("skinsOwned updated");
        }
    }

    private IEnumerator UpdateEquippedSkin()
    {
        var DBTask = DBreference.Child("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("equippedSkin").SetValueAsync(skinID);
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("equippedSkin updated");
        }
    }
}
