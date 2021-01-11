using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQSquadHeader is related to a Squad element in the header bar of the CommandCenter Canvas
/// </summary>
public class HQSquadHeader : MonoBehaviour
{
    // Public elements of the Squad Header
    public Image background;
    public Toggle engage;
    public SoldierImage soldier1Image;
    public SoldierImage soldier2Image;
    public SoldierImage soldier3Image;
    public SoldierImage soldier4Image;
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
    public int unlockCost;
    public Text unlockText;
    public Text unlockCostLabel;

    // Events
    public delegate void SquadHeaderEventHandler(HQSquadHeader _squadHeader);
    public event SquadHeaderEventHandler OnSelection;
    public event SquadHeaderEventHandler OnUnlock;

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
        unlockText.text = "Locked";
        unlockCostLabel.text = "Cost: " + unlockCost.ToString();
        unlockCostLabel.enabled = false;
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
        engage.isOn = squad.isEngaged;
        engage.onValueChanged.AddListener(delegate { squad.Engage(engage.isOn); });
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
            engage.onValueChanged.RemoveAllListeners();
        }
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
        soldier1Image.Setup(squad.Soldiers[0]);
    }

    /// <summary>
    /// UpdateSoldier2 method updates the soldier 2 image
    /// </summary>
    private void UpdateSoldier2()
    {
        soldier2Image.Setup(squad.Soldiers[1]);
    }

    /// <summary>
    /// UpdateSoldier3 method updates the soldier 3 image
    /// </summary>
    private void UpdateSoldier3()
    {
        soldier3Image.Setup(squad.Soldiers[2]);
    }

    /// <summary>
    /// UpdateSoldier4 method updates the soldier 4 image
    /// </summary>
    private void UpdateSoldier4()
    {
        soldier4Image.Setup(squad.Soldiers[3]);
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
                rangeSelection.rectTransform.localPosition = new Vector3(183, -3, 0);
                break;
            case Squad.PreferedRange.MiddleRange:
                rangeSelection.rectTransform.localPosition = new Vector3(228, -3, 0);
                break;
            case Squad.PreferedRange.LongRange:
                rangeSelection.rectTransform.localPosition = new Vector3(273, -3, 0);
                break;
        }
    }
    #endregion

    #region Selection/Unlock
    /// <summary>
    /// Select method is linked to the background button of the Header
    /// </summary>
    public void Select()
    {
        OnSelection?.Invoke(this);
    }

    /// <summary>
    /// ReadyToUnlock method is used to changed the Unlock button from Locked to Unlock state
    /// </summary>
    public void ReadyToUnlock()
    {
        unlockButton.interactable = true;
        unlockText.text = "Unlock";
        unlockCostLabel.enabled = true;
    }

    /// <summary>
    /// Unlock method creates a new squad and links it to the header
    /// </summary>
    public void Unlock()
    {
        if(PlayManager.coins >= unlockCost)
        {
            unlockButton.gameObject.SetActive(false);
            PlayManager.AddCoins(-unlockCost);
            Squad _squad = ScriptableObject.CreateInstance("Squad") as Squad;
            _squad.InitData();
            PlayManager.squadList.Add(_squad);
            OnUnlock?.Invoke(this);
        }
    }
    #endregion
}
