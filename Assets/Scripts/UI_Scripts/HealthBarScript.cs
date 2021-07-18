using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Image Image;

    float speed = 100.0f;
    float amount = 3f;
    Vector2 storePosition;

    private bool isShaking = false;
    private float shakeTime = 0.5f;
    private float shakeTimer = 0f;

    private float lerpDuration = 0.5f;
    private float lerpVal;

    private float currValue;
    private float prevValue;

    public Playerstats playerstats;

    // In Player, call this to set HP
    private void Start()
    {
        SetMaxHP(100);
        storePosition = Image.transform.position;
        currValue = prevValue = 100;
    }

    private void Update()
    {
        if(playerstats == null)
        {
            return;
        }
        if(playerstats.isDamaged)
        {
            SetHP(playerstats.hp);
            playerstats.isDamaged = false;
        }
       
        if (isShaking)
        {
            if(shakeTimer < shakeTime)
            {
                shakeTimer += Time.deltaTime;
                Image.transform.position = new Vector2(Image.transform.position.x + (Mathf.Sin(shakeTimer * speed) * amount), Image.transform.position.y + (Mathf.Sin(shakeTimer * speed) * amount));
            }
            else
            {
                shakeTimer = 0f;
                isShaking = false;
                Image.transform.position = storePosition;
            }
        }
    }


    IEnumerator LerpHP()
    {
        float TimeElapsed = 0;

        while (TimeElapsed < lerpDuration)
        {
            lerpVal = Mathf.Lerp(prevValue, currValue, TimeElapsed / lerpDuration);
            TimeElapsed += Time.deltaTime;
            slider.value = lerpVal;
            yield return null;
        }

        lerpVal = currValue;
        slider.value = lerpVal;
        prevValue = currValue;
    }

    public void SetMaxHP(int maxhp)
    {
        slider.maxValue = maxhp;
        slider.value = maxhp;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHP(float health)
    {
        isShaking = true;
        slider.value = health;
        currValue = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void ReduceHP(float health)
    {
       //slider.value -= health;
       isShaking = true;
       currValue -= health;
       StartCoroutine(LerpHP());
       fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
