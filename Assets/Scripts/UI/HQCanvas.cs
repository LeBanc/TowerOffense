using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQCanvas is the class used to manage the overall Menu of HQ management
/// </summary>
public class HQCanvas : MonoBehaviour
{
    // Text label to display 'Day' value
    public Text day;
    // Text label to display 'Coins' value
    public Text coins;

    // CommandCenter element (on CommandCenter canvas, trigged by Tab-toggle)
    public HQCommandCenter commandCenter;

    /// <summary>
    /// Show method shows the whole HQCanvas and upadtes Day and Coins texts
    /// </summary>
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
        transform.SetAsLastSibling();
        UpdateDay();
        UpdateCoins();
    }

    /// <summary>
    /// Hide method hides the HQCanvas
    /// </summary>
    public void Hide()
    {
        transform.SetAsFirstSibling();
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// UpdateDay method updates the "Day" text with the current day value
    /// </summary>
    public void UpdateDay()
    {
        day.text = PlayManager.day.ToString();
    }

    /// <summary>
    /// UpdateCoins method updates the "Coins" text with the current coins value
    /// </summary>
    public void UpdateCoins()
    {
        coins.text = PlayManager.coins.ToString();
    }
}
