using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SquadActionPanel : MonoBehaviour
{
    public Image background;
    public Image squadSelectionOverlay;
    
    public Image soldier1Image;
    public Image soldier2Image;
    public Image soldier3Image;
    public Image soldier4Image;

    public HealthBar soldier1HealthBar;
    public HealthBar soldier2HealthBar;
    public HealthBar soldier3HealthBar;
    public HealthBar soldier4HealthBar;

    public Button moveAction;
    public Button retreatAction;
    public Button specialAction1;
    public Button specialAction2;
    public Button specialAction3;
    public Button specialAction4;

    public Sprite moveActionSprite;
    public Sprite retreatActionSprite;
    public Sprite healActionSprite;
    public Sprite buildHQActionSprite;
    public Sprite buildTurretActionSprite;
    public Sprite explosivesActionSprite;


    private SquadUnit squadUnit;

    private int healCapacity;
    private int buildHQCapacity;
    private int buildTurretCapacity;
    private int explosivesCapacity;

    private Button healButton;
    private Button buildHQButton;
    private Button buildTurretButton;
    private Button explosivesButton;

    public Button selectedButton;

    private void Start()
    {
        moveAction.interactable = false;
        retreatAction.interactable = false;
        specialAction1.interactable = false;
        specialAction2.interactable = false;
        specialAction3.interactable = false;
        specialAction4.interactable = false;
    }

    public void Setup(Squad _squad, SquadUnit _squadUnit)
    {
        gameObject.SetActive(true);
        background.color = _squad.Color;

        soldier1Image.sprite = _squad.Soldiers[0].Image;
        soldier2Image.sprite = _squad.Soldiers[1].Image;
        soldier3Image.sprite = _squad.Soldiers[2].Image;
        soldier4Image.sprite = _squad.Soldiers[3].Image;

        squadUnit = _squadUnit;
        _squadUnit.Soldier1.OnDamage += soldier1HealthBar.UpdateValue;
        _squadUnit.Soldier2.OnDamage += soldier2HealthBar.UpdateValue;
        _squadUnit.Soldier3.OnDamage += soldier3HealthBar.UpdateValue;
        _squadUnit.Soldier4.OnDamage += soldier4HealthBar.UpdateValue;


        healCapacity = 0;
        buildHQCapacity = 0;
        buildTurretCapacity = 0;
        explosivesCapacity = 0;
        foreach(Soldier _s in _squad.Soldiers)
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

        PlayManager.RetreatAll += Retreat;
    }

    private void OnDestroy()
    {
        PlayManager.RetreatAll -= Retreat;
    }

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

    public void Retreat()
    {
        squadUnit.Retreat();
        gameObject.SetActive(false);
    }

    private void Heal()
    {
        if(healCapacity > 0)
        {
            // Heal Action TBD
            healCapacity--;
            healButton.GetComponentInChildren<Text>().text = healCapacity.ToString();
            squadUnit.Heal();
            if (healCapacity == 0)
            {
                healButton.interactable = false;
            }
        }
    }

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

    public void SelectSquad(bool _b)
    {
        if (_b)
        {
            moveAction.Select();
            squadUnit.OnMoveActionSelected();
            selectedButton = moveAction;
            Invoke("ActivateInteractions", 0.05f);
        }
        else
        {
            moveAction.interactable = false;
            retreatAction.interactable = false;
            if (specialAction1.gameObject.activeSelf) specialAction1.interactable = false;
            if (specialAction2.gameObject.activeSelf) specialAction2.interactable = false;
            if (specialAction3.gameObject.activeSelf) specialAction3.interactable = false;
            if (specialAction4.gameObject.activeSelf) specialAction4.interactable = false;
            GameManager.PlayUpdate -= SquadActionPanelUpdate;
        }
        squadSelectionOverlay.enabled = !_b;
        background.raycastTarget = !_b;
    }

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

    public bool IsActive()
    {
        return !squadSelectionOverlay.enabled;
    }

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
