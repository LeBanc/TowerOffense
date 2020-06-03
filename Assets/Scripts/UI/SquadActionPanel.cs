using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SquadActionPanel is used during city attack to update the UI Squad Panel and gives order to the SquadUnit
/// </summary>
public class SquadActionPanel : MonoBehaviour
{
    // Public data sets by users in Squad panel prefab
    // Background image (for color)
    public Image background;

    // Squad overlay to select the squad by clicking on it
    public Image squadSelectionOverlay;
    
    // Soldier images
    public SoldierImage soldier1Image;
    public SoldierImage soldier2Image;
    public SoldierImage soldier3Image;
    public SoldierImage soldier4Image;

    // Soldier HealthBars
    public HealthBar soldier1HealthBar;
    public HealthBar soldier2HealthBar;
    public HealthBar soldier3HealthBar;
    public HealthBar soldier4HealthBar;

    // Action buttons
    public Button moveAction;
    public Button retreatAction;
    public Button specialAction1;
    public Button specialAction2;
    public Button specialAction3;
    public Button specialAction4;

    // Action sprites (data)
    public Sprite moveActionSprite;
    public Sprite retreatActionSprite;
    public Sprite healActionSprite;
    public Sprite buildHQActionSprite;
    public Sprite buildTurretActionSprite;
    public Sprite explosivesActionSprite;

    // Private data used in the script
    // SquadUnit linked to this panel
    private SquadUnit squadUnit;

    // Boolean defining if the panel has been intialized or not
    internal bool isSet = false;

    // Capacities amount
    private int healCapacity;
    private int buildHQCapacity;
    private int buildTurretCapacity;
    private int explosivesCapacity;

    // Private buttons to link capacities action to physical buttons
    private Button healButton;
    private Button buildHQButton;
    private Button buildTurretButton;
    private Button explosivesButton;

    public Button selectedButton;

    /// <summary>
    /// At Start, sets all buttons not interactable
    /// </summary>
    private void Start()
    {
        moveAction.interactable = false;
        retreatAction.interactable = false;
        specialAction1.interactable = false;
        specialAction2.interactable = false;
        specialAction3.interactable = false;
        specialAction4.interactable = false;
    }

    /// <summary>
    /// Setup method initializes the panel from Squad data and SquadUnit
    /// </summary>
    /// <param name="_squadUnit">SquadUnit to link to the panel</param>
    public void Setup(SquadUnit _squadUnit)
    {
        // Activate the object and change the background color
        gameObject.SetActive(true);
        background.color = _squadUnit.Squad.Color;

        // Update SoldierImages (background and border) from SquadUnit's Squad data
        soldier1Image.Setup(_squadUnit.Squad.Soldiers[0]);
        soldier2Image.Setup(_squadUnit.Squad.Soldiers[1]);
        soldier3Image.Setup(_squadUnit.Squad.Soldiers[2]);
        soldier4Image.Setup(_squadUnit.Squad.Soldiers[3]);
        
        // Link squadUnit and subscribe to events
        squadUnit = _squadUnit;
        _squadUnit.Soldier1.OnDamage += soldier1HealthBar.UpdateValue;
        _squadUnit.Soldier2.OnDamage += soldier2HealthBar.UpdateValue;
        _squadUnit.Soldier3.OnDamage += soldier3HealthBar.UpdateValue;
        _squadUnit.Soldier4.OnDamage += soldier4HealthBar.UpdateValue;

        // Initialize Capacities amount
        healCapacity = 0;
        buildHQCapacity = 0;
        buildTurretCapacity = 0;
        explosivesCapacity = 0;
        foreach(Soldier _s in _squadUnit.Squad.Soldiers)
        {
            foreach (SoldierData.Capacities _c in _s.Data.capacities)
            {
                switch (_c)
                {
                    case SoldierData.Capacities.Heal:
                        healCapacity++;
                        break;
                    case SoldierData.Capacities.HQBuild:
                        buildHQCapacity++;
                        break;
                    case SoldierData.Capacities.TurretBuild:
                        buildTurretCapacity++;
                        break;
                    case SoldierData.Capacities.Explosives:
                        explosivesCapacity++;
                        break;
                }
            }
        }

        // Setup the special action buttons from the capacities amount
        specialAction1.gameObject.SetActive(false);
        specialAction2.gameObject.SetActive(false);
        specialAction3.gameObject.SetActive(false);
        specialAction4.gameObject.SetActive(false);
        if (healCapacity > 0)
        {
            healButton = GetNextButton();
            healButton.onClick.AddListener(Heal);
            healButton.GetComponent<Image>().sprite = healActionSprite;
            healButton.GetComponentInChildren<Text>().text = healCapacity.ToString();
        }
        if (buildHQCapacity > 0)
        {
            buildHQButton = GetNextButton();
            buildHQButton.onClick.AddListener(BuildHQ);
            buildHQButton.GetComponent<Image>().sprite = buildHQActionSprite;
            buildHQButton.GetComponentInChildren<Text>().text = buildHQCapacity.ToString();
        }
        if (buildTurretCapacity > 0)
        {
            buildTurretButton = GetNextButton();
            buildTurretButton.onClick.AddListener(BuildTurret);
            buildTurretButton.GetComponent<Image>().sprite = buildTurretActionSprite;
            buildTurretButton.GetComponentInChildren<Text>().text = buildTurretCapacity.ToString();
        }
        if (explosivesCapacity > 0)
        {
            explosivesButton = GetNextButton();
            explosivesButton.onClick.AddListener(Explosives);
            explosivesButton.GetComponent<Image>().sprite = explosivesActionSprite;
            explosivesButton.GetComponentInChildren<Text>().text = explosivesCapacity.ToString();
        }

        isSet = true;
        PlayManager.RetreatAll += Retreat;
    }

    /// <summary>
    /// GetNextButton method returns the next not assigned capacity button
    /// </summary>
    /// <returns>Capacity Button to assign</returns>
    private Button GetNextButton()
    {
        if (!specialAction1.gameObject.activeSelf)
        {
            specialAction1.gameObject.SetActive(true);
            return specialAction1;
        }
        else if (!specialAction2.gameObject.activeSelf)
        {
            specialAction2.gameObject.SetActive(true);
            return specialAction2;
        }
        else if (!specialAction3.gameObject.activeSelf)
        {
            specialAction3.gameObject.SetActive(true);
            return specialAction3;
        }
        else if (!specialAction4.gameObject.activeSelf)
        {
            specialAction4.gameObject.SetActive(true);
            return specialAction4;
        }
        else
        {
            Debug.LogError("[SquadActionPanel] Trying to activate a 5th capacity!");
            return null;
        }
    }

    /// <summary>
    /// Reset method hides the panel and sets it as "uninitialized"
    /// </summary>
    public void Reset()
    {
        isSet = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// OnDestroy unsubscribes from events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.RetreatAll -= Retreat;
    }

    /// <summary>
    /// Retrat method triggers the Retreat action of the SquadUnit and hides this panel
    /// </summary>
    public void Retreat()
    {
        squadUnit.Retreat();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Heal method triggers the Heal action of the SquadUnit if Heal amount is positive
    /// </summary>
    private void Heal()
    {
        if(healCapacity > 0)
        {
            healCapacity--;
            healButton.GetComponentInChildren<Text>().text = healCapacity.ToString();
            squadUnit.Heal();
            if (healCapacity == 0)
            {
                healButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// Heal method triggers the BuildHQ action of the SquadUnit if BuildHQ amount is positive => TBD
    /// </summary>
    private void BuildHQ()
    {
        if (buildHQCapacity > 0)
        {
            // Build HQ Action TBD
            buildHQCapacity--;
            buildHQButton.GetComponentInChildren<Text>().text = buildHQCapacity.ToString();

            if(buildHQCapacity == 0)
            {
                buildHQButton.interactable = false;
            }

        }
    }

    /// <summary>
    /// BuildTurret method triggers the BuildTurret action of the SquadUnit if BuildTurret amount is positive => TBD
    /// </summary>
    private void BuildTurret()
    {
        if (buildTurretCapacity > 0)
        {
            // Build Turret Action TBD
            buildTurretCapacity--;
            buildTurretButton.GetComponentInChildren<Text>().text = buildTurretCapacity.ToString();
            if (buildTurretCapacity == 0)
            {
                buildTurretButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// Explosives method triggers the Explosives action of the SquadUnit if Explosives amount is positive => TBD
    /// </summary>
    private void Explosives()
    {
        if (explosivesCapacity > 0)
        {
            // Build Explosives Action TBD
            explosivesCapacity--;
            explosivesButton.GetComponentInChildren<Text>().text = explosivesCapacity.ToString();
            if (explosivesCapacity == 0)
            {
                explosivesButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// SelectSquad method selects and unselects the SquadActionPanel and the SqaudUnit linked to it
    /// </summary>
    /// <param name="_b">Selected or not</param>
    public void SelectSquad(bool _b)
    {
        if (_b) // Selection
        {
            // Select the first Action button and the associated SqaudUnit method
            moveAction.Select();
            squadUnit.OnMoveActionSelected();
            selectedButton = moveAction;
            Invoke("ActivateInteractions", 0.05f);
        }
        else // Unselection
        {
            // Make all buttons uninteractable and unsubscribe from Update event
            moveAction.interactable = false;
            retreatAction.interactable = false;
            if (specialAction1.gameObject.activeSelf) specialAction1.interactable = false;
            if (specialAction2.gameObject.activeSelf) specialAction2.interactable = false;
            if (specialAction3.gameObject.activeSelf) specialAction3.interactable = false;
            if (specialAction4.gameObject.activeSelf) specialAction4.interactable = false;
            GameManager.PlayUpdate -= SquadActionPanelUpdate;
        }
        // Enable (or not) the Overlay image and the raycast on the background (to enable the selection)
        squadSelectionOverlay.enabled = !_b;
        background.raycastTarget = !_b;
    }

    /// <summary>
    /// ActivateInteractions method sets all buttons to interactable and subscribe to Update event
    /// </summary>
    private void ActivateInteractions()
    {
        moveAction.interactable = true;
        retreatAction.interactable = true;
        if (specialAction1.gameObject.activeSelf) specialAction1.interactable = true;
        if (specialAction2.gameObject.activeSelf) specialAction2.interactable = true;
        if (specialAction3.gameObject.activeSelf) specialAction3.interactable = true;
        if (specialAction4.gameObject.activeSelf) specialAction4.interactable = true;
        GameManager.PlayUpdate += SquadActionPanelUpdate;
    }

    /// <summary>
    /// IsActive method returns if the panel is active or not
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return !squadSelectionOverlay.enabled;
    }

    /// <summary>
    /// SquadActionPanelUpdate is the Update method of the SquadActionPanel
    /// Left and Right arrow to move between active buttons
    /// </summary>
    private void SquadActionPanelUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedButton == moveAction)
            {
                retreatAction.Select();
                selectedButton = retreatAction;
                squadUnit.OnMoveActionUnselected();
            }
            else if (selectedButton == retreatAction)
            {
                if (specialAction1.gameObject.activeSelf)
                {
                    specialAction1.Select();
                    selectedButton = specialAction1;
                }
                else
                {
                    moveAction.Select();
                    selectedButton = moveAction;
                    squadUnit.OnMoveActionSelected();
                }
            }
            else if(selectedButton == specialAction1)
            {
                if (specialAction2.gameObject.activeSelf)
                {
                    specialAction2.Select();
                    selectedButton = specialAction2;
                }
                else
                {
                    moveAction.Select();
                    selectedButton = moveAction;
                    squadUnit.OnMoveActionSelected();
                }
            }
            else if(selectedButton == specialAction2)
            {
                if (specialAction3.gameObject.activeSelf)
                {
                    specialAction3.Select();
                    selectedButton = specialAction3;
                }
                else
                {
                    moveAction.Select();
                    selectedButton = moveAction;
                    squadUnit.OnMoveActionSelected();
                }
            }
            else if(selectedButton == specialAction3)
            {
                if (specialAction4.gameObject.activeSelf)
                {
                    specialAction4.Select();
                    selectedButton = specialAction4;
                }
                else
                {
                    moveAction.Select();
                    selectedButton = moveAction;
                    squadUnit.OnMoveActionSelected();
                }
            }
            else if(selectedButton == specialAction4)
            {
                moveAction.Select();
                selectedButton = moveAction;
                squadUnit.OnMoveActionSelected();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedButton == moveAction)
            {
                if (specialAction4.gameObject.activeSelf)
                {
                    specialAction4.Select();
                    selectedButton = specialAction4;
                }
                else if (specialAction3.gameObject.activeSelf)
                {
                    specialAction3.Select();
                    selectedButton = specialAction3;
                }
                else if (specialAction2.gameObject.activeSelf)
                {
                    specialAction2.Select();
                    selectedButton = specialAction2;
                }
                else if (specialAction1.gameObject.activeSelf)
                {
                    specialAction1.Select();
                    selectedButton = specialAction1;
                }
                else
                {
                    retreatAction.Select();
                    selectedButton = retreatAction;
                }
                squadUnit.OnMoveActionUnselected();
            }
            else if (selectedButton == retreatAction)
            {
                moveAction.Select();
                selectedButton = moveAction;
                squadUnit.OnMoveActionSelected();
            }
            else if (selectedButton == specialAction1)
            {
                retreatAction.Select();
                selectedButton = retreatAction;
            }
            else if (selectedButton == specialAction2)
            {
                specialAction1.Select();
                selectedButton = specialAction1;
            }
            else if (selectedButton == specialAction3)
            {
                specialAction2.Select();
                selectedButton = specialAction2;
            }
            else if (selectedButton == specialAction4)
            {
                specialAction4.Select();
                selectedButton = specialAction4;
            }
        }
    }
}
