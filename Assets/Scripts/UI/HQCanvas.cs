using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQCanvas is the class used to manage the overall Menu of HQ management
/// </summary>
public class HQCanvas : UICanvas
{
    // Text label to display 'Day' value
    public Text day;
    // Text label to display 'Coins' value
    public Text coins;
    // TabToggle
    public HQTabToggle tabs;
    // Default selected component
    public Selectable defaultSelected;
    // Barrack tab Level up immage
    public Image barracksLevelUpImage;

    /// <summary>
    /// On Awake, get the Canvas component and subscribe to events
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        PlayManager.OnWorkforceUpdate += UpdateWorkforce;
        PlayManager.OnLoadSquadsOnNewDay += Hide;
        PlayManager.OnHQPhase += Show;
        PlayManager.OnHQPhase += Init;
        PlayManager.OnLoadGame += Init;

        tabs.barracks.soldierUpgrade.levelupCanvas.OnLevelUp += UpdateLevelUp;
        NewSoldierCanvas.OnRecruitWithXP += UpdateLevelUp;

    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnWorkforceUpdate -= UpdateWorkforce;
        PlayManager.OnLoadSquadsOnNewDay -= Hide;
        PlayManager.OnHQPhase -= Show;
        PlayManager.OnHQPhase -= Init;
        PlayManager.OnLoadGame -= Init;

        tabs.barracks.soldierUpgrade.levelupCanvas.OnLevelUp -= UpdateLevelUp;
        NewSoldierCanvas.OnRecruitWithXP -= UpdateLevelUp;
    }

    /// <summary>
    /// Hide method hides the HQCanvas
    /// </summary>
    public override void Hide()
    {
        tabs.HideAll();
        base.Hide();
    }

    /// <summary>
    /// Init refresh the canvas data (upadte Day and Coins texts) and select the default selectable
    /// </summary>
    public void Init()
    {
        tabs.comCenterTab.isOn = true;
        tabs.Refresh();
        UpdateDay();
        UpdateWorkforce();
        UpdateLevelUp();
        defaultSelected.Select();
    }

    /// <summary>
    /// UpdateDay method updates the "Day" text with the current day value
    /// </summary>
    public void UpdateDay()
    {
        day.text = PlayManager.day.ToString();
    }

    /// <summary>
    /// UpdateWorkforce method updates the "Coins" text with the current coins value
    /// </summary>
    public void UpdateWorkforce()
    {
        coins.text = PlayManager.workforce.ToString();
    }

    /// <summary>
    /// UpdateLevelUp method updates the visibility of the Update icon on the Barracks tab
    /// </summary>
    public void UpdateLevelUp()
    {
        barracksLevelUpImage.enabled = false;
        foreach(Soldier _s in PlayManager.soldierList)
        {
            if(_s.CanLevelUp && !_s.IsDead)
            {
                barracksLevelUpImage.enabled = true;
                return;
            }
        }
    }
}
