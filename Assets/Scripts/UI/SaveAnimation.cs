using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SaveAnimation class defines the Save animation
/// The save animation is currently an image filling then disapearing
/// </summary>
public class SaveAnimation : MonoBehaviour
{
    // Save animation image
    public Image image;
    // Time to fill the image
    public float fillTime = 1f;
    // Animation coroutine
    private Coroutine saveAnimation;

    /// <summary>
    /// StartAnimation method starts the animation
    /// </summary>
    public void StartAnimation()
    {
        image.enabled = true;
        saveAnimation = StartCoroutine(Animate());
    }

    /// <summary>
    /// StopAnimation method stops the animation
    /// </summary>
    public void StopAnimation()
    {
        if(saveAnimation != null) StopCoroutine(saveAnimation);
        image.enabled = false;
    }

    /// <summary>
    /// Animate coroutine defines the animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator Animate()
    {
        // Clear the image filling amount (no image displayed)
        image.fillAmount = 0f;
        image.fillClockwise = true;

        while(true)
        {
            // If the image is in filling state
            if(image.fillClockwise)
            {
                // Increment the fill amount
                image.fillAmount += Time.deltaTime/fillTime;
                // If the image is fully filled
                if (image.fillAmount >= 1f)
                {
                    image.fillAmount = 1f;
                    // Change the filling direction to switch to unfill state
                    image.fillClockwise = false;
                }
                yield return null;
            }
            else // The image is in unfilling state
            {
                // Decrment the fill amount
                image.fillAmount -= Time.deltaTime/fillTime;
                // If the image is completly hiden
                if (image.fillAmount <= 0f)
                {
                    image.fillAmount = 0f;
                    // Change the filling direction to switch to fill state
                    image.fillClockwise = true;
                }
                yield return null;
            }
        }
    }



}
