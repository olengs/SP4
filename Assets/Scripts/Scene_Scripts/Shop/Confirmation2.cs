using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Confirmation2 : MonoBehaviour
{
    public TMP_Text cost;
    public GameObject upgrade;
    public GameObject notEnoughGems;
    public GameObject eventSystem;
    private int price;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnOpen()
    {
        price = upgrade.GetComponent<Upgrade>().price;
        Debug.Log(price);
        cost.text = "Cost: " + price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmButton()
    {
        int gemCount = eventSystem.GetComponent<Shop>().GetGemCount();
        if (gemCount >= price)
        {
            eventSystem.GetComponent<Shop>().SetGemCount(gemCount - price);
            
            upgrade.GetComponent<Upgrade>().OnBought();
            gameObject.SetActive(false);
        }
        else
        {
            notEnoughGems.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void CancelButton()
    {
        gameObject.SetActive(false);
    }
}
