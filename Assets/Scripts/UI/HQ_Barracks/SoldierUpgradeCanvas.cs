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

    public GameObject friendshipPanel;
    public GameObject friendshipTitlePrefab;
    public GameObject friendshipItemPrefab;

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
        changeNameCanvas.OnHide -= ChangeNameButtonSelection;
        changeImageCanvas.OnHide -= ChangeImageButtonSelection;
        levelupCanvas.OnHide -= LevelUpButtonSelection;

        if (selectedSoldier != null)
        {
            // Unsubscribe from events
            selectedSoldier.OnNameChange -= UpdateName;
            selectedSoldier.OnImageChange -= UpdateImage;
            selectedSoldier.OnDataChange -= UpdateData;
        }

        changeNameCanvas.Hide();
        changeNameCanvas.OnHide += ChangeNameButtonSelection;

        changeImageCanvas.Hide();
        changeImageCanvas.OnHide += ChangeImageButtonSelection;

        levelupCanvas.Hide();
        levelupCanvas.OnHide += LevelUpButtonSelection;

        selectedSoldier = _soldier;

        soldierImage.Setup(selectedSoldier, true);
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
        atkShortValue.text = (selectedSoldier.ShortRangeAttack).ToString();
        atkMiddleValue.text = (selectedSoldier.MiddleRangeAttack).ToString();
        atkLongValue.text = (selectedSoldier.LongRangeAttack).ToString();

        defShortValue.text = (selectedSoldier.ShortRangeDefense).ToString();
        defMiddleValue.text = (selectedSoldier.MiddleRangeDefense).ToString();
        defLongValue.text = (selectedSoldier.LongRangeDefense).ToString();
        defExploValue.text = (selectedSoldier.ExplosivesDefense).ToString();

        GetCapacities(selectedSoldier);
        GetFriendship(selectedSoldier);

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
    /// GetFriendship method sets the list of accointance of the soldier from its friendship list
    /// </summary>
    /// <param name="_soldier">Soldier from which get the freindship points (Soldier)</param>
    private void GetFriendship(Soldier _soldier)
    {
        // Clear the previous friendship data
        foreach(Transform _t in friendshipPanel.transform)
        {
            Destroy(_t.gameObject);
        }

        List<Soldier> _friendsLvl4 = new List<Soldier>();
        List<Soldier> _friendsLvl3 = new List<Soldier>();
        List<Soldier> _friendsLvl2 = new List<Soldier>();
        List<Soldier> _friendsLvl1 = new List<Soldier>();
        List<Soldier> _friendsLvl0 = new List<Soldier>();

        foreach(int _ID in _soldier.Friendship.Keys)
        {
            if(_soldier.Friendship[_ID] >= PlayManager.data.friendshipLevels[4].threshold)
            {
                _friendsLvl4.Add(PlayManager.soldierList[_ID]);
            }
            else if (_soldier.Friendship[_ID] >= PlayManager.data.friendshipLevels[3].threshold)
            {
                _friendsLvl3.Add(PlayManager.soldierList[_ID]);
            }
            else if (_soldier.Friendship[_ID] >= PlayManager.data.friendshipLevels[2].threshold)
            {
                _friendsLvl2.Add(PlayManager.soldierList[_ID]);
            }
            else if (_soldier.Friendship[_ID] >= PlayManager.data.friendshipLevels[1].threshold)
            {
                _friendsLvl1.Add(PlayManager.soldierList[_ID]);
            }
            else if (_soldier.Friendship[_ID] >= PlayManager.data.friendshipLevels[0].threshold)
            {
                _friendsLvl0.Add(PlayManager.soldierList[_ID]);
            }
        }

        // List of friends lvl 4
        Text _title = Instantiate(friendshipTitlePrefab, friendshipPanel.transform).GetComponent<Text>();
        _title.text = PlayManager.data.friendshipLevels[4].levelName + ":";
        if (_friendsLvl4.Count >0)
        {
            foreach(Soldier _s in _friendsLvl4)
            {
                Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
                _friend.text = PlayManager.data.ranks[_s.Data.soldierLevel] + " " + _s.Name;
            }
        }
        else
        {
            Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
            _friend.text = "None";
        }

        // List of friends lvl3
        _title = Instantiate(friendshipTitlePrefab, friendshipPanel.transform).GetComponent<Text>();
        _title.text = PlayManager.data.friendshipLevels[3].levelName + ":";
        if (_friendsLvl3.Count > 0)
        {
            foreach (Soldier _s in _friendsLvl3)
            {
                Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
                _friend.text = PlayManager.data.ranks[_s.Data.soldierLevel] + " " + _s.Name;
            }
        }
        else
        {
            Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
            _friend.text = "None";
        }

        // List of friends lvl2
        _title = Instantiate(friendshipTitlePrefab, friendshipPanel.transform).GetComponent<Text>();
        _title.text = PlayManager.data.friendshipLevels[2].levelName + ":";
        if (_friendsLvl2.Count > 0)
        {
            foreach (Soldier _s in _friendsLvl2)
            {
                Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
                _friend.text = PlayManager.data.ranks[_s.Data.soldierLevel] + " " + _s.Name;
            }
        }
        else
        {
            Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
            _friend.text = "None";
        }

        // List of friends lvl1
        _title = Instantiate(friendshipTitlePrefab, friendshipPanel.transform).GetComponent<Text>();
        _title.text = PlayManager.data.friendshipLevels[1].levelName + ":";
        if (_friendsLvl1.Count > 0)
        {
            foreach (Soldier _s in _friendsLvl1)
            {
                Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
                _friend.text = PlayManager.data.ranks[_s.Data.soldierLevel] + " " + _s.Name;
            }
        }
        else
        {
            Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
            _friend.text = "None";
        }

        // List of friends lvl0
        _title = Instantiate(friendshipTitlePrefab, friendshipPanel.transform).GetComponent<Text>();
        _title.text = PlayManager.data.friendshipLevels[0].levelName + ":";
        if (_friendsLvl0.Count > 0)
        {
            foreach (Soldier _s in _friendsLvl0)
            {
                Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
                _friend.text = PlayManager.data.ranks[_s.Data.soldierLevel] + " " + _s.Name;
            }
        }
        else
        {
            Text _friend = Instantiate(friendshipItemPrefab, friendshipPanel.transform).GetComponent<Text>();
            _friend.text = "None";
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
        changeNameCanvas.OnHide -= ChangeNameButtonSelection;
        changeImageCanvas.OnHide -= ChangeImageButtonSelection;
        levelupCanvas.OnHide -= LevelUpButtonSelection;
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
        soldierImage.Setup(selectedSoldier, true);
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
        soldierImage.Setup(selectedSoldier, true);

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
        atkShortValue.text = (selectedSoldier.ShortRangeAttack).ToString();
        atkMiddleValue.text = (selectedSoldier.MiddleRangeAttack).ToString();
        atkLongValue.text = (selectedSoldier.LongRangeAttack).ToString();

        defShortValue.text = (selectedSoldier.ShortRangeDefense).ToString();
        defMiddleValue.text = (selectedSoldier.MiddleRangeDefense).ToString();
        defLongValue.text = (selectedSoldier.LongRangeDefense).ToString();
        defExploValue.text = (selectedSoldier.ExplosivesDefense).ToString();

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
