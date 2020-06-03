using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// HQCommandCenter is the class used by the CommandCenter canvas
/// </summary>
public class HQCommandCenter : MonoBehaviour
{
    // Public elements used in the prefab
    public HQSquadHeader squad1Header;
    public HQSquadHeader squad2Header;
    public HQSquadHeader squad3Header;
    public HQSquadHeader squad4Header;

    public HQSquadEdition squadEditionCanvas;

    // Private Squad to save the selected squad
    private Squad selectedSquad;

    // Events
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    /// <summary>
    /// On Start, initialize the event system (for raycast)
    /// </summary>
    private void Start()
    {
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
    }

    /// <summary>
    /// OnEnable, updates the squad headers and subscribes to the PlayUpdate event
    /// </summary>
    void OnEnable()
    {
        UpdateSquadHeaders();
        // Link update events
        GameManager.PlayUpdate += UIUpdate;
    }

    /// <summary>
    /// OnDisable, unsubscribes events
    /// </summary>
    private void OnDisable()
    {
        // Unlink update events
        GameManager.PlayUpdate -= UIUpdate;
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
    /// UpdateSquadHeaders updates all the SquadHeaders from the PlayManager Squad list
    /// </summary>
    public void UpdateSquadHeaders()
    {
        if (PlayManager.squadList.Count > 0)
        {
            squad1Header.Setup(PlayManager.squadList[0]);
            selectedSquad = PlayManager.squadList[0];
        }
        if (PlayManager.squadList.Count > 1) squad2Header.Setup(PlayManager.squadList[1]);
        if (PlayManager.squadList.Count > 2) squad3Header.Setup(PlayManager.squadList[2]);
        if (PlayManager.squadList.Count > 3) squad4Header.Setup(PlayManager.squadList[3]);

        if (selectedSquad != null) squadEditionCanvas.DisplaySquad(selectedSquad);
    }

    /// <summary>
    /// UIUpdate is the Update method of the HQCommandCenter
    /// </summary>
    private void UIUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            EventSystem.current.RaycastAll(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                // Select or unlock the squad on which the player clicked
                if(result.gameObject == squad1Header.background.gameObject)
                {
                    if (PlayManager.nextSquadID > 0)
                    {
                        SelectSquad(PlayManager.squadList[0]);
                    }
                }
                if (result.gameObject == squad2Header.background.gameObject)
                {
                    if (PlayManager.nextSquadID > 1)
                    {
                        SelectSquad(PlayManager.squadList[1]);
                    }
                    else
                    {
                        // Unlock Squad2 to be defined
                    }
                }
                if (result.gameObject == squad3Header.background.gameObject)
                {
                    if (PlayManager.nextSquadID > 2)
                    {
                        SelectSquad(PlayManager.squadList[2]);
                    }
                    else
                    {
                        // Unlock Squad3 to be defined
                    }
                }
                if (result.gameObject == squad4Header.background.gameObject)
                {
                    if (PlayManager.nextSquadID > 3)
                    {
                        SelectSquad(PlayManager.squadList[3]);
                    }
                    else
                    {
                        // Unlock Squad4 to be defined
                    }
                }

            }
        }
    }
}
