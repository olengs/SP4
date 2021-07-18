using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using allgameColor;
public class EnergyBarScript : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public CGameColors.GameColors color;
    public Playerstats playerstats;
    
    public float energycost = 10f;

    // Update is called once per frame
    void Update()
    {
        if (playerstats == null)
        {
            return;
        }

        SetEnergy(playerstats.energy);

        fill.color = CGameColors.getDefColor(color);
    }

    public void SetMaxEnergy(int maxEnergy)
    {
        slider.maxValue = maxEnergy;
        slider.value = maxEnergy;
    }

    public void SetEnergy(float energy)
    {
        if(energy <= 0)
        {
            slider.value = 0;
        }
        else
        {
            slider.value = energy;
        }
        
    }
}
