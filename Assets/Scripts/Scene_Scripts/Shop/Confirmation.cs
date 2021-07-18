using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Confirmation : MonoBehaviour
{
    public TMP_Text cost;
    public GameObject skin;
    public GameObject notEnoughGems;
    public GameObject eventSystem;
    private int price;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnOpen()
    {
        
        price = skin.GetComponent<Skin>().price;
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
            skin.GetComponent<Skin>().OnBought();
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
