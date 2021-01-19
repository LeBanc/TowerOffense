﻿using UnityEngine;

/// <summary>
/// HQCommandCenter is the class used by the CommandCenter canvas
/// </summary>
public class HQCommandCenter : UICanvas
{
    // Public elements used in the prefab
    public HQSquadHeader squad1Header;
    public HQSquadHeader squad2Header;
    public HQSquadHeader squad3Header;
    public HQSquadHeader squad4Header;

    public HQSquadEdition squadEditionCanvas;

    // Private Squad to save the selected squad
    private Squad selectedSquad;

    /// <summary>
    /// On Awake, fetches the canvas and subscribe to events
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        squad1Header.OnSelection += SelectSquadHeader;
        squad2Header.OnSelection += SelectSquadHeader;
        squad3Header.OnSelection += SelectSquadHeader;
        squad4Header.OnSelection += SelectSquadHeader;

        squad1Header.OnUnlock += delegate { UpdateSquadHeaders(); };
        squad2Header.OnUnlock += delegate { UpdateSquadHeaders(); };
        squad3Header.OnUnlock += delegate { UpdateSquadHeaders(); };
        squad4Header.OnUnlock += delegate { UpdateSquadHeaders(); };
    }

    /// <summary>
    /// OnDestroy, unsubscribes to all events
    /// </summary>
    private void OnDestroy()
    {
        squad1Header.OnSelection -= SelectSquadHeader;
        squad2Header.OnSelection -= SelectSquadHeader;
        squad3Header.OnSelection -= SelectSquadHeader;
        squad4Header.OnSelection -= SelectSquadHeader;

        squad1Header.OnUnlock -= delegate { UpdateSquadHeaders(); };
        squad2Header.OnUnlock -= delegate { UpdateSquadHeaders(); };
        squad3Header.OnUnlock -= delegate { UpdateSquadHeaders(); };
        squad4Header.OnUnlock -= delegate { UpdateSquadHeaders(); };
    }

    /// <summary>
    /// Show methods displays the canvas and its subcanvas
    /// </summary>
    public override void Show()
    {
        base.Show();

        squad1Header.Show();
        squad2Header.Show();
        squad3Header.Show();
        squad4Header.Show();

        //Update headers
        UpdateSquadHeaders();
    }

    /// <summary>
    /// Hide methods hides the canvas and its subcanvas
    /// </summary>
    public override void Hide()
    {
        // Hide canvas and subcanvas
        squad1Header.Hide();
        squad2Header.Hide();
        squad3Header.Hide();
        squad4Header.Hide();
        squadEditionCanvas.Hide();

        base.Hide();
    }

    /// <summary>
    /// SelectSquadHeader select the dedicate Squad Header
    /// </summary>
    /// <param name="_squadHeader">Header to select</param>
    private void SelectSquadHeader(HQSquadHeader _squadHeader)
    {
        SelectSquad(_squadHeader.Squad);
    }

    /// <summary>
    /// SelectSquad method sets the Squad as the selected one and displays it in SquadEdition
    /// </summary>
    /// <param name="_squad">Squad to select</param>
    public void SelectSquad(Squad _squad)
    {
        if(_squad != selectedSquad)
        {
            selectedSquad = _squad;
            squadEditionCanvas.DisplaySquad(_squad);
        }
    }

    /// <summary>
    /// UpdateSquadHeaders updates all the SquadHeaders if needed from the PlayManager Squad list
    /// </summary>
    public void UpdateSquadHeaders()
    {
        // if there is more than 0 squad in the list
        if(PlayManager.squadList.Count > 0)
        {
            if(squad1Header.Squad != PlayManager.squadList[0]) // If the header is null or after a game load
            {
                squad1Header.Setup(PlayManager.squadList[0]); // Initialize the header
                squad2Header.ReadyToUnlock(); // Set the next squad as ready to unlock
            }
        }
        if (PlayManager.squadList.Count > 1)
        {
            if (squad2Header.Squad != PlayManager.squadList[1])
            {
                squad2Header.Setup(PlayManager.squadList[1]);
                squad3Header.ReadyToUnlock();
                // If there is already a selected squad, this is an unlock situation so select this squad
                if (selectedSquad != null) selectedSquad = PlayManager.squadList[1]; 
            }
        }
        if (PlayManager.squadList.Count > 2)
        {
            if (squad3Header.Squad != PlayManager.squadList[2])
            {
                squad3Header.Setup(PlayManager.squadList[2]);
                squad4Header.ReadyToUnlock();
                if (selectedSquad != null) selectedSquad = PlayManager.squadList[2];
            }
        }
        if (PlayManager.squadList.Count > 3)
        {
            if (squad4Header.Squad != PlayManager.squadList[3])
            {
                squad4Header.Setup(PlayManager.squadList[3]);
                if (selectedSquad != null) selectedSquad = PlayManager.squadList[3];
            }
        }
        // If no squad is selected, select the first squad
        if (selectedSquad == null) selectedSquad = PlayManager.squadList[0];
        // Display the selected squad in the Edition Canvas
        if (selectedSquad != null) squadEditionCanvas.DisplaySquad(selectedSquad);
    }    
}
