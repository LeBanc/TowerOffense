using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SoldierUpgradeCanvas is the class linked to the Soldier Upgrade Canvas
/// </summary>
public class SoldierUpgradeCanvas : UICanvas
{
    // public graphical elements
    public SoldierImage soldierImage;
    public HealthBar healthBar;
    public HealthBar experienceBar;

    public Text soldierName;
    public Text soldierLevel;
    public Text soldierType;
    public Text speed;

    public Text atkShortValue;
    public Text atkMiddleValue;
    public Text atkLongValue;

    public Text defShortValue;
    public Text defMiddleValue;
    public Text defLongValue;
    public Text defExploValue;

    public Text capacity1;
    public Text capacity2;
    public Text capacity3;
    public Text capacity4;

    public Button changeImageButton;
    public Button changeNameButton;
    public Button levelupButton;

    public HQChangeNameCanvas changeNameCanvas;
    public HQChangeImageCanvas changeImageCanvas;
    public HQLevelupCanvas levelupCanvas;

    // private Soldier for storing current selection
    private Soldier selectedSoldier;

    /// <summary>
    /// Setup method is used to initialize all data with a soldier
    /// </summary>
    /// <param name="_soldier">Soldier to get the data from</param>
    public void Setup(Soldier _soldier)
    {
        // Remove events'subscription to avoid button selection
        changeNameCanvas.OnCanvasHide -= ChangeNameButtonSelection;
        changeImageCanvas.OnCanvasHide -= ChangeImageButtonSelection;
        levelupCanvas.OnCanvasHide -= LevelUpButtonSelection;

        if (selectedSoldier != null)
        {
            // Unsubscribe from events
            selectedSoldier.OnNameChange -= UpdateName;
            selectedSoldier.OnImageChange -= UpdateImage;
            selectedSoldier.OnDataChange -= UpdateData;
        }

        changeNameCanvas.Hide();
        changeNameCanvas.OnCanvasHide += ChangeNameButtonSelection;

        changeImageCanvas.Hide();
        changeImageCanvas.OnCanvasHide += ChangeImageButtonSelection;

        levelupCanvas.Hide();
        levelupCanvas.OnCanvasHide += LevelUpButtonSelection;

        selectedSoldier = _soldier;

        soldierImage.Setup(selectedSoldier);
        healthBar.UpdateValue(selectedSoldier.CurrentHP, selectedSoldier.MaxHP);
        if (selectedSoldier.MaxXP > 0)
        {
            experienceBar.UpdateValue(Mathf.Min(selectedSoldier.CurrentXP, selectedSoldier.MaxXP), selectedSoldier.MaxXP);
        }
        else
        {
            experienceBar.Hide();
        }

        levelupButton.interactable = (selectedSoldier.CurrentXP >= selectedSoldier.MaxXP) && (selectedSoldier.MaxXP > 0);
        Navigation _changeNameNav = changeNameButton.navigation;
        _changeNameNav.selectOnDown = levelupButton.interactable ? levelupButton : null;
        changeNameButton.navigation = _changeNameNav;
        
        soldierName.text = selectedSoldier.Name;
        soldierLevel.text = PlayManager.data.ranks[selectedSoldier.Data.soldierLevel];
        soldierType.text = selectedSoldier.Data.typeName;

        speed.text = selectedSoldier.Speed.ToString();
        atkShortValue.text = (selectedSoldier.ShortRangeAttack - selectedSoldier.BonusAtkShortRange).ToString();
        atkMiddleValue.text = (selectedSoldier.MiddleRangeAttack - selectedSoldier.BonusAtkMidRange).ToString();
        atkLongValue.text = (selectedSoldier.LongRangeAttack - selectedSoldier.BonusAtkLongRange).ToString();

        defShortValue.text = (selectedSoldier.ShortRangeDefense - selectedSoldier.BonusDefShortRange).ToString();
        defMiddleValue.text = (selectedSoldier.MiddleRangeDefense - selectedSoldier.BonusDefMidRange).ToString();
        defLongValue.text = (selectedSoldier.LongRangeDefense - selectedSoldier.BonusDefLongRange).ToString();
        defExploValue.text = (selectedSoldier.ExplosivesDefense - selectedSoldier.BonusDefExplosives).ToString();

        GetCapacities(selectedSoldier);

        // Subscribe to events
        selectedSoldier.OnNameChange += UpdateName;
        selectedSoldier.OnImageChange += UpdateImage;
        selectedSoldier.OnDataChange += UpdateData;        
    }

    /// <summary>
    /// GetCapacities method extractsthe capacity (text form) from a soldier data
    /// </summary>
    /// <param name="_soldier">Soldier from which extract the capacity</param>
    private void GetCapacities(Soldier _soldier)
    {
        // Initialize capacities' amount
        int _increaseSpeed = 0;
        int _heal = 0;
        int _hqBuild = 0;
        int _turretBuild = 0;
        int _explosives = 0;
        int _woundedSaving = 0;

        // Get capacities and increment their amount
        foreach (SoldierData.Capacities _soldierCapa in _soldier.Data.capacities)
        {
            switch (_soldierCapa)
            {
                case SoldierData.Capacities.IncreaseSpeed:
                    _increaseSpeed++;
                    break;
                case SoldierData.Capacities.Heal:
                    _heal++;
                    break;
                case SoldierData.Capacities.HQBuild:
                    _hqBuild++;
                    break;
                case SoldierData.Capacities.TurretBuild:
                    _turretBuild++;
                    break;
                case SoldierData.Capacities.Explosives:
                    _explosives++;
                    break;
                case SoldierData.Capacities.WoundedSaving:
                    _woundedSaving++;
                    break;
            }
        }

        // Clear text label
        capacity1.text = "";
        capacity2.text = "";
        capacity3.text = "";
        capacity4.text = "";

        // For each capacity, add its index to a list if it is not null
        List<int> _indexes = new List<int>();
        int[] _capa = { _increaseSpeed, _heal, _hqBuild, _turretBuild, _explosives, _woundedSaving };
        for (int i = 0; i < _capa.Length; i++)
        {
            if (_capa[i] > 0) _indexes.Add(i);
        }
        // If their is no capacity enables, return here
        if (_indexes.Count == 0) return;

        Text[] _capacitiesLabel = { capacity1, capacity2, capacity3, capacity4 };
        // For each not null capacity (in _indexes list)
        for (int i = 0; i < Mathf.Min(_indexes.Count, _capacitiesLabel.Length); i++) // There are 4 capacity labels so the loop should not go ever 4
        {
            // Get the index of the capacity and build the right text (with amount if needed)
            switch (_indexes[i])
            {
                case 0:
                    _capacitiesLabel[i].text = "Increase Speed" + ((_increaseSpeed > 1) ? string.Format(" x{0}", _increaseSpeed) : "");
                    break;
                case 1:
                    _capacitiesLabel[i].text = "Heal" + ((_heal > 1) ? string.Format(" x{0}", _heal) : "");
                    break;
                case 2:
                    _capacitiesLabel[i].text = "Build new HQ" + ((_hqBuild > 1) ? string.Format(" x{0}", _hqBuild) : "");
                    break;
                case 3:
                    _capacitiesLabel[i].text = "Build Turret" + ((_turretBuild > 1) ? string.Format(" x{0}", _turretBuild) : "");
                    break;
                case 4:
                    _capacitiesLabel[i].text = "Explosives" + ((_explosives > 1) ? string.Format(" x{0}", _explosives) : "");
                    break;
                case 5:
                    _capacitiesLabel[i].text = "Save Wounded Soldiers" + ((_woundedSaving > 1) ? string.Format(" x{0}", _woundedSaving) : "");
                    break;
            }
        }
    }

    /// <summary>
    /// OnDestroy, unsubscribes from events
    /// </summary>
    private void OnDestroy()
    {
        if(selectedSoldier != null)
        {
            // Unsubscribe from events
            selectedSoldier.OnNameChange -= UpdateName;
            selectedSoldier.OnImageChange -= UpdateImage;
            selectedSoldier.OnDataChange -= UpdateData;
        }
        changeNameCanvas.OnCanvasHide -= ChangeNameButtonSelection;
        changeImageCanvas.OnCanvasHide -= ChangeImageButtonSelection;
        levelupCanvas.OnCanvasHide -= LevelUpButtonSelection;
    }

    /// <summary>
    /// Show method hides the canvas and subcanvas
    /// </summary>
    public override void Hide()
    {
        changeNameCanvas.Hide();
        changeImageCanvas.Hide();
        levelupCanvas.Hide();

        base.Hide();
    }

    /// <summary>
    /// ChangeName method displays the change name canvas (called by button)
    /// </summary>
    public void ChangeName()
    {
        changeNameCanvas.Show(selectedSoldier);
    }

    /// <summary>
    /// UpdateName method updates the soldier name
    /// </summary>
    public void UpdateName()
    {
        soldierName.text = selectedSoldier.Name;
    }

    /// <summary>
    /// ChangeImage method displays the change image canvas (called by button)
    /// </summary>
    public void ChangeImage()
    {
        changeImageCanvas.Show(selectedSoldier);
    }

    /// <summary>
    /// UpdateImage method updates the soldier image
    /// </summary>
    public void UpdateImage()
    {
        soldierImage.Setup(selectedSoldier);
    }

    /// <summary>
    /// ChangeLevel method displays the update level canvas (called by button)
    /// </summary>
    public void ChangeLevel()
    {
        levelupCanvas.Show(selectedSoldier);
    }

    /// <summary>
    /// UpdateData method updates the soldier data after level up
    /// </summary>
    public void UpdateData()
    {
        // Update image for border color
        soldierImage.Setup(selectedSoldier);

        // Update HP and XP bars
        healthBar.UpdateValue(selectedSoldier.CurrentHP, selectedSoldier.MaxHP);
        if(selectedSoldier.MaxXP > 0)
        {
            experienceBar.UpdateValue(Mathf.Min(selectedSoldier.CurrentXP, selectedSoldier.MaxXP), selectedSoldier.MaxXP);
        }
        else
        {
            experienceBar.Hide();
        }
        
        levelupButton.interactable = (selectedSoldier.CurrentXP >= selectedSoldier.MaxXP) && (selectedSoldier.MaxXP > 0);
        Navigation _changeNameNav = changeNameButton.navigation;
        _changeNameNav.selectOnDown = levelupButton.interactable ? levelupButton : null;
        changeNameButton.navigation = _changeNameNav;

        soldierLevel.text = PlayManager.data.ranks[selectedSoldier.Data.soldierLevel];
        soldierType.text = selectedSoldier.Data.typeName;

        speed.text = selectedSoldier.Speed.ToString();
        atkShortValue.text = (selectedSoldier.ShortRangeAttack - selectedSoldier.BonusAtkShortRange).ToString();
        atkMiddleValue.text = (selectedSoldier.MiddleRangeAttack - selectedSoldier.BonusAtkMidRange).ToString();
        atkLongValue.text = (selectedSoldier.LongRangeAttack - selectedSoldier.BonusAtkLongRange).ToString();

        defShortValue.text = (selectedSoldier.ShortRangeDefense - selectedSoldier.BonusDefShortRange).ToString();
        defMiddleValue.text = (selectedSoldier.MiddleRangeDefense - selectedSoldier.BonusDefMidRange).ToString();
        defLongValue.text = (selectedSoldier.LongRangeDefense - selectedSoldier.BonusDefLongRange).ToString();
        defExploValue.text = (selectedSoldier.ExplosivesDefense - selectedSoldier.BonusDefExplosives).ToString();

        GetCapacities(selectedSoldier);
    }

    /// <summary>
    /// ChangeNameButtonSelection method selects the "ChangeName" button
    /// </summary>
    private void ChangeNameButtonSelection()
    {
        changeNameButton.Select();
    }

    /// <summary>
    /// ChangeImageButtonSelection method selects the "ChangeImage" button
    /// </summary>
    private void ChangeImageButtonSelection()
    {
        changeImageButton.Select();
    }

    /// <summary>
    /// LevelUpButtonSelection method selects the "LevelUp" button
    /// </summary>
    private void LevelUpButtonSelection()
    {
        // If LevelUp button is interactable, selects it
        if(levelupButton.interactable)
        {
            levelupButton.Select();
        }
        else // else, selects the ChangeName button
        {
            changeNameButton.Select();
        }
    }
}
