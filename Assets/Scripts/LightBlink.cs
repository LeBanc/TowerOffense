using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlink : MonoBehaviour
{
    public Color blinkColor;
    public float blinkIntensity;
    public float blackTime = 1f;
    public float blinkTime = 0.5f;

    private Light blinkLight;
    
    private float blackCounter;
    private float lightCounter;
    

    private void Start()
    {
        blinkLight = GetComponent<Light>();
        blinkLight.color = blinkColor;
    }

    private void OnDestroy()
    {
        Deactivate();
    }

    public void Activate()
    {
        Deactivate();
        GameManager.PlayUpdate += LightUpdate;
    }

    private void Deactivate()
    {
        blinkLight.intensity = 0f;
        GameManager.PlayUpdate -= LightUpdate;
    }

    private void LightUpdate()
    {
        if(blackCounter >= blackTime)
        {
            if(lightCounter < blinkTime /2)
            {
                blinkLight.intensity = Mathf.Lerp(blinkLight.intensity, blinkIntensity, blinkTime / 2);
            }
            else if(lightCounter >= blinkTime)
            {
                blackCounter = 0f;
                lightCounter = 0f;
            }
            else
            {
                blinkLight.intensity = Mathf.Lerp(blinkLight.intensity, 0f, blinkTime / 2);
            }
            lightCounter += Time.deltaTime;
        }
        else
        {
            blackCounter += Time.deltaTime;
        }
    }
}
