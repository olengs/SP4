using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using GameplayDatabaseManager;

public class GameDataStorage : MonoBehaviour
{
    const string reference = "users";

    string userID;

    public static int max_health;
    string[] healthUP_data_child;
    const string health_id = "hp";

    public static int max_energy;
    string[] energyUP_data_child;
    const string energy_id = "energy";

    public static int energy_recharge;
    string[] energyRecharge_data_child;
    const string energyRecharge_id = "energy_recharge";

    public static int laser_range;
    string[] laserRange_data_child;
    const string laserlen_id = "laserRange";

    // Start is called before the first frame update
    DataManager dm;
    void Start()
    {
        dm = new DataManager();
        userID = PlayerPrefs.GetString("UserID");
        healthUP_data_child = new string[]{ userID, "upgrades", "0" };
        energyUP_data_child = new string[] { userID, "upgrades", "1" };
        energyRecharge_data_child = new string[] { userID, "upgrades", "2" };
        laserRange_data_child = new string[] { userID, "upgrades", "3" };
    }

    public void init()
    {
        dm.StartGettingData(reference, healthUP_data_child, health_id);
        max_health = 0;
        dm.StartGettingData(reference, energyUP_data_child, energy_id);
        max_energy = 0;
        dm.StartGettingData(reference, energyRecharge_data_child, energyRecharge_id);
        energy_recharge = 0;
        dm.StartGettingData(reference, laserRange_data_child, laserlen_id);
        laser_range = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(dm.QueueRequestsCount <= 0)
        {
            return;
        }
        data_output output;
        if(dm.GetOutput(out output))
        {
            HandleOutput(output);
        }
    }

    void HandleOutput(data_output data)
    {
        int value = (int)data.snapshot.Value;
        if(data.id == health_id)
        {
            max_health = value;
        }
        else if(data.id == energy_id)
        {
            max_energy = value;
        }
        else if(data.id == energyRecharge_id)
        {
            energy_recharge = value;
        }
        else if(data.id == laserlen_id)
        {
            laser_range = value;
        }
    }
}
