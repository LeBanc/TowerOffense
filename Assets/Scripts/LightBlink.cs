using UnityEngine;

/// <summary>
/// LightBlink class manages a light blink
/// </summary>
public class LightBlink : MonoBehaviour
{
    // Color of the light
    public Color blinkColor;
    // Intensity of the blick
    public float blinkIntensity;
    // Time when the light is off
    public float blackTime = 1f;
    // Time when the light is on
    public float blinkTime = 0.5f;

    // Light that should blink
    private Light blinkLight;
    
    // Counters
    private float blackCounter;
    private float lightCounter;
    
    /// <summary>
    /// At Start, fetches the light object and sets its color
    /// </summary>
    private void Start()
    {
        blinkLight = GetComponent<Light>();
        blinkLight.color = blinkColor;
    }

    /// <summary>
    /// OnDestroy, deactivates the blink
    /// </summary>
    private void OnDestroy()
    {
        Deactivate();
    }

    /// <summary>
    /// Activate method starts the blinking
    /// </summary>
    public void Activate()
    {
        // Reset the current blink
        Deactivate();
        // Add the update method to the PlayUpdate
        GameManager.PlayUpdate += LightUpdate;
    }

    /// <summary>
    /// Deactivate method stops the blinking
    /// </summary>
    private void Deactivate()
    {
        // Turn the light off
        blinkLight.intensity = 0f;
        // Remove the call of LightUpdate
        GameManager.PlayUpdate -= LightUpdate;
    }

    /// <summary>
    /// LightUpdate method updates the counter and sets the light on or off
    /// </summary>
    private void LightUpdate()
    {
        // If the off time is exceeded
        if(blackCounter >= blackTime)
        {
            // Rise intesity for half the blinkTime
            if(lightCounter < blinkTime /2)
            {
                blinkLight.intensity = Mathf.Lerp(blinkLight.intensity, blinkIntensity, blinkTime / 2);
            }
            else if(lightCounter >= blinkTime) // Reset the counter when time is esceeded
            {
                blackCounter = 0f;
                lightCounter = 0f;
            }
            else // Reduce the intensity for the second half of the blink time
            {
                blinkLight.intensity = Mathf.Lerp(blinkLight.intensity, 0f, blinkTime / 2);
            }
            lightCounter += Time.deltaTime;
        }
        else // increment the counter of Off time
        {
            blackCounter += Time.deltaTime;
        }
    }
}
