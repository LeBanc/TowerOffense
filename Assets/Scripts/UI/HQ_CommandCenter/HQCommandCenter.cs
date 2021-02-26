using UnityEngine;
using UnityEngine.UI;

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

        PlayManager.OnReset += ResetSelectedSquad;

        squad1Header.OnSelection += SelectSquadHeader;
        squad1Header.OnSelection += delegate { squad2Header.select.Unselect(); squad3Header.select.Unselect(); squad4Header.select.Unselect(); };

        squad2Header.OnSelection += SelectSquadHeader;
        squad2Header.OnSelection += delegate { squad1Header.select.Unselect(); squad3Header.select.Unselect(); squad4Header.select.Unselect(); };

        squad3Header.OnSelection += SelectSquadHeader;
        squad3Header.OnSelection += delegate { squad2Header.select.Unselect(); squad1Header.select.Unselect(); squad4Header.select.Unselect(); };

        squad4Header.OnSelection += SelectSquadHeader;
        squad4Header.OnSelection += delegate { squad2Header.select.Unselect(); squad3Header.select.Unselect(); squad1Header.select.Unselect(); };
    }

    /// <summary>
    /// OnDestroy, unsubscribes to all events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnReset -= ResetSelectedSquad;

        squad1Header.OnSelection -= SelectSquadHeader;
        squad1Header.OnSelection -= delegate { squad2Header.select.Unselect(); squad3Header.select.Unselect(); squad4Header.select.Unselect(); };

        squad2Header.OnSelection -= SelectSquadHeader;
        squad2Header.OnSelection -= delegate { squad1Header.select.Unselect(); squad3Header.select.Unselect(); squad4Header.select.Unselect(); };

        squad3Header.OnSelection -= SelectSquadHeader;
        squad3Header.OnSelection -= delegate { squad2Header.select.Unselect(); squad1Header.select.Unselect(); squad4Header.select.Unselect(); };

        squad4Header.OnSelection -= SelectSquadHeader;
        squad4Header.OnSelection -= delegate { squad2Header.select.Unselect(); squad3Header.select.Unselect(); squad1Header.select.Unselect(); };
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

        // Set Up navigation of Engage button to Selected SquadHeader
        Navigation _nav = squadEditionCanvas.engageButton.navigation;
        _nav.selectOnUp = _squadHeader.select;
        squadEditionCanvas.engageButton.navigation = _nav;

        // Set Up navigation of Soldier1 change button to Selected SquadHeader
        _nav = squadEditionCanvas.soldier1Change.navigation;
        _nav.selectOnUp = _squadHeader.select;
        squadEditionCanvas.soldier1Change.navigation = _nav;

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
    /// ResetSelectedSquad method sets the selected squad to null
    /// </summary>
    private void ResetSelectedSquad()
    {
        selectedSquad = null;
    }

    /// <summary>
    /// UpdateSquadHeaders updates all the SquadHeaders if needed from the PlayManager Squad list
    /// </summary>
    public void UpdateSquadHeaders()
    {
        // if there is more than 0 squad in the list
        if (PlayManager.squadList.Count > 0)
        {
            if(squad1Header.Squad != PlayManager.squadList[0]) // If the header is null or after a game load
            {
                squad1Header.Setup(PlayManager.squadList[0]); // Initialize the header
            }
        }
        else
        {
            squad1Header.Lock();
        }

        if (PlayManager.squadList.Count > 1)
        {
            if (squad2Header.Squad != PlayManager.squadList[1])
            {
                squad2Header.Setup(PlayManager.squadList[1]);
                squad2Header.SetUINav(squad1Header);
            }
        }
        else
        {
            squad2Header.Lock();
        }

        if (PlayManager.squadList.Count > 2)
        {
            if (squad3Header.Squad != PlayManager.squadList[2])
            {
                squad3Header.Setup(PlayManager.squadList[2]);
                squad3Header.SetUINav(squad2Header);
            }
        }
        else
        {
            squad3Header.Lock();
        }

        if (PlayManager.squadList.Count > 3)
        {
            if (squad4Header.Squad != PlayManager.squadList[3])
            {
                squad4Header.Setup(PlayManager.squadList[3]);
                squad4Header.SetUINav(squad3Header);
            }
        }
        else
        {
            squad4Header.Lock();
        }

        // If no squad is selected, select the first squad
        if (selectedSquad == null) selectedSquad = PlayManager.squadList[0];
        // Display the selected squad in the Edition Canvas
        if (selectedSquad != null) squadEditionCanvas.DisplaySquad(selectedSquad);
    }    
}
