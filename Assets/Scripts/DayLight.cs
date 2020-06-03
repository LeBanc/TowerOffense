using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DayLight class is used to rotate and change color of the Sun during an attack
/// </summary>
public class DayLight : MonoBehaviour
{
    // Sun rotations (Euler)
    public Vector3 morningAngle;
    public Vector3 noonAngle;
    public Vector3 eveningAngle;

    // Sun colors
    public Color morningColor;
    public Color noonColor;
    public Color eveningColor;

    // Sun intensities
    public float morningIntensity;
    public float noonIntensity;
    public float eveningIntensity;

    // Sun rotations (Quaternion)
    private Quaternion morningQuaternion;
    private Quaternion noonQuaternion;
    private Quaternion eveningQuaternion;

    // Directional light component
    private Light dayLight;

    // Private time floats
    private float nightTime;
    private float counter = 0f;

    /// <summary>
    /// On Start, initialize Light component and Sun rotations (Quaternion)
    /// </summary>
    private void Start()
    {
        dayLight = GetComponent<Light>();

        morningQuaternion = Quaternion.Euler(morningAngle.x, morningAngle.y, morningAngle.z);
        noonQuaternion = Quaternion.Euler(noonAngle.x, noonAngle.y, noonAngle.z);
        eveningQuaternion = Quaternion.Euler(eveningAngle.x, eveningAngle.y, eveningAngle.z);
    }

    /// <summary>
    /// Morning method initialize light as morning light (rotation, color and intensity)
    /// </summary>
    /// <param name="time">Time at which the night should come</param>
    public void Morning(float time)
    {
        // Set up private parameters
        nightTime = time + 10f;
        counter = 0f;

        // Subscribe to PlayUpdate event
        GameManager.PlayUpdate += LightUpdate;

        // Initialize light parameters
        dayLight.enabled = true;
        dayLight.color = morningColor;
        dayLight.intensity = morningIntensity;
        transform.rotation = morningQuaternion;
    }

    /// <summary>
    /// Night method declares the Night as come
    /// </summary>
    public void Night()
    {
        // Disable light and unsubscribe event
        dayLight.enabled = false;
        GameManager.PlayUpdate -= LightUpdate;
    }

    /// <summary>
    /// LightUpdate is the Update method of the light
    /// </summary>
    public void LightUpdate()
    {
        counter += Time.deltaTime;

        // If the counter is under half the time befor the night, it is morning (lerp between morning and noon)
        if(counter < nightTime / 2)
        {
            // Compute the value between morning and noon at which we are
            float _delta = 2 * counter / nightTime;
            // Lerp color, intensity and rotation with this delta value
            dayLight.color = Color.Lerp(morningColor, noonColor, Mathf.Pow(_delta,1/5f));
            dayLight.intensity = Mathf.Lerp(morningIntensity, noonIntensity, Mathf.Pow(_delta, 1/5f));
            transform.rotation = Quaternion.Lerp(morningQuaternion, noonQuaternion, Mathf.Pow(_delta, 1/2f));
        }
        // Else it is the afternoon (lerp between noon and evening)
        else
        {
            // Compute the value between noon and evening at which we are
            float _delta = 2 * counter / nightTime -1;
            // Lerp color, intensity and rotation with this delta value
            dayLight.color = Color.Lerp(noonColor, eveningColor, Mathf.Pow(_delta, 5f));
            dayLight.intensity = Mathf.Lerp(noonIntensity, eveningIntensity, Mathf.Pow(_delta, 5f));
            transform.rotation = Quaternion.Lerp(noonQuaternion, eveningQuaternion, Mathf.Pow(_delta, 2f));
        }


    }

}
