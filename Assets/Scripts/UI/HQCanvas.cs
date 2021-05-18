using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public Button newDayButton;
    // Barrack tab images
    public Image barracksLevelUpImage;
    public Image memorialActiveImage;
    public Image intelServiceImage;

    // Events
    public delegate void HQCanvasEventHandler();
    public static event HQCanvasEventHandler OnSecondaryShortcut;

    private AudioUI audioUI;

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

        audioUI = GetComponent<AudioUI>();

        // Events to update Barracks Levelup Icon
        tabs.barracks.soldierUpgrade.levelupCanvas.OnLevelUp += UpdateLevelUp;
        NewSoldierCanvas.OnRecruitWithXP += UpdateLevelUp;

        // Events to update other tabs icons
        tabs.memorial.OnDisplayDead += UpdateMemorialIcon;
        tabs.intelligenceServices.OnDisplayTower += UpdateIntelServiceIcon;
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
        GameManager.PlayUpdate -= HQCanvasUpdate;

        tabs.barracks.soldierUpgrade.levelupCanvas.OnLevelUp -= UpdateLevelUp;
        NewSoldierCanvas.OnRecruitWithXP -= UpdateLevelUp;
        tabs.memorial.OnDisplayDead -= UpdateMemorialIcon;
        tabs.intelligenceServices.OnDisplayTower -= UpdateIntelServiceIcon;
    }

    /// <summary>
    /// Hide method hides the HQCanvas
    /// </summary>
    public override void Hide()
    {
        tabs.HideAll();
        GameManager.PlayUpdate -= HQCanvasUpdate;
        base.Hide();
    }

    /// <summary>
    /// Init refresh the canvas data (update Day and Coins texts) and select the default selectable
    /// </summary>
    public void Init()
    {
        GameManager.PlayUpdate -= HQCanvasUpdate;
        tabs.comCenterTab.isOn = true;
        tabs.Refresh();
        UpdateDay();
        UpdateWorkforce();
        UpdateLevelUp();
        newDayButton.Select();
        GameManager.PlayUpdate += HQCanvasUpdate;
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

    /// <summary>
    /// UpdateMemorial method display or hide the Memorial icon
    /// </summary>
    /// <param name="_displayIcon">True to display, false to hide</param>
    public void UpdateMemorialIcon(bool _displayIcon)
    {
        memorialActiveImage.enabled = _displayIcon;
    }

    /// <summary>
    /// UpdateIntelServiceIcon method display or hide the Intel. Services icon
    /// </summary>
    /// <param name="_displayIcon">True to display, false to hide</param>
    public void UpdateIntelServiceIcon(bool _displayIcon)
    {
        intelServiceImage.enabled = _displayIcon;
    }

    /// <summary>
    /// HQCanvasUpdate is the Update method of the HQCanvas
    /// </summary>
    private void HQCanvasUpdate()
    {
        // Check Inputs for right or left selection buttons => Change tabs
        if (Input.GetButtonDown("RightSelection"))
        {
            tabs.SelectRightTab();
        }
        else if (Input.GetButtonDown("LeftSelection"))
        {
            tabs.SelectLeftTab();
        }

        // Check Inputs for primary shortcut button => New Day button
        if(Input.GetButtonDown("PrimaryShortcut"))
        {
            newDayButton.OnSubmit(new BaseEventData(EventSystem.current));
        }

        // Check Inputs for secondary shortcut button => Sort dropdowns
        if (Input.GetButtonDown("SecondaryShortcut"))
        {
            OnSecondaryShortcut?.Invoke();
        }
    }
}
