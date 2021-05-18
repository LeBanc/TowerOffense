using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HQBarracks : UICanvas
{
    // Graphical element for the soldier data display
    public SoldierUpgradeCanvas soldierUpgrade;
    // Dropdown to sort the list
    public Dropdown sortDropdown;
    // AutoScroll component (of the scroll view)
    public AutoScroll autoScroll;
    // Prefab of a SoldierListItem
    public GameObject soldierListItem;

    // private selected soldier item
    private SoldierListItem selectedSoldierItem;

    /// <summary>
    /// Show methods initializes the canvas and the soldier list with all soldiers to display
    /// </summary>
    public override void Show()
    {
        // If canvas is not visible, init and show it
        if (!canvas.enabled)
        {
            base.Show();
            soldierUpgrade.Show();

            // If not sort by ID, change the sort type (that will create the soldier list)
            if(sortDropdown.value != 0)
            {
                sortDropdown.value = 0;
            }
            else // else create the list
            {
                CreateSoldierListItems(PlayManager.soldierList);
            }
            HQCanvas.OnSecondaryShortcut += sortDropdown.Show;
        }

        // Set the first soldier as default navigation from autoscroll exits
        autoScroll.SetNavToFirstObject();
    }

    /// <summary>
    /// CreateSoldierListItems methods creates the SoldierListItems from a sorted soldiers list
    /// </summary>
    /// <param name="_soldiers">Sorted soldiers list</param>
    private void CreateSoldierListItems(List<Soldier> _soldiers)
    {
        // Initialize Soldier Item List
        int number = 0;
        foreach (Soldier _soldier in _soldiers)
        {
            if (!_soldier.IsDead)
            {
                GameObject _go = autoScroll.AddPrefabReturnInstance(soldierListItem);
                SoldierListItem _item = _go.GetComponent<SoldierListItem>();
                _item.Setup(_soldier);
                _item.OnSelection += soldierUpgrade.Setup;
                _item.OnSelection += delegate { ChangeSelection(_item); };
                if (number == 0)
                {
                    selectedSoldierItem = _item;
                    selectedSoldierItem.Select();
                }
                number++;
            }
        }
    }

    /// <summary>
    /// ClearSoldierListItems method clears the displayed SoldierItems
    /// </summary>
    private void ClearSoldierListItems()
    {
        // Clear Soldier Item List
        foreach(GameObject _go in autoScroll.List)
        {
            if (_go.TryGetComponent<SoldierListItem>(out SoldierListItem _soldierItem))
            {
                _soldierItem.OnSelection -= soldierUpgrade.Setup;
                _soldierItem.OnSelection -= delegate { ChangeSelection(_soldierItem); };
            }
        }
        autoScroll.Clear();
    }

    /// <summary>
    /// Hide method deletes all soldierList items and hides the canvas
    /// </summary>
    public override void Hide()
    {
        HQCanvas.OnSecondaryShortcut -= sortDropdown.Show;
        sortDropdown.Hide();

        soldierUpgrade.Hide();

        ClearSoldierListItems();

        base.Hide();
    }

    /// <summary>
    /// ChangeSelection method unsubscribe events of the selected soldierItem and unselects it before subscribing events of the new one
    /// </summary>
    /// <param name="_soldierItem">SoldierItem to select</param>
    private void ChangeSelection(SoldierListItem _soldierItem)
    {
        if (selectedSoldierItem != null)
        {
            if (selectedSoldierItem == _soldierItem) return;
            selectedSoldierItem.Unselect();
        }
        selectedSoldierItem = _soldierItem;
    }

    /// <summary>
    /// SortSoldierList method sorts the soldier list (from PlayManager) depending of a dropdown data
    /// </summary>
    /// <param name="_order">Dropdown data</param>
    public void SortSoldierList(Int32 _order)
    {
        ClearSoldierListItems();
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
        CreateSoldierListItems(_soldiers);
    }
}
