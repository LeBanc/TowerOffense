using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLight : MonoBehaviour
{
    public Vector3 morningAngle;
    public Vector3 noonAngle;
    public Vector3 eveningAngle;

    public Color morningColor;
    public Color noonColor;
    public Color eveningColor;

    public float morningIntensity;
    public float noonIntensity;
    public float eveningIntensity;


    private Quaternion morningQuaternion;
    private Quaternion noonQuaternion;
    private Quaternion eveningQuaternion;

    private Light dayLight;
    private float nightTime;
    private float counter = 0f;

    private void Start()
    {
        dayLight = GetComponent<Light>();

        morningQuaternion = Quaternion.Euler(morningAngle.x, morningAngle.y, morningAngle.z);
        noonQuaternion = Quaternion.Euler(noonAngle.x, noonAngle.y, noonAngle.z);
        eveningQuaternion = Quaternion.Euler(eveningAngle.x, eveningAngle.y, eveningAngle.z);
    }

    public void Morning(float time)
    {
        dayLight.enabled = true;
        nightTime = time + 10f;
        counter = 0f;
        GameManager.PlayUpdate += LightUpdate;

        dayLight.color = morningColor;
        dayLight.intensity = morningIntensity;
        transform.rotation = morningQuaternion;
    }

    public void Night()
    {
        dayLight.enabled = false;
        GameManager.PlayUpdate -= LightUpdate;
    }

    public void LightUpdate()
    {
        counter += Time.deltaTime;
        if(counter < nightTime / 2) //morning
        {
            float _delta = 2 * counter / nightTime;
            dayLight.color = Color.Lerp(morningColor, noonColor, Mathf.Pow(_delta,1/5f));
            dayLight.intensity = Mathf.Lerp(morningIntensity, noonIntensity, Mathf.Pow(_delta, 1/5f));
            transform.rotation = Quaternion.Lerp(morningQuaternion, noonQuaternion, Mathf.Pow(_delta, 1/2f));
        }
        else //afternoon
        {
            float _delta = 2 * counter / nightTime -1;
            dayLight.color = Color.Lerp(noonColor, eveningColor, Mathf.Pow(_delta, 5f));
            dayLight.intensity = Mathf.Lerp(noonIntensity, eveningIntensity, Mathf.Pow(_delta, 5f));
            transform.rotation = Quaternion.Lerp(noonQuaternion, eveningQuaternion, Mathf.Pow(_delta, 2f));
        }


    }

}
