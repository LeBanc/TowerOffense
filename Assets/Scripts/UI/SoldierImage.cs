using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SoldierImage class display soldier image with colored border
/// </summary>
public class SoldierImage : MonoBehaviour
{
    // Public images used in prefab
    public Image soldierImage;
    public Image border;

    /// <summary>
    /// Setup method display the soldier image and associated border
    /// </summary>
    /// <param name="soldier">Soldier to display</param>
    public void Setup(Soldier soldier)
    {
        // If the soldier is null (reset of the image)
        if(soldier == null)
        {
            soldierImage.sprite = null;
            soldierImage.enabled = false;
            border.color = Color.gray;
        }
        // Else, display the soldier image and change the border color
        else
        {
            // Display the soldier image
            soldierImage.enabled = true;
            soldierImage.sprite = soldier.Image;

            // Change the border color depending of soldier type
            switch (soldier.Data.soldierType)
            {
                case SoldierData.SoldierType.Attack:
                    border.color = Color.red;
                    break;
                case SoldierData.SoldierType.Defense:
                    border.color = Color.blue;
                    break;
                case SoldierData.SoldierType.Special:
                    border.color = Color.green;
                    break;
                default:
                    border.color = Color.gray;
                    break;
            }
        }
    }
}
