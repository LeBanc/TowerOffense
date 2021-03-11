using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HQCCSoldierSelection is the class managing the HQ - Command Center - Soldier Selection Canvas
/// </summary>
public class HQCCSoldierSelection : MonoBehaviour
{
    // public elements of the canvas
    public AutoScroll autoScroll;

    // public prefab
    public GameObject soldierSelectionItem;

    private Soldier selectedSoldier;
    private Squad squad;
    private int position;
    private SoldierSelectionItem selectedItem;

    // Events
    public delegate void SoldierSelectionEventHandler();
    public event SoldierSelectionEventHandler OnCanvasHide;

    /// <summary>
    /// Show method shows the Selection canvas, filling it with all living soldiers except the seleced one
    /// </summary>
    /// <param name="_squad">Selected squad</param>
    /// <param name="_position">Position of the soldier in the squad</param>
    /// <param name="_selectedSoldier">Selected soldier</param>
    public void Show(Squad _squad, int _position, Soldier _selectedSoldier)
    {
        Clear();
        selectedSoldier = _selectedSoldier;
        squad = _squad;
        position = _position;

        CreateSoldierList(PlayManager.soldierList);

        // Display the canvas
        GetComponent<Canvas>().enabled = true;
        transform.SetAsLastSibling();

        GameObject _selection = autoScroll.SelectFirtsItem();
        if (_selection != null) selectedItem = _selection.GetComponent<SoldierSelectionItem>();
    }

    /// <summary>
    /// CreateSoldierList method creates the soldiers items on screen from a sorted soldiers list
    /// </summary>
    /// <param name="_soldiers">Sorted soldiers list</param>
    private void CreateSoldierList(List<Soldier> _soldiers)
    {
        // Loop through all soldiers
        foreach (Soldier _soldier in _soldiers)
        {
            if (_soldier == selectedSoldier) continue; // If the soldier is the selected one, don't display it
            if (_soldier.IsDead) continue; // If the soldier is dead, don't display it

            // Else create a new SoldierSelectionItem and initialize it
            GameObject _item = autoScroll.AddPrefabReturnInstance(soldierSelectionItem);
            _item.GetComponent<SoldierSelectionItem>().Setup(_soldier);
            _item.GetComponent<SoldierSelectionItem>().OnSelection += ChangeSelection;
        }
    }

    /// <summary>
    /// Hide method hides and clears the canvas
    /// </summary>
    public void Hide()
    {
        transform.SetAsFirstSibling();
        GetComponent<Canvas>().enabled = false;
        Clear();
        OnCanvasHide?.Invoke();
    }

    /// <summary>
    /// Clear method destroys all the SoldierSelectionItems from the children list
    /// </summary>
    private void Clear()
    {
        foreach(GameObject _go in autoScroll.List)
        {
            _go.GetComponent<SoldierSelectionItem>().OnSelection -= ChangeSelection;
        }

        autoScroll.Clear();
    }

    /// <summary>
    /// SortSoldierList method sorts the soldier list (from PlayManager) depending of dropdown data
    /// </summary>
    /// <param name="_order">Dropdown selection data</param>
    public void SortSoldierList(Int32 _order)
    {
        Clear();
        List<Soldier> _soldiers = new List<Soldier>(PlayManager.soldierList);
        switch (_order)
        {
            case 1:
                _soldiers.Sort(Soldier.SortByName);
                break;
            case 2:
                _soldiers.Sort(Soldier.SortByLevelLower);
                break;
            case 3:
                _soldiers.Sort(Soldier.SortByLevelGreater);
                break;
            case 4:
                _soldiers.Sort(Soldier.SortByType);
                break;
            case 5:
                _soldiers.Sort(Soldier.SortBySquad);
                break;
            default:
                _soldiers.Sort(Soldier.SortByID);
                break;
        }
        CreateSoldierList(_soldiers);
    }

    /// <summary>
    /// ChangeSelection method selects the chosen item
    /// </summary>
    /// <param name="_item">Item to select (SoldierSelectionItem)</param>
    private void ChangeSelection(SoldierSelectionItem _item)
    {
        if (selectedItem != null)
        {
            if (selectedItem == _item) return;
            selectedItem.Unselect();
        }
        selectedItem = _item;
    }

    /// <summary>
    /// Validate method is the method used when the "Validate/OK" button is clicked/validated. It set the selected soldier as the new soldier
    /// </summary>
    public void Validate()
    {
        squad.ChangeSoldier(position, selectedItem.Soldier);
        Hide();
    }

}
