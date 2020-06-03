using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// CityCanvas class manages the CityCanvas overall functions
/// </summary>
public class CityCanvas : MonoBehaviour
{
    // Public Squad Action Panels
    public SquadActionPanel squad1;
    public SquadActionPanel squad2;
    public SquadActionPanel squad3;
    public SquadActionPanel squad4;
    
    // Public Slow motion effect image
    public Image slowMotionEffect;

    // Selected squad
    private SquadActionPanel selectedSquad = null;
    private int lastSelected = 0;

    // HealthBars (over Units and Towers)
    public GameObject healthBar;
    private List<HealthBar> hBarList;

    // Events
    public delegate void CityCanvasEventHandler(bool isSelected);
    public event CityCanvasEventHandler OnSquad1Selection;
    public event CityCanvasEventHandler OnSquad2Selection;
    public event CityCanvasEventHandler OnSquad3Selection;
    public event CityCanvasEventHandler OnSquad4Selection;

    // Raycasting elements
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    /// <summary>
    /// On Awake, disables the SquadActionPanels and initializes HelthBar list
    /// </summary>
    void Awake()
    {
        squad1.enabled = false;
        squad2.enabled = false;
        squad3.enabled = false;
        squad4.enabled = false;

        hBarList = new List<HealthBar>();
    }

    /// <summary>
    /// On Start, sets the raycast elements and hides the graphical elements
    /// </summary>
    private void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        squad1.gameObject.SetActive(false);
        squad2.gameObject.SetActive(false);
        squad3.gameObject.SetActive(false);
        squad4.gameObject.SetActive(false);

        slowMotionEffect.CrossFadeAlpha(0f, 0f, true);

        enabled = false;
    }

    /// <summary>
    /// OnDestroy, unlinks events and clears the HealthBar list
    /// </summary>
    private void OnDestroy()
    {
        // Unlink update events
        GameManager.PlayUpdate -= UIUpdate;
        foreach(HealthBar _hb in hBarList)
        {
            _hb.OnRemove -= RemoveHealthBar;
        }
        hBarList.Clear();
    }

    /// <summary>
    /// AddHelthBar adds a HealthBar to the UI
    /// </summary>
    /// <param name="_t">Transform over which the HealthBar will be displayed</param>
    /// <param name="_maxW">Max width of the HealthBar</param>
    /// <returns></returns>
    public HealthBar AddHealthBar(Transform _t, float _maxW)
    {
        // Instantiate the new HealthBar as a chils of the CityCanvas
        GameObject _go = Instantiate(healthBar, transform);
        // Initialize, setup and draw the HealthBar
        HealthBar _hb = _go.GetComponent<HealthBar>();
        _hb.Setup(_t, _maxW);
        _hb.UpdatePosition();
        // Add the HealthBar to the HealthBar list and subscribe to Remove event
        hBarList.Add(_hb);
        _hb.OnRemove += RemoveHealthBar;
        return _hb;
    }

    /// <summary>
    /// RemoveHealthBar removes the HealthBar from the UI and its list
    /// </summary>
    /// <param name="_hb"></param>
    private void RemoveHealthBar(HealthBar _hb)
    {
        hBarList.Remove(_hb);
        _hb.OnRemove -= RemoveHealthBar;
    }

    #region Squad
    /// <summary>
    /// AddSquad links a SquadUnit to a SquadActionPanel and shows it on UI
    /// </summary>
    /// <param name="_unit">SquadUnit to link</param>
    public void AddSquad(SquadUnit _unit)
    {
        // If the first SquadActionPanel isn't active, use it
        if (!squad1.gameObject.activeSelf)
        {
            // Setup the SquadActionPanel with SquadUnit data
            squad1.Setup(_unit);

            // Subscribe all needed SquadActionPanel events
            OnSquad1Selection += SelectSquad1;
            OnSquad1Selection += squad1.SelectSquad;
            OnSquad1Selection += ShowMotionEffect;
            OnSquad1Selection += PlayManager.SlowSpeed;
            OnSquad1Selection?.Invoke(false);

            // Subscribe all needed SquadUnit events
            _unit.Unselect += UnselectSquads;
            _unit.Unselect += HideMotionEffect;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        // If the first SquadActionPanel is already active but the second isn't, use it
        else if (!squad2.gameObject.activeSelf)
        {
            squad2.Setup(_unit);

            OnSquad2Selection += SelectSquad2;
            OnSquad2Selection += squad2.SelectSquad;
            OnSquad2Selection += ShowMotionEffect;
            OnSquad2Selection += PlayManager.SlowSpeed;
            OnSquad2Selection?.Invoke(false);

            _unit.Unselect += UnselectSquads;
            _unit.Unselect += HideMotionEffect;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        // If the two first SquadActionPanel are already active but the third isn't, use it
        else if (!squad3.gameObject.activeSelf)
        {
            squad3.Setup(_unit);

            //SelectSquad3 += _unit.Select;
            OnSquad3Selection += SelectSquad3;
            OnSquad3Selection += squad3.SelectSquad;
            OnSquad3Selection += ShowMotionEffect;
            OnSquad3Selection += PlayManager.SlowSpeed;
            OnSquad3Selection?.Invoke(false);

            _unit.Unselect += UnselectSquads;
            _unit.Unselect += HideMotionEffect;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        // If the three first SquadActionPanel are already active but the fourth isn't, use it
        else if (!squad4.gameObject.activeSelf)
        {
            squad4.Setup(_unit);

            //SelectSquad4 += _unit.Select;
            OnSquad4Selection += SelectSquad4;
            OnSquad4Selection += squad4.SelectSquad;
            OnSquad4Selection += ShowMotionEffect;
            OnSquad4Selection += PlayManager.SlowSpeed;
            OnSquad4Selection?.Invoke(false);

            _unit.Unselect += UnselectSquads;
            _unit.Unselect += HideMotionEffect;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        // If the four SqaudActionPanel are active and a fifth is required, ther is an error
        else
        {
            Debug.LogError("[CityCanvas] Trying to instantiate a 5th squad.");
        }
    }

    /// <summary>
    /// SelectSquad1 is used to select the first SquadSelectionPanel
    /// </summary>
    /// <param name="_b">Selection boolean</param>
    private void SelectSquad1(bool _b)
    {
        // If selection is request, selects the SquadSelectionPanel
        if (_b)
        {
            selectedSquad = squad1;
            lastSelected = 1;
        }
        // Else clear the selection
        else
        {
            selectedSquad = null;
        }
    }

    /// <summary>
    /// SelectSquad2 is used to select the second SquadSelectionPanel
    /// </summary>
    /// <param name="_b">Selection boolean</param>
    private void SelectSquad2(bool _b)
    {
        if (_b)
        {
            selectedSquad = squad2;
            lastSelected = 2;
        }
        else
        {
            selectedSquad = null;
        }
    }

    /// <summary>
    /// SelectSquad3 is used to select the third SquadSelectionPanel
    /// </summary>
    /// <param name="_b">Selection boolean</param>
    private void SelectSquad3(bool _b)
    {
        if (_b)
        {
            selectedSquad = squad3;
            lastSelected = 3;
        }
        else
        {
            selectedSquad = null;
        }
    }

    /// <summary>
    /// SelectSquad4 is used to select the fouth SquadSelectionPanel
    /// </summary>
    /// <param name="_b">Selection boolean</param>
    private void SelectSquad4(bool _b)
    {
        if (_b)
        {
            selectedSquad = squad4;
            lastSelected = 4;
        }
        else
        {
            selectedSquad = null;
        }
    }

    /// <summary>
    /// UnselectSquads unselects all 4 SquadSelectionPanels
    /// </summary>
    public void UnselectSquads()
    {
        OnSquad1Selection?.Invoke(false);
        OnSquad2Selection?.Invoke(false);
        OnSquad3Selection?.Invoke(false);
        OnSquad4Selection?.Invoke(false);
    }
    #endregion

    /// <summary>
    /// ShowMotionEffect enables the slow motion effect with a fade in
    /// </summary>
    /// <param name="isSelected">Boolean to enable or not the effect</param>
    private void ShowMotionEffect(bool isSelected)
    {
        // Show motion effect only if the squad is selected and not unselected
        if(isSelected) slowMotionEffect.CrossFadeAlpha(0.6f, 0.2f, true);
    }

    /// <summary>
    /// HideMotionEffect hides the slow motion effect with a fade out
    /// </summary>
    private void HideMotionEffect()
    {
        slowMotionEffect.CrossFadeAlpha(0f, 0.2f, true);
    }

    /// <summary>
    /// Show method shows the CityCanvas
    /// </summary>
    public void Show()
    {
        // Enable Canvas and set it as last sibling to unsure its visibility
        GetComponent<Canvas>().enabled = true;
        transform.SetAsLastSibling();

        // Link update events
        GameManager.PlayUpdate += UIUpdate;

        // Show set (active) SquadActionPanels
        if (squad1.isSet) squad1.gameObject.SetActive(true);
        if (squad2.isSet) squad2.gameObject.SetActive(true);
        if (squad3.isSet) squad3.gameObject.SetActive(true);
        if (squad4.isSet) squad4.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide method hides the CityCanvas
    /// </summary>
    public void Hide()
    {
        // Unlink update events
        GameManager.PlayUpdate -= UIUpdate;

        // Hide all SquadActionPanels
        squad1.gameObject.SetActive(false);
        squad2.gameObject.SetActive(false);
        squad3.gameObject.SetActive(false);
        squad4.gameObject.SetActive(false);

        // Set Canvas as first sibling and disable it to unsure its invisibility
        transform.SetAsFirstSibling();
        GetComponent<Canvas>().enabled = false;

    }

    /// <summary>
    /// Reset method clears the SquadActionPanels
    /// </summary>
    public void Reset()
    {
        // Clear all private parameters
        squad1.Reset();
        squad2.Reset();
        squad3.Reset();
        squad4.Reset();
        OnSquad1Selection = null;
        OnSquad2Selection = null;
        OnSquad3Selection = null;
        OnSquad4Selection = null;
    }

    /// <summary>
    /// UIUpdate is the Update method of the CityCanvas
    /// </summary>
    private void UIUpdate()
    {
        // Selection of SquadActionPanel by mouse (Graphic raycast)
        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                // If the object is a SquadSelectionPanel
                if(result.gameObject == squad1.gameObject || result.gameObject == squad2.gameObject ||
                    result.gameObject == squad3.gameObject || result.gameObject == squad4.gameObject)
                {
                    // Select the squad only if it is not already selected
                    if(selectedSquad == null)
                    {
                        OnSquad1Selection?.Invoke(result.gameObject == squad1.gameObject);
                        OnSquad2Selection?.Invoke(result.gameObject == squad2.gameObject);
                        OnSquad3Selection?.Invoke(result.gameObject == squad3.gameObject);
                        OnSquad4Selection?.Invoke(result.gameObject == squad4.gameObject);
                    }
                    else if(result.gameObject != selectedSquad.gameObject)
                    {
                        OnSquad1Selection?.Invoke(result.gameObject == squad1.gameObject);
                        OnSquad2Selection?.Invoke(result.gameObject == squad2.gameObject);
                        OnSquad3Selection?.Invoke(result.gameObject == squad3.gameObject);
                        OnSquad4Selection?.Invoke(result.gameObject == squad4.gameObject);
                    }
                }
            }
        }
        // Selection of SquadActionPanel by Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // If no Squad was selected in the whole attack, select first Squad
            if (lastSelected == 0)
            {
                OnSquad1Selection.Invoke(true);
            }
            // Else, active the last selected squad or change selection
            else
            {
                switch (lastSelected)
                {
                    // If the last selected squad was the first
                    case 1:
                        // If the selection is empty, select the first squad
                        if(selectedSquad == null && squad1.gameObject.activeSelf)
                        {
                            OnSquad1Selection?.Invoke(true);
                        }
                        // else (ie the first squad is selected), select the next active squad
                        else
                        {
                            if (squad2.gameObject.activeSelf)
                            {
                                OnSquad1Selection?.Invoke(false);
                                OnSquad2Selection?.Invoke(true);
                            }
                            else if (squad3.gameObject.activeSelf)
                            {
                                OnSquad1Selection?.Invoke(false);
                                OnSquad3Selection?.Invoke(true);
                            }
                            else if (squad4.gameObject.activeSelf)
                            {
                                OnSquad1Selection?.Invoke(false);
                                OnSquad4Selection?.Invoke(true);
                            }
                        }
                        break;
                    case 2:
                        if (selectedSquad == null && squad2.gameObject.activeSelf)
                        {
                            OnSquad2Selection?.Invoke(true);
                        }
                        else
                        {
                            if (squad3.gameObject.activeSelf)
                            {
                                OnSquad2Selection?.Invoke(false);
                                OnSquad3Selection?.Invoke(true);
                            }
                            else if (squad4.gameObject.activeSelf)
                            {
                                OnSquad2Selection?.Invoke(false);
                                OnSquad4Selection?.Invoke(true);
                            }
                            else if (squad1.gameObject.activeSelf)
                            {
                                OnSquad2Selection?.Invoke(false);
                                OnSquad1Selection?.Invoke(true);
                            }
                        }
                        break;
                    case 3:
                        if (selectedSquad == null && squad3.gameObject.activeSelf)
                        {
                            OnSquad3Selection?.Invoke(true);
                        }
                        else
                        {
                            if (squad4.gameObject.activeSelf)
                            {
                                OnSquad3Selection?.Invoke(false);
                                OnSquad4Selection?.Invoke(true);
                            }
                            else if (squad1.gameObject.activeSelf)
                            {
                                OnSquad3Selection?.Invoke(false);
                                OnSquad1Selection?.Invoke(true);
                            }
                            else if (squad2.gameObject.activeSelf)
                            {
                                OnSquad3Selection?.Invoke(false);
                                OnSquad2Selection?.Invoke(true);
                            }
                        }
                        break;
                    case 4:
                        if (selectedSquad == null && squad4.gameObject.activeSelf)
                        {
                            OnSquad4Selection?.Invoke(true);
                        }
                        else
                        {
                            if (squad1.gameObject.activeSelf)
                            {
                                OnSquad4Selection?.Invoke(false);
                                OnSquad1Selection?.Invoke(true);
                            }
                            else if (squad2.gameObject.activeSelf)
                            {
                                OnSquad4Selection?.Invoke(false);
                                OnSquad2Selection?.Invoke(true);
                            }
                            else if (squad3.gameObject.activeSelf)
                            {
                                OnSquad4Selection?.Invoke(false);
                                OnSquad3Selection?.Invoke(true);
                            }
                        }
                        break;
                }
            }
        }
        // Camera movment with arrow keys (up and down)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Move Camera up and update HealthBars
            Camera.main.transform.position += new Vector3(0f, 0f, 1f);
            foreach(HealthBar _hb in hBarList)
            {
                _hb.UpdatePosition();
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // Move Camera down and update HealthBars
            Camera.main.transform.position += new Vector3(0f, 0f, -1f);
            foreach (HealthBar _hb in hBarList)
            {
                _hb.UpdatePosition();
            }
        }
    }
}
