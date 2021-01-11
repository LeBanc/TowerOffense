using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAnimation : MonoBehaviour
{
    public Image image;
    public float fillTime = 1f;
    private Coroutine saveAnimation;

    public void StartAnimation()
    {
        image.enabled = true;
        saveAnimation = StartCoroutine(Animate());
    }

    public void StopAnimation()
    {
        if(saveAnimation != null) StopCoroutine(saveAnimation);
        image.enabled = false;
    }

    private IEnumerator Animate()
    {
        image.fillAmount = 0f;
        image.fillClockwise = true;

        while(true)
        {
            if(image.fillClockwise)
            {
                image.fillAmount += Time.deltaTime/fillTime;
                if (image.fillAmount >= 1f)
                {
                    image.fillAmount = 1f;
                    image.fillClockwise = false;
                }
                yield return null;
            }
            else
            {
                image.fillAmount -= Time.deltaTime/fillTime;
                if (image.fillAmount <= 0f)
                {
                    image.fillAmount = 0f;
                    image.fillClockwise = true;
                }
                yield return null;
            }
        }
    }



}
