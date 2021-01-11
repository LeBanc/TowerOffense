using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// SoldierSelectionItem class manages a Soldier selection item
/// </summary>
public class SoldierSelectionItem : Button
{
    // Public element of the item
    public SoldierImage soldierImage;
    public Image squadColor;
    public Text level;
    public Text soldierName;
    public Text type;
    public HealthBar healthBar;
    public HealthBar experienceBar;
    public Text atkValue;
    public Text defValue;
    public Text speedValue;
    public Text friendValue;
    public Image selectedImage;

    // Displayed soldier
    private Soldier soldier;

    public Soldier Soldier
    {
        get { return soldier; }
    }

    /// <summary>
    /// Setup method initialize the Soldier Selection item with Soldier data
    /// </summary>
    /// <param name="_soldier">Soldier to display</param>
    public void Setup(Soldier _soldier)
    {
        soldier = _soldier;

        // If the soldier exists
        if(soldier != null)
        {
            // Display its squad color id it has one
            if(soldier.Squad != null)
            {
                squadColor.enabled = true;
                squadColor.color = soldier.Squad.Color;
            }
            else
            {
                squadColor.enabled = false;
                squadColor.color = Color.white;
            }            

            // Display soldier data
            soldierImage.Setup(_soldier);
            level.text = soldier.Data.soldierLevel.ToString();
            soldierName.text = soldier.Name;
            type.text = soldier.Data.typeName;

            healthBar.UpdateValue(soldier.CurrentHP, soldier.MaxHP);
            experienceBar.UpdateValue(Mathf.Min(soldier.CurrentXP,soldier.MaxXP), soldier.MaxXP);

            atkValue.text = string.Format("S:{0} M:{1} L:{2}", soldier.ShortRangeAttack, soldier.MiddleRangeAttack, soldier.LongRangeAttack);
            defValue.text = string.Format("S:{0} M:{1} L:{2} E:{3}", soldier.ShortRangeDefense, soldier.MiddleRangeDefense, soldier.LongRangeDefense, soldier.ExplosivesDefense);
            speedValue.text = soldier.Speed.ToString();

            friendValue.text = ""; // tbd
        }        
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
    }

    public override void Select()
    {
        base.Select();
        selectedImage.enabled = true;
    }

    public void Unselect()
    {
        selectedImage.enabled = false;
    }
}
