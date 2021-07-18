using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    public DatabaseReference DBreference;
    public Transform leaderboardContent;
    public GameObject leadboardElement;

    // Start is called before the first frame update
    void Start()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Test10");
        StartCoroutine(LoadLeaderboardData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator LoadLeaderboardData()
    {
        Debug.Log("Test1");
        //FirebaseDatabase.DefaultInstance
        //    .GetReference("users")
        //    .Child
       
        var DBTask = DBreference.Child("users").OrderByChild("waves").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("Test2");
            DataSnapshot snapshot = DBTask.Result;

            foreach (Transform child in leaderboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int waves = int.Parse(childSnapshot.Child("waves").Value.ToString());

                GameObject leaderboardElement = Instantiate(leadboardElement, leaderboardContent);
                leaderboardElement.GetComponent<ScoreElements>().NewScoreElement(username, waves);
               
            }
        }

    }
    public void pressBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
