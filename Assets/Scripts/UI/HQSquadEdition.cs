﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQSquadEdition is used to disply the Squad Edition Canvas
/// </summary>
public class HQSquadEdition : MonoBehaviour
{
    // Public elements used in the prefab
    public ColorPickerCanvas colorPickerCanvas;

    public Image squadTypeImage;
    public Image squadColorBkg;
    
    public SoldierImage soldier1Image;
    public SoldierImage soldier2Image;
    public SoldierImage soldier3Image;
    public SoldierImage soldier4Image;

    public Dropdown squadTypeDropdown;
    public Button squadColorChange;
    public Dropdown squadPositionChoice;
    public Dropdown squadRangeSelection;

    public Button soldier1Change;
    public Button soldier2Change;
    public Button soldier3Change;
    public Button soldier4Change;

    public SoldierPreview soldier1Preview;
    public SoldierPreview soldier2Preview;
    public SoldierPreview soldier3Preview;
    public SoldierPreview soldier4Preview;

    // Private Squad that is displayed
    private Squad selectedSquad;

    /// <summary>
    /// On Start, subscribe events and hides the Color Picker Canvas
    /// </summary>
    private void Start()
    {
        squadTypeDropdown.onValueChanged.AddListener(delegate { ChangeSquadType(squadTypeDropdown.value); });
        squadPositionChoice.onValueChanged.AddListener(delegate { ChangeSquadPositionning(squadPositionChoice.value); });
        squadRangeSelection.onValueChanged.AddListener(delegate { ChangePlayerRange(squadRangeSelection.value); });
        colorPickerCanvas.Hide();
    }

    /// <summary>
    /// DisplaySquad method dispalys the squad elements in the SquadEdition canvas
    /// </summary>
    /// <param name="_squad">Squad to display</param>
    public void DisplaySquad(Squad _squad)
    {
        // If the squad changes, update display and events
        if(selectedSquad != _squad)
        {
            // If the previous squad was not null, unsubscribe events
            if(selectedSquad != null)
            {
                selectedSquad.OnTypeChange -= UpdateSquadType;
                selectedSquad.OnColorChange -= UpdateSquadColor;
                selectedSquad.OnSoldier1Change -= UpdateSoldier1;
                selectedSquad.OnSoldier2Change -= UpdateSoldier2;
                selectedSquad.OnSoldier3Change -= UpdateSoldier3;
                selectedSquad.OnSoldier4Change -= UpdateSoldier4;
            }           

            // Set the squad as selected, update displays and subscribe events
            selectedSquad = _squad;
            UpdateSquadType();
            selectedSquad.OnTypeChange += UpdateSquadType;
            UpdateSquadColor();
            selectedSquad.OnColorChange += UpdateSquadColor;
            UpdateSoldier1();
            selectedSquad.OnSoldier1Change += UpdateSoldier1;
            UpdateSoldier2();
            selectedSquad.OnSoldier2Change += UpdateSoldier2;
            UpdateSoldier3();
            selectedSquad.OnSoldier3Change += UpdateSoldier3;
            UpdateSoldier4();
            selectedSquad.OnSoldier4Change += UpdateSoldier4;

            UpdateDropdowns();
        }
    }

    /// <summary>
    /// UpdateDropdowns initializes the dropdown displays
    /// </summary>
    private void UpdateDropdowns()
    {
        // Initialize the squad type dropdown
        switch (selectedSquad.SquadType.squadTypeName)
        {
            case "Losange":
                squadTypeDropdown.value = 0;
                break;
            case "Forward Arrow":
                squadTypeDropdown.value = 1;
                break;
            case "Backward Arrow":
                squadTypeDropdown.value = 2;
                break;
            case "Square":
                squadTypeDropdown.value = 3;
                break;
            default:
                squadTypeDropdown.value = 0;
                break;
        }
        // Initialize the preferred range dropdown
        switch (selectedSquad.PrefRange)
        {
            case Squad.PreferedRange.ShortRange:
                squadRangeSelection.value = 0;
                break;
            case Squad.PreferedRange.MiddleRange:
                squadRangeSelection.value = 1;
                break;
            case Squad.PreferedRange.LongRange:
                squadRangeSelection.value = 2;
                break;
            default:
                squadRangeSelection.value = 1;
                break;
        }
        // Initialize the position choice dropdown
        switch (selectedSquad.PosChoice)
        {
            case Squad.PositionChoice.MaximizeAttack:
                squadPositionChoice.value = 0;
                squadRangeSelection.gameObject.SetActive(false);
                break;
            case Squad.PositionChoice.MaximizeDefense:
                squadPositionChoice.value = 1;
                squadRangeSelection.gameObject.SetActive(false);
                break;
            case Squad.PositionChoice.PlayerChoice:
                squadPositionChoice.value = 2;
                squadRangeSelection.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// ChangeSquadType is used to change the squad type image through the dropdown
    /// </summary>
    /// <param name="_newType">Type to change to</param>
    private void ChangeSquadType(int _newType)
    {
        if(selectedSquad != null)
        {
            switch (_newType)
            {
                case 0:
                    selectedSquad.ChangeSquadType(Resources.Load("SquadData/LosangeSquadData") as SquadData);
                    break;
                case 1:
                    selectedSquad.ChangeSquadType(Resources.Load("SquadData/FwdArrowSquadData") as SquadData);
                    break;
                case 2:
                    selectedSquad.ChangeSquadType(Resources.Load("SquadData/BwdArrowSquadData") as SquadData);
                    break;
                case 3:
                    selectedSquad.ChangeSquadType(Resources.Load("SquadData/SquareSquadData") as SquadData);
                    break;
                default:
                    selectedSquad.ChangeSquadType(Resources.Load("SquadData/LosangeSquadData") as SquadData);
                    break;
            }
        }
    }

    /// <summary>
    /// OpenColorPicker method opens the color picher canvas
    /// </summary>
    public void OpenColorPicker()
    {
        colorPickerCanvas.Setup(selectedSquad);
        colorPickerCanvas.Show();
    }

    /// <summary>
    /// ChangeSquadPositionning is used to change the squad position choice through the dropdown
    /// </summary>
    /// <param name="_newChoice">New choice</param>
    private void ChangeSquadPositionning(int _newChoice)
    {
        if(selectedSquad != null)
        {
            switch (_newChoice)
            {
                case 0: // PosChoice is MaximizeAttack: hide the manual selection dropdown
                    selectedSquad.UpdatePosChoice(Squad.PositionChoice.MaximizeAttack);
                    squadRangeSelection.gameObject.SetActive(false);
                    break;
                case 1: // PosChoice is Maximizedefense: hide the manual selection dropdown
                    selectedSquad.UpdatePosChoice(Squad.PositionChoice.MaximizeDefense);
                    squadRangeSelection.gameObject.SetActive(false);
                    break;
                case 2: // PosChoice is PlayerChoice: show the manual selection dropdown
                    selectedSquad.UpdatePosChoice(Squad.PositionChoice.PlayerChoice);
                    switch (selectedSquad.PrefRange)
                    {
                        case Squad.PreferedRange.ShortRange:
                            squadRangeSelection.value = 0;
                            break;
                        case Squad.PreferedRange.MiddleRange:
                            squadRangeSelection.value = 1;
                            break;
                        case Squad.PreferedRange.LongRange:
                            squadRangeSelection.value = 2;
                            break;
                        default:
                            squadRangeSelection.value = 1;
                            break;
                    }
                    squadRangeSelection.gameObject.SetActive(true);
                    break;
                default:
                    selectedSquad.UpdatePosChoice(Squad.PositionChoice.MaximizeAttack);
                    squadRangeSelection.gameObject.SetActive(false);
                    break;
            }
        }
    }

    /// <summary>
    /// ChangePlayerRange is used to change the preferred range through the dropdown
    /// </summary>
    /// <param name="_newRange">New range</param>
    private void ChangePlayerRange(int _newRange)
    {
        if (selectedSquad != null)
        {
            switch (_newRange)
            {
                case 0:
                    selectedSquad.PrefRange = Squad.PreferedRange.ShortRange;
                    break;
                case 1:
                    selectedSquad.PrefRange = Squad.PreferedRange.MiddleRange;
                    break;
                case 2:
                    selectedSquad.PrefRange = Squad.PreferedRange.LongRange;
                    break;
                default:
                    selectedSquad.PrefRange = Squad.PreferedRange.MiddleRange;
                    break;
            }
            selectedSquad.UpdatePosChoice(selectedSquad.PosChoice);
        }
    }

    /// <summary>
    /// UpdateSquadType updates the squad type display and the soldier image positions
    /// </summary>
    private void UpdateSquadType()
    {
        squadTypeImage.sprite = selectedSquad.SquadType.squadTypeSprite;
        soldier1Image.transform.localPosition = selectedSquad.SquadType.soldier1SpritePosition;
        soldier2Image.transform.localPosition = selectedSquad.SquadType.soldier2SpritePosition;
        soldier3Image.transform.localPosition = selectedSquad.SquadType.soldier3SpritePosition;
        soldier4Image.transform.localPosition = selectedSquad.SquadType.soldier4SpritePosition;
    }

    /// <summary>
    /// UpdateSquadColor updates the Squad color
    /// </summary>
    private void UpdateSquadColor()
    {
        squadColorBkg.color = selectedSquad.Color;
    }

    /// <summary>
    /// UpdateSoldier1 updates the soldier 1 image and the soldier 1 preview
    /// </summary>
    private void UpdateSoldier1()
    {
        soldier1Image.Setup(selectedSquad.Soldiers[0]);
        soldier1Preview.Setup(selectedSquad.Soldiers[0]);
    }

    /// <summary>
    /// UpdateSoldier2 updates the soldier 2 image and the soldier 2 preview
    /// </summary>
    private void UpdateSoldier2()
    {
        soldier2Image.Setup(selectedSquad.Soldiers[1]);
        soldier2Preview.Setup(selectedSquad.Soldiers[1]);
    }

    /// <summary>
    /// UpdateSoldier3 updates the soldier 3 image and the soldier 3 preview
    /// </summary>
    private void UpdateSoldier3()
    {
        soldier3Image.Setup(selectedSquad.Soldiers[2]);
        soldier3Preview.Setup(selectedSquad.Soldiers[2]);
    }

    /// <summary>
    /// UpdateSoldier4 updates the soldier 4 image and the soldier 4 preview
    /// </summary>
    private void UpdateSoldier4()
    {
        soldier4Image.Setup(selectedSquad.Soldiers[3]);
        soldier4Preview.Setup(selectedSquad.Soldiers[3]);
    }

    /// <summary>
    /// OnDestroy, unsubscribe events
    /// </summary>
    private void OnDestroy()
    {
        if(selectedSquad != null)
        {
            selectedSquad.OnTypeChange -= UpdateSquadType;
            selectedSquad.OnColorChange -= UpdateSquadColor;
            selectedSquad.OnSoldier1Change -= UpdateSoldier1;
            selectedSquad.OnSoldier2Change -= UpdateSoldier2;
            selectedSquad.OnSoldier3Change -= UpdateSoldier3;
            selectedSquad.OnSoldier4Change -= UpdateSoldier4;
        }
    }

}
