using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQSquadHeader is related to a Squad element in the header bar of the CommandCenter Canvas
/// </summary>
public class HQSquadHeader : MonoBehaviour
{
    // Public elements of the Squad Header
    public Image background;
    public SelectedButton select;
    public Toggle engage;
    public SoldierImage soldier1Image;
    public SoldierImage soldier2Image;
    public SoldierImage soldier3Image;
    public SoldierImage soldier4Image;
    public HealthBar soldier1Healthbar;
    public HealthBar soldier2Healthbar;
    public HealthBar soldier3Healthbar;
    public HealthBar soldier4Healthbar;
    public Text atkShortValue;
    public Text atkMiddleValue;
    public Text atkLongValue;
    public Text defShortValue;
    public Text defMiddleValue;
    public Text defLongValue;
    public Text defExplosives;
    public Text speedValue;
    public Image rangeSelection;
    public Button unlockButton;

    // Events
    public delegate void SquadHeaderEventHandler(HQSquadHeader _squadHeader);
    public event SquadHeaderEventHandler OnSelection;

    // Private Squad displayed in the header
    private Squad squad;

    // Private canvas
    private Canvas canvas;

    #region Properties access
    public Squad Squad
    {
        get { return squad; }
    }
    #endregion

    /// <summary>
    /// At Awake, shows the Unlock button as Locked squad
    /// </summary>
    private void Awake()
    {
        unlockButton.gameObject.SetActive(true);
        canvas = GetComponent<Canvas>();

        // Set squad as locked
        unlockButton.interactable = false;

        // Events
        select.OnSelection += Select;
        PlayManager.OnHQPhase += UpdateSoldier1;
        PlayManager.OnHQPhase += UpdateSoldier2;
        PlayManager.OnHQPhase += UpdateSoldier3;
        PlayManager.OnHQPhase += UpdateSoldier4;
    }

    #region Canvas
    /// <summary>
    /// Show method displays the canvas
    /// </summary>
    public void Show()
    {
        canvas.enabled = true;
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public void Hide()
    {
        canvas.enabled = false;
    }
    #endregion

    /// <summary>
    /// Setup method links the Squad to the Header, update the graphic elements and links events
    /// </summary>
    /// <param name="_squad">Squad to display</param>
    public void Setup(Squad _squad)
    {
        // If there is a squad, hides the Unlock button
        unlockButton.gameObject.SetActive(false);
        // Set the squad as the displayed squad
        squad = _squad;

        // Update background color and link event
        UpdateColor();
        squad.OnColorChange += UpdateColor;

        // Update soldiers image and link events
        UpdateSoldier1();
        squad.OnSoldier1Change += UpdateSoldier1;
        UpdateSoldier2();
        squad.OnSoldier2Change += UpdateSoldier2;
        UpdateSoldier3();
        squad.OnSoldier3Change += UpdateSoldier3;
        UpdateSoldier4();
        squad.OnSoldier4Change += UpdateSoldier4;

        // Update squad values and link event
        UpdateSquadValues();
        squad.OnValueChange += UpdateSquadValues;

        // Update range choice and link event
        UpdateRangeChoice();
        squad.OnPrefRangeChange += UpdateRangeChoice;

        // Set the Engage value and link event
        UpdateEngageValue();
        squad.OnEngageChange += UpdateEngageValue;

        // Reset UI navigation
        ResetUINav();
    }

    /// <summary>
    /// Lock method
    /// </summary>
    public void Lock()
    {
        unlockButton.gameObject.SetActive(true);
        PlayManager.OnHQPhase -= UpdateSoldier1;
        PlayManager.OnHQPhase -= UpdateSoldier2;
        PlayManager.OnHQPhase -= UpdateSoldier3;
        PlayManager.OnHQPhase -= UpdateSoldier4;
    }

    /// <summary>
    /// OnDestroy, unsubscribe all events
    /// </summary>
    private void OnDestroy()
    {
        if (squad != null)
        {
            squad.OnColorChange -= UpdateColor;
            squad.OnSoldier1Change -= UpdateSoldier1;
            squad.OnSoldier2Change -= UpdateSoldier2;
            squad.OnSoldier3Change -= UpdateSoldier3;
            squad.OnSoldier4Change -= UpdateSoldier4;
            squad.OnValueChange -= UpdateSquadValues;
            squad.OnPrefRangeChange -= UpdateRangeChoice;
            squad.OnEngageChange -= UpdateEngageValue;
        }

        select.OnSelection -= Select;
        PlayManager.OnHQPhase -= UpdateSoldier1;
        PlayManager.OnHQPhase -= UpdateSoldier2;
        PlayManager.OnHQPhase -= UpdateSoldier3;
        PlayManager.OnHQPhase -= UpdateSoldier4;
    }

    #region Update elements
    /// <summary>
    /// UpdateColor method updates the background color
    /// </summary>
    private void UpdateColor()
    {
        background.color = squad.Color;
    }

    /// <summary>
    /// UpdateSoldier1 method updates the soldier 1 image
    /// </summary>
    private void UpdateSoldier1()
    {
        if (squad == null)
        {
            soldier1Image.Setup(null);
            soldier1Healthbar.Hide();
        }
        else
        {
            soldier1Image.Setup(squad.Soldiers[0]);
            if (squad.Soldiers[0] == null)
            {
                soldier1Healthbar.Hide();
            }
            else
            {
                soldier1Healthbar.Show();
                soldier1Healthbar.UpdateValue(squad.Soldiers[0].CurrentHP, squad.Soldiers[0].MaxHP);
            }
        }        
    }

    /// <summary>
    /// UpdateSoldier2 method updates the soldier 2 image
    /// </summary>
    private void UpdateSoldier2()
    {
        if (squad == null)
        {
            soldier2Image.Setup(null);
            soldier2Healthbar.Hide();
        }
        else
        {
            soldier2Image.Setup(squad.Soldiers[1]);
            if (squad.Soldiers[1] == null)
            {
                soldier2Healthbar.Hide();
            }
            else
            {
                soldier2Healthbar.Show();
                soldier2Healthbar.UpdateValue(squad.Soldiers[1].CurrentHP, squad.Soldiers[1].MaxHP);
            }
        }
    }

    /// <summary>
    /// UpdateSoldier3 method updates the soldier 3 image
    /// </summary>
    private void UpdateSoldier3()
    {
        if (squad == null)
        {
            soldier3Image.Setup(null);
            soldier3Healthbar.Hide();
        }
        else
        {
            soldier3Image.Setup(squad.Soldiers[2]);
            if (squad.Soldiers[2] == null)
            {
                soldier3Healthbar.Hide();
            }
            else
            {
                soldier3Healthbar.Show();
                soldier3Healthbar.UpdateValue(squad.Soldiers[2].CurrentHP, squad.Soldiers[2].MaxHP);
            }
        }
    }

    /// <summary>
    /// UpdateSoldier4 method updates the soldier 4 image
    /// </summary>
    private void UpdateSoldier4()
    {
        if (squad == null)
        {
            soldier4Image.Setup(null);
            soldier4Healthbar.Hide();
        }
        else
        {
            soldier4Image.Setup(squad.Soldiers[3]);
            if (squad.Soldiers[3] == null)
            {
                soldier4Healthbar.Hide();
            }
            else
            {
                soldier4Healthbar.Show();
                soldier4Healthbar.UpdateValue(squad.Soldiers[3].CurrentHP, squad.Soldiers[3].MaxHP);
            }
        }
    }

    /// <summary>
    /// UpdateSquadValues method updates the squad values to display
    /// </summary>
    private void UpdateSquadValues()
    {
        atkShortValue.text = squad.AttackValues[0].ToString();
        atkMiddleValue.text = squad.AttackValues[1].ToString();
        atkLongValue.text = squad.AttackValues[2].ToString();

        defShortValue.text = squad.DefenseValues[0].ToString();
        defMiddleValue.text = squad.DefenseValues[1].ToString();
        defLongValue.text = squad.DefenseValues[2].ToString();
        defExplosives.text = squad.DefenseValues[3].ToString();

        speedValue.text = squad.Speed.ToString();
    }

    /// <summary>
    /// UpdateRangeChoice method updates the prefered range square position
    /// </summary>
    private void UpdateRangeChoice()
    {
        switch (squad.PrefRange)
        {
            case Squad.PreferedRange.ShortRange:
                rangeSelection.rectTransform.localPosition = atkShortValue.rectTransform.localPosition + new Vector3(-2, 0, 0);
                break;
            case Squad.PreferedRange.MiddleRange:
                rangeSelection.rectTransform.localPosition = atkMiddleValue.rectTransform.localPosition + new Vector3(-2, 0, 0);
                break;
            case Squad.PreferedRange.LongRange:
                rangeSelection.rectTransform.localPosition = atkLongValue.rectTransform.localPosition + new Vector3(-2, 0, 0);
                break;
        }
    }

    /// <summary>
    /// UpdateEngageValue method updates the Engage toggle value
    /// </summary>
    private void UpdateEngageValue()
    {
        // Set the Engage value and link event
        engage.isOn = squad.isEngaged;
    }
    #endregion

    #region Selection/UI Navigation
    /// <summary>
    /// Select method is linked to the background button of the Header
    /// </summary>
    public void Select()
    {
        OnSelection?.Invoke(this);
    }

    /// <summary>
    /// SetUINav method sets the UI navigation left element to the previous SquadHeader and the UI navigation right element of the previous SquadHeader to the current one
    /// </summary>
    /// <param name="_prevSquadHeader">Previous SquadHeader to select on left UI navigation (SquadHeader)</param>
    public void SetUINav(HQSquadHeader _prevSquadHeader)
    {
        Navigation _nav = select.navigation;
        _nav.selectOnLeft = _prevSquadHeader.select;
        select.navigation = _nav;

        Navigation _prevNav = _prevSquadHeader.select.navigation;
        _prevNav.selectOnRight = select;
        _prevSquadHeader.select.navigation = _prevNav;
    }

    /// <summary>
    /// ResetUINav method resets the UI navigation left/right elements
    /// </summary>
    public void ResetUINav()
    {
        Navigation _nav = select.navigation;
        _nav.selectOnLeft = null;
        _nav.selectOnRight = null;
    }

    #endregion
}
