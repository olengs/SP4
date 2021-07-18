using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

namespace GameplayDatabaseManager
{
    public class data_output
    {
        public DataSnapshot snapshot;
        public string id;
        public data_output(DataSnapshot DSS, string id)
        {
            this.snapshot = DSS;
            this.id = id;
        }
    }
    public class DataManager : MonoBehaviour
    {
        public DatabaseReference DBreference;
        private DataSnapshot output;
        private Queue<data_output> data_Outputs;
        public int QueueRequestsCount;
        

        public void Start()
        {
            data_Outputs = new Queue<data_output>();
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
            QueueRequestsCount = 0;
        }

        public IEnumerator GetData(string reference, string[] child, string id)
        {
            //var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("VzCsEWmN8JeG6pnE27An7gFIJ2s1").Child("upgrades").Child(upgradeID.ToString()).GetValueAsync();
            QueueRequestsCount++;
            var togetchild = FirebaseDatabase.DefaultInstance.GetReference("users");
            foreach (string s in child)
            {
                togetchild = togetchild.Child(s);
            }
            var DBTask = togetchild.GetValueAsync();
            // wait till task complete
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                data_Outputs.Enqueue(new data_output(DBTask.Result, id)); 
                
            }
        }

        public void StartGettingData(string reference, string[] child, string id)
        {
            StartCoroutine(GetData(reference, child,id));
        }

        public bool GetOutput(out data_output data)
        {
            if(data_Outputs.Count > 0)
            {
                data = data_Outputs.Dequeue();
                --QueueRequestsCount;
                return true;
            }
            data = null;
            return false;
        }
    }
}