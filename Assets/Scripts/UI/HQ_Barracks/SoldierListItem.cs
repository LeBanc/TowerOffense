using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// SoldierListItem class is linked to the prefab SoldierListItem and is used to display an overview of a soldier in the barracks'list
/// </summary>
public class SoldierListItem : Button
{
    // public UI elements
    public Image selected;
    public SoldierImage soldierImage;
    public Text gradeText;
    public Text nameText;
    public HealthBar hpBar;
    public HealthBar xpBar;
    public Image levelUpImage;
    public Image squadImage;
    public Text squadText;

    // private displayed soldier
    private Soldier soldier;

    // Events
    public delegate void SoldierListItemEventHandler(Soldier _soldier);
    public event SoldierListItemEventHandler OnSelection;


    public Soldier Soldier
    {
        get { return soldier; }
    }

    /// <summary>
    /// On Start, unselect the item
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        Unselect();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Select();
    }

    /// <summary>
    /// Setup method updates all graphical elements with soldier data
    /// </summary>
    /// <param name="_soldier">Soldier to display</param>
    public void Setup(Soldier _soldier)
    {
        // If soldier is not null, unsubscribe from events before continuing
        if(soldier != null)
        {
            // Unsubscribe from events
            soldier.OnNameChange -= UpdateName;
            soldier.OnImageChange -= UpdateImage;
            soldier.OnDataChange -= UpdateData;
        }

        soldier = _soldier;
        UpdateImage();
        UpdateName();
        UpdateData();

        if(soldier.Squad != null)
        {
            squadImage.enabled = true;
            squadImage.color = soldier.Squad.Color;
            squadText.text = soldier.Squad.GetSoldierIndex(soldier).ToString();
        }
        else
        {
            squadImage.enabled = false;
            squadText.text = "";
        }

        // Subscribe to events
        soldier.OnNameChange += UpdateName;
        soldier.OnImageChange += UpdateImage;
        soldier.OnDataChange += UpdateData;
    }

    /// <summary>
    /// OnDestroy, unsubscribes from events
    /// </summary>
    protected override void OnDestroy()
    {
        if(soldier != null)
        {
            // Unsubscribe from events
            soldier.OnNameChange -= UpdateName;
            soldier.OnImageChange -= UpdateImage;
            soldier.OnDataChange -= UpdateData;
        }
        base.OnDestroy();
    }

    /// <summary>
    /// Select method is used to display the item as selected
    /// </summary>
    public override void Select()
    {
        base.Select();
        selected.enabled = true;
        OnSelection?.Invoke(soldier);
    }

    /// <summary>
    /// Unselect method is used to display the item as unselected
    /// </summary>
    public void Unselect()
    {
        selected.enabled = false;
    }

    /// <summary>
    /// UpdateName method updates the soldier name UI
    /// </summary>
    public void UpdateName()
    {
        nameText.text = soldier.Name;
    }

    /// <summary>
    /// UpdateImage method updates the soldier image UI
    /// </summary>
    public void UpdateImage()
    {
        soldierImage.Setup(soldier);
    }

    /// <summary>
    /// UpdateData method updates the soldier data
    /// </summary>
    public void UpdateData()
    {
        // Update image for border color
        soldierImage.Setup(soldier);

        gradeText.text = PlayManager.data.ranks[soldier.Data.soldierLevel];
        hpBar.UpdateValue(soldier.CurrentHP, soldier.MaxHP);
        xpBar.UpdateValue(Mathf.Min(soldier.CurrentXP, soldier.MaxXP), soldier.MaxXP);

        levelUpImage.enabled = (soldier.CurrentXP >= soldier.MaxXP) && (soldier.MaxXP > 0);
    }
}
