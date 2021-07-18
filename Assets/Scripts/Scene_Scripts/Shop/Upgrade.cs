using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;

public class Upgrade : MonoBehaviour
{
    public DatabaseReference DBreference;
    public GameObject confirmScreen;
    public bool isMaxed;
    public int price;
    private int upCount;

    public TMP_Text priceText;
    public TMP_Text countText;
    public int upgradeID;

    // Start is called before the first frame update
    void Start()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        priceText.text = price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPress()
    {
        if (isMaxed)
            return;
        confirmScreen.SetActive(true);
        confirmScreen.GetComponent<Confirmation2>().upgrade = gameObject;
        confirmScreen.GetComponent<Confirmation2>().OnOpen();
        StartCoroutine(GetUpgradeAmtFromDataBase());
    }

    public IEnumerator GetUpgradeAmtFromDataBase()
    {
        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("upgrades").Child(upgradeID.ToString()).GetValueAsync();
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            upCount = int.Parse(snapshot.Value.ToString());
            Debug.Log("1. " + upCount);
        }
    }
    public void OnBought()
    {
        Debug.Log("2. " + upCount.ToString());
        ++upCount;
        StartCoroutine(UpdateUpgradesOwned(upCount));
        countText.text = upCount.ToString();
        Debug.Log("3. " + upCount.ToString());
        if (upCount == 5)
            SetToMaxed();
    }
    public void SetToMaxed()
    {
        isMaxed = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    private IEnumerator UpdateUpgradesOwned(int upCount)
    {
        var DBTask2 = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("upgrades").Child(upgradeID.ToString()).SetValueAsync(upCount);
        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

        if (DBTask2.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
        }
        else
        {
            Debug.Log("upgrades updated");
        }
    }
}
