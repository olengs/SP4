using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class Shop : MonoBehaviour
{
    public DatabaseReference DBreference;
    public TMP_Text gemText;
    private int gemCount;
    public GameObject skinContent;
    public GameObject upContent;


    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(PlayerPrefs.GetString("UserID"));
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        StartCoroutine(GetGemCountFromDataBase());

        StartCoroutine(GetSkinsOwnedFromDataBase());

        StartCoroutine(GetEquippedSkinFromDataBase());

        for(int i =0;i<4;++i)
            StartCoroutine(GetUpgradesAmountFromDataBase(i));
    }


    public void showUpContent()
    {
        upContent.SetActive(true);
        skinContent.SetActive(false);
    }

    public void showSkinContent()
    {
        upContent.SetActive(false);
        skinContent.SetActive(true);
    }

    private IEnumerator GetGemCountFromDataBase()
    {
        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("gems").GetValueAsync();
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            gemCount = int.Parse(snapshot.Value.ToString());
        }
    }

    private IEnumerator GetEquippedSkinFromDataBase()
    {
        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("equippedSkin").GetValueAsync();
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            int skinID = int.Parse(snapshot.Value.ToString());
            skinContent.transform.GetChild(skinID).GetComponent<Skin>().SetToEquipped();
        }
    }
    private IEnumerator GetSkinsOwnedFromDataBase()
    {
        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("skinsOwned").GetValueAsync();
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            for (int i = 0; i < snapshot.ChildrenCount; ++i)
            {
                int skinsID = int.Parse(snapshot.Child(i.ToString()).Value.ToString());
                skinContent.transform.GetChild(i).GetComponent<Skin>().SetToOwned();
            }
        }
    }

    private IEnumerator GetUpgradesAmountFromDataBase(int id)
    {
        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("upgrades").Child(id.ToString()).GetValueAsync();
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            int upgradesamt = int.Parse(snapshot.Value.ToString());
            if (upgradesamt >= 5)
                upContent.transform.GetChild(id).GetComponent<Upgrade>().SetToMaxed();
            else
                upContent.transform.GetChild(id).GetChild(3).GetComponent<TMP_Text>().text = upgradesamt.ToString();


        }
    }

    // Update is called once per frame
    void Update()
    {
         gemText.text = "<mspace=0.75em>" + gemCount.ToString() + "</mspace>";
    }

    public void SetGemCount(int _gemCount)
    {
        gemCount = _gemCount;
        StartCoroutine(UpdateGemCount(_gemCount));
    }

    private IEnumerator UpdateGemCount(int _gemCount)
    {
        //var DBTask = DBreference.Child("users").Child(PlayerPrefs.GetString("userID")).Child("gems").SetValueAsync(_gemCount);
        var DBTask = DBreference.Child("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("gems").SetValueAsync(_gemCount);
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("gemcount updated");
        }
    }

    public int GetGemCount()
    {
        return gemCount;
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
