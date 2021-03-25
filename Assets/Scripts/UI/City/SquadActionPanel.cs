using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public Toggle moveToggle;
    public ToggleClickable retreatToggle;
    public ToggleClickable healToggle;
    public Toggle buildHQToggle;
    public Toggle buildTurretToggle;
    public Toggle explosivesToggle;

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

    // Events
    public delegate void SquadActionPanelEventHandler();
    public event SquadActionPanelEventHandler OnSelection;
    public event SquadActionPanelEventHandler OnSelectionRequest;
    public event SquadActionPanelEventHandler OnRetreat;

    // Properties
    public SquadUnit SquadUnit
    {
        get { return squadUnit; }
    }

    /// <summary>
    /// At Start, sets all buttons not interactable
    /// </summary>
    private void Start()
    {
        moveToggle.interactable = false;
        retreatToggle.interactable = false;
        healToggle.interactable = false;
        buildHQToggle.interactable = false;
        buildTurretToggle.interactable = false;
        explosivesToggle.interactable = false;
    }

    /// <summary>
    /// Setup method initializes the panel from Squad data and SquadUnit
    /// </summary>
    /// <param name="_squadUnit">SquadUnit to link to the panel</param>
    public void Setup(SquadUnit _squadUnit)
    {
        // Link squadUnit and subscribe to events
        squadUnit = _squadUnit;
        squadUnit.Soldier1.OnDamage += soldier1HealthBar.UpdateValue;
        squadUnit.Soldier2.OnDamage += soldier2HealthBar.UpdateValue;
        squadUnit.Soldier3.OnDamage += soldier3HealthBar.UpdateValue;
        squadUnit.Soldier4.OnDamage += soldier4HealthBar.UpdateValue;

        moveToggle.onValueChanged.AddListener(squadUnit.OnActionSelected);
        moveToggle.onValueChanged.AddListener(squadUnit.OnMoveActionSelected);
        moveToggle.onValueChanged.AddListener(CursorManager.ChangeToMoveAttack);

        retreatToggle.onValueChanged.AddListener(squadUnit.OnActionSelected);
        retreatToggle.onValueChanged.AddListener(CursorManager.ChangeToBasic);
        retreatToggle.onClick += Retreat;

        healToggle.onValueChanged.AddListener(squadUnit.OnActionSelected);
        healToggle.onValueChanged.AddListener(CursorManager.ChangeToBasic);
        healToggle.onClick += Heal;

        buildHQToggle.onValueChanged.AddListener(squadUnit.OnActionSelected);
        buildHQToggle.onValueChanged.AddListener(squadUnit.OnBuildHQSelected);
        buildHQToggle.onValueChanged.AddListener(CursorManager.ChangeToBuildHQ);
        buildHQToggle.onValueChanged.AddListener(BuildHQ);

        buildTurretToggle.onValueChanged.AddListener(squadUnit.OnActionSelected);
        buildTurretToggle.onValueChanged.AddListener(squadUnit.OnBuildTurretSelected);
        buildTurretToggle.onValueChanged.AddListener(CursorManager.ChangeToBuildTurret);
        buildTurretToggle.onValueChanged.AddListener(BuildTurret);

        explosivesToggle.onValueChanged.AddListener(squadUnit.OnActionSelected);
        explosivesToggle.onValueChanged.AddListener(squadUnit.OnExplosivesSelected);
        explosivesToggle.onValueChanged.AddListener(CursorManager.ChangeToExplosives);
        explosivesToggle.onValueChanged.AddListener(Explosives);

        squadUnit.OnHQBack += RemoveSquad;
        squadUnit.OnDeath += RemoveSquad;

        // Activate the object and change the background color
        gameObject.SetActive(true);
        background.color = squadUnit.Squad.Color;

        // Update SoldierImages (background and border) from SquadUnit's Squad data
        soldier1Image.Setup(squadUnit.Squad.Soldiers[0]);
        soldier2Image.Setup(squadUnit.Squad.Soldiers[1]);
        soldier3Image.Setup(squadUnit.Squad.Soldiers[2]);
        soldier4Image.Setup(squadUnit.Squad.Soldiers[3]);
        
        // Initialize Capacities amount
        healCapacity = 0;
        buildHQCapacity = 0;
        buildTurretCapacity = 0;
        explosivesCapacity = 0;
        foreach(Soldier _s in squadUnit.Squad.Soldiers)
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
        healToggle.gameObject.SetActive(false);
        buildHQToggle.gameObject.SetActive(false);
        buildTurretToggle.gameObject.SetActive(false);
        explosivesToggle.gameObject.SetActive(false);
        if (healCapacity > 0)
        {
            healToggle.gameObject.SetActive(true);
            healToggle.GetComponentInChildren<Text>().text = healCapacity.ToString();
        }
        if (buildHQCapacity > 0)
        {
            buildHQToggle.gameObject.SetActive(true);
            buildHQToggle.GetComponentInChildren<Text>().text = buildHQCapacity.ToString();
        }
        if (buildTurretCapacity > 0)
        {
            buildTurretToggle.gameObject.SetActive(true);
            buildTurretToggle.GetComponentInChildren<Text>().text = buildTurretCapacity.ToString();
        }
        if (explosivesCapacity > 0)
        {
            explosivesToggle.gameObject.SetActive(true);
            explosivesToggle.GetComponentInChildren<Text>().text = explosivesCapacity.ToString();
        }

        isSet = true;
        PlayManager.OnRetreatAll += Retreat;
    }

    /// <summary>
    /// RemoveSquad method resets the SquadActionPanel and clear the attached SquadUnit
    /// </summary>
    private void RemoveSquad()
    {
        Reset();
        squadUnit = null;
    }

    /// <summary>
    /// Reset method hides the panel and sets it as "uninitialized"
    /// </summary>
    public void Reset()
    {
        SelectSquad(false);

        moveToggle.onValueChanged.RemoveAllListeners();
        retreatToggle.onValueChanged.RemoveAllListeners();
        retreatToggle.onClick -= Retreat;
        healToggle.onValueChanged.RemoveAllListeners();
        healToggle.onClick -= Heal;
        buildHQToggle.onValueChanged.RemoveAllListeners();
        buildTurretToggle.onValueChanged.RemoveAllListeners();
        explosivesToggle.onValueChanged.RemoveAllListeners();

        if (squadUnit != null)
        {
            squadUnit.Soldier1.OnDamage -= soldier1HealthBar.UpdateValue;
            squadUnit.Soldier2.OnDamage -= soldier2HealthBar.UpdateValue;
            squadUnit.Soldier3.OnDamage -= soldier3HealthBar.UpdateValue;
            squadUnit.Soldier4.OnDamage -= soldier4HealthBar.UpdateValue;

            squadUnit.OnHQBack -= RemoveSquad;
            squadUnit.OnDeath -= RemoveSquad;
            squadUnit = null;
        }
        PlayManager.OnRetreatAll -= Retreat;
        isSet = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// OnDestroy unsubscribes from events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnRetreatAll -= Retreat;
        Reset();        
    }

    /// <summary>
    /// Retrat method triggers the Retreat action of the SquadUnit and hides this panel
    /// </summary>
    public void Retreat()
    {
        squadUnit.Retreat();
        gameObject.SetActive(false);
        OnRetreat?.Invoke();
    }

    /// <summary>
    /// Heal method triggers the Heal action of the SquadUnit if Heal amount is positive
    /// </summary>
    private void Heal()
    {
        if(healCapacity > 0)
        {
            healCapacity--;
            healToggle.GetComponentInChildren<Text>().text = healCapacity.ToString();
            squadUnit.Heal();
            if (healCapacity == 0)
            {
                healToggle.interactable = false;
            }
        }
    }

    /// <summary>
    /// BuildHQ method subscribes methods to Squad events to build the HQ or cancel the action, if the BuildHQ amount is positive
    /// </summary>
    private void BuildHQ(bool _isOn)
    {
        if(_isOn)
        {
            if (buildHQCapacity > 0)
            {
                squadUnit.OnActionDone += BuildHQDone;
                squadUnit.OnUnselection += BuildHQCancel;
            }
        }
        else
        {
            BuildHQCancel();
        }
    }

    /// <summary>
    /// BuildHQDone method decrements the BuildHQ capacity and unsubscribes from squad events
    /// </summary>
    private void BuildHQDone()
    {
        buildHQCapacity--;
        buildHQToggle.GetComponentInChildren<Text>().text = buildHQCapacity.ToString();
        if (buildHQCapacity == 0)
        {
            buildHQToggle.interactable = false;
        }
        squadUnit.OnActionDone -= BuildHQDone;
        squadUnit.OnUnselection -= BuildHQCancel;
    }

    /// <summary>
    /// BuildHQCancel method unsubscribes from squad events when the BuildHQ action has been canceled
    /// </summary>
    private void BuildHQCancel()
    {
        squadUnit.OnActionDone -= BuildHQDone;
        squadUnit.OnUnselection -= BuildHQCancel;
    }

    /// <summary>
    /// BuildTurret method subscribes methods to Squad events to build the turret or cancel the action, if the BuildTurret amount is positive
    /// </summary>
    private void BuildTurret(bool _isOn)
    {

        if (_isOn)
        {
            if (buildTurretCapacity > 0)
            {
                squadUnit.OnActionDone += BuildTurretDone;
                squadUnit.OnUnselection += BuildTurretCancel;
            }
        }
        else
        {
            BuildTurretCancel();
        }        
    }

    /// <summary>
    /// BuildTurretDone method decrements the BuildTurret capacity and unsubscribes from squad events
    /// </summary>
    private void BuildTurretDone()
    {
        buildTurretCapacity--;
        buildTurretToggle.GetComponentInChildren<Text>().text = buildTurretCapacity.ToString();
        if (buildTurretCapacity == 0)
        {
            buildTurretToggle.interactable = false;
        }
        squadUnit.OnActionDone -= BuildTurretDone;
        squadUnit.OnUnselection -= BuildTurretCancel;
    }

    /// <summary>
    /// BuildTurretCancel method unsubscribes from squad events when the BuildTurret action has been canceled
    /// </summary>
    private void BuildTurretCancel()
    {
        squadUnit.OnActionDone -= BuildTurretDone;
        squadUnit.OnUnselection -= BuildTurretCancel;
    }

    /// <summary>
    /// Explosives method triggers the Explosives action of the SquadUnit if Explosives amount is positive => TBD
    /// </summary>
    private void Explosives(bool _isOn)
    {
        if (_isOn)
        {
            if (explosivesCapacity > 0)
            {
                squadUnit.OnActionDone += ExplosivesDone;
                squadUnit.OnUnselection += ExplosivesCancel;
            }
        }
        else
        {
            ExplosivesCancel();
        }
    }

    /// <summary>
    /// ExplosivesDone method decrements the Explosives capacity and unsubscribes from squad events
    /// </summary>
    private void ExplosivesDone()
    {
        explosivesCapacity--;
        explosivesToggle.GetComponentInChildren<Text>().text = explosivesCapacity.ToString();
        if (explosivesCapacity == 0)
        {
            explosivesToggle.interactable = false;
        }
        squadUnit.OnActionDone -= ExplosivesDone;
        squadUnit.OnUnselection -= ExplosivesCancel;
    }

    /// <summary>
    /// ExplosivesCancel method unsubscribes from squad events when the Explosives action has been canceled
    /// </summary>
    private void ExplosivesCancel()
    {
        squadUnit.OnActionDone -= ExplosivesDone;
        squadUnit.OnUnselection -= ExplosivesCancel;
    }

    /// <summary>
    /// RequestSquadSelection method requests the Squad selection when clicking on it
    /// </summary>
    public void RequestSquadSelection()
    {
        OnSelectionRequest?.Invoke();
    }

    /// <summary>
    /// SelectSquad method selects and unselects the SquadActionPanel and the SquadUnit linked to it
    /// </summary>
    /// <param name="_b">Selected or not</param>
    public void SelectSquad(bool _b)
    {
        if (_b) // Selection
        {
            // Select the first Action button and the associated SquadUnit method
            moveToggle.isOn = true;
            //moveToggle.Select();
            //selectedButton = moveAction;
            ActivateInteractions();
            OnSelection?.Invoke();
        }
        else // Unselection
        {
            if (isSet)
            {
                // Make all buttons uninteractable
                moveToggle.isOn = false;
                moveToggle.interactable = false;
                retreatToggle.isOn = false;
                retreatToggle.interactable = false;
                healToggle.isOn = false;
                healToggle.interactable = false;
                buildHQToggle.isOn = false;
                buildHQToggle.interactable = false;
                buildTurretToggle.isOn = false;
                buildTurretToggle.interactable = false;
                explosivesToggle.isOn = false;
                explosivesToggle.interactable = false;
                GameManager.PlayUpdate -= SquadActionPanelUpdate;
            }
        }
        // Enable (or not) the Overlay image and the raycast on the background (to enable the selection)
        squadSelectionOverlay.enabled = !_b;
    }

    /// <summary>
    /// ActivateInteractions method sets all buttons to interactable and subscribe to Update event
    /// </summary>
    private void ActivateInteractions()
    {
        moveToggle.interactable = true;
        retreatToggle.interactable = true;
        if(healCapacity >0) healToggle.interactable = true;
        if(buildHQCapacity >0) buildHQToggle.interactable = true;
        if (buildTurretCapacity > 0) buildTurretToggle.interactable = true;
        if(explosivesCapacity > 0) explosivesToggle.interactable = true;
   
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
            if (moveToggle.isOn)
            {
                retreatToggle.isOn = true;
            }
            else if (retreatToggle.isOn)
            {
                if (healCapacity > 0)
                {
                    healToggle.isOn = true;
                }
                else if(buildHQCapacity > 0)
                {
                    buildHQToggle.isOn = true;
                }
                else if(buildTurretCapacity > 0)
                {
                    buildTurretToggle.isOn = true;
                }
                else if(explosivesCapacity > 0)
                {
                    explosivesToggle.isOn = true;
                }
                else
                {
                    moveToggle.isOn = true;
                }
            }
            else if(healToggle.isOn)
            {
                if (buildHQCapacity > 0)
                {
                    buildHQToggle.isOn = true;
                }
                else if (buildTurretCapacity > 0)
                {
                    buildTurretToggle.isOn = true;
                }
                else if (explosivesCapacity > 0)
                {
                    explosivesToggle.isOn = true;
                }
                else
                {
                    moveToggle.isOn = true;
                }
            }
            else if(buildHQToggle.isOn)
            {
                if (buildTurretCapacity > 0)
                {
                    buildTurretToggle.isOn = true;
                }
                else if (explosivesCapacity > 0)
                {
                    explosivesToggle.isOn = true;
                }
                else
                {
                    moveToggle.isOn = true;
                }
            }
            else if(buildTurretToggle.isOn)
            {
                if (explosivesCapacity > 0)
                {
                    explosivesToggle.isOn = true;
                }
                else
                {
                    moveToggle.isOn = true;
                }
            }
            else if(explosivesToggle.isOn)
            {
                moveToggle.isOn = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (moveToggle.isOn)
            {
                if (explosivesCapacity > 0)
                {
                    explosivesToggle.isOn = true;
                }
                else if (buildTurretCapacity > 0)
                {
                    buildTurretToggle.isOn = true;
                }
                else if (buildHQCapacity > 0)
                {
                    buildHQToggle.isOn = true;
                }
                else if (healCapacity > 0)
                {
                    healToggle.isOn = true;
                }
                else
                {
                    retreatToggle.isOn = true;
                }
            }
            else if (retreatToggle.isOn)
            {
                moveToggle.isOn = true;
            }
            else if (healToggle.isOn)
            {
                retreatToggle.isOn = true;
            }
            else if (buildHQToggle.isOn)
            {
                if (healCapacity > 0)
                {
                    healToggle.isOn = true;
                }
                else
                {
                    retreatToggle.isOn = true;
                }
            }
            else if (buildTurretToggle.isOn)
            {
                if (buildHQCapacity > 0)
                {
                    buildHQToggle.isOn = true;
                }
                else if (healCapacity > 0)
                {
                    healToggle.isOn = true;
                }
                else
                {
                    retreatToggle.isOn = true;
                }
            }
            else if (explosivesToggle.isOn)
            {
                if (buildTurretCapacity > 0)
                {
                    buildTurretToggle.isOn = true;
                }
                else if (buildHQCapacity > 0)
                {
                    buildHQToggle.isOn = true;
                }
                else if (healCapacity > 0)
                {
                    healToggle.isOn = true;
                }
                else
                {
                    retreatToggle.isOn = true;
                }
            }
        }
    }
}
