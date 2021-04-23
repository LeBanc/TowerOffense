using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SoldierSelectionItem class manages a Soldier selection item
/// SoldierSelectionItem requires the GameObject to have a SelectedButton component
/// </summary>
[RequireComponent(typeof(SelectedButton))]
public class SoldierSelectionItem : MonoBehaviour
{
    // Public element of the item
    public SoldierImage soldierImage;
    public Image squadColor;
    public Text soldierName;
    public Text type;
    public HealthBar healthBar;
    public HealthBar experienceBar;
    public Text atkValue;
    public Text defValue;
    public Text speedValue;
    public Text friendValue;

    // private mandatory SelectedButton
    private SelectedButton button;

    // Events
    public delegate void SoldierSelectionItemEventHandler(SoldierSelectionItem _item);
    public event SoldierSelectionItemEventHandler OnSelection;

    // Displayed soldier
    private Soldier soldier;

    public Soldier Soldier
    {
        get { return soldier; }
    }

    /// <summary>
    /// On Start, fetch the SelectedButton, subscribe to event and unselect the item
    /// </summary>
    private void Awake()
    {
        button = GetComponent<SelectedButton>();
        if (button != null)
        {
            button.OnSelection += Select;
        }
        else
        {
            Debug.LogError("[SoldierSelectionItem] SelectedButton component not found!");
        }

        Unselect();
    }

    /// <summary>
    /// OnDestroy, unsubscribes from events
    /// </summary>
    void OnDestroy()
    {
        if (button != null)
        {
            button.OnSelection -= Select;
            button.onClick.RemoveAllListeners();
        }
        OnSelection = null;
    }

    /// <summary>
    /// Setup method initialize the Soldier Selection item with Soldier data
    /// </summary>
    /// <param name="_soldier">Soldier to display</param>
    public void Setup(Soldier _soldier, Squad _squad, Soldier _selectedSoldier)
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
            soldierImage.Setup(_soldier, true);
            soldierName.text = soldier.Name;
            type.text = soldier.Data.typeName;

            healthBar.UpdateValue(soldier.CurrentHP, soldier.MaxHP);
            experienceBar.UpdateValue(Mathf.Min(soldier.CurrentXP,soldier.MaxXP), soldier.MaxXP);

            atkValue.text = string.Format("S:{0} M:{1} L:{2}", soldier.ShortRangeAttack, soldier.MiddleRangeAttack, soldier.LongRangeAttack);
            defValue.text = string.Format("S:{0} M:{1} L:{2} E:{3}", soldier.ShortRangeDefense, soldier.MiddleRangeDefense, soldier.LongRangeDefense, soldier.ExplosivesDefense);
            speedValue.text = soldier.Speed.ToString();

            // Compute friendship points and display a "+" for each point
            friendValue.text = "";
            int _friendshipPoints = soldier.ComputeFriendship(_squad, _selectedSoldier);
            for (int i=0; i < _friendshipPoints; i++)
            {
                friendValue.text = friendValue.text + "+";
            }
        }        
    }

    /// <summary>
    /// Select method is used to display the item as selected
    /// </summary>
    private void Select()
    {
        OnSelection?.Invoke(this);
    }

    /// <summary>
    /// Unselect method is used to display the item as unselected
    /// </summary>
    public void Unselect()
    {
        if (button != null) button.Unselect();
    }
}
