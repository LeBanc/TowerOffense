using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// CityCanvas class manages the CityCanvas overall functions
/// </summary>
public class CityCanvas : UICanvas
{
    // Public Squad Action Panels
    public SquadActionPanel squad1;
    public SquadActionPanel squad2;
    public SquadActionPanel squad3;
    public SquadActionPanel squad4;
    public Button retreatButton;
    
    // Public Slow motion effect image
    public Image slowMotionEffect;

    public GameObject[] hideOnRetrat;

    // Selected squad
    private SquadActionPanel selectedSquad = null;
    private int lastSelected = 0;

    // HealthBars (over Units and Towers)
    public GameObject healthBar;
    private List<HealthBar> hBarList;

    // Active camera
    private CameraMovement activeCamMove;

    // Events
    public delegate void CityCanvasEventHandler();
    public event CityCanvasEventHandler OnSquad1Selection;
    public event CityCanvasEventHandler OnSquad2Selection;
    public event CityCanvasEventHandler OnSquad3Selection;
    public event CityCanvasEventHandler OnSquad4Selection;

    /// <summary>
    /// On Awake, disables the SquadActionPanels and initializes HelthBar list
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        squad1.enabled = false;
        squad2.enabled = false;
        squad3.enabled = false;
        squad4.enabled = false;

        hBarList = new List<HealthBar>();

        PlayManager.OnNewHealthBarAdded += AddHealthBar;
        PlayManager.OnReset += Reset;
    }

    /// <summary>
    /// On Start, sets the raycast elements, hides the graphical elements and subscribes to events
    /// </summary>
    private void Start()
    {
        squad1.gameObject.SetActive(false);
        squad1.OnRetreat += HideRetreatButton;
        squad2.gameObject.SetActive(false);
        squad2.OnRetreat += HideRetreatButton;
        squad3.gameObject.SetActive(false);
        squad3.OnRetreat += HideRetreatButton;
        squad4.gameObject.SetActive(false);
        squad4.OnRetreat += HideRetreatButton;
        retreatButton.gameObject.SetActive(false);

        foreach (GameObject _go in hideOnRetrat) _go.SetActive(false);

        slowMotionEffect.CrossFadeAlpha(0f, 0f, true);

        PlayManager.OnLoadSquadsOnNewDay += Show;
        PlayManager.OnHQPhase += Hide;
        PlayManager.OnHQPhase += Reset;
        PlayManager.OnLoadGame += FetchCamera;
        //enabled = false;
    }

    /// <summary>
    /// OnDestroy, unlinks events and clears the HealthBar list
    /// </summary>
    private void OnDestroy()
    {
        // Unlink update events
        GameManager.PlayUpdate -= CityCanvasUpdate;
        PlayManager.OnLoadSquadsOnNewDay -= Show;
        PlayManager.OnHQPhase -= Hide;
        PlayManager.OnHQPhase -= Reset;
        PlayManager.OnNewHealthBarAdded -= AddHealthBar;
        PlayManager.OnReset -= Reset;
        PlayManager.OnLoadGame -= FetchCamera;

        squad1.OnRetreat -= HideRetreatButton;
        squad2.OnRetreat -= HideRetreatButton;
        squad3.OnRetreat -= HideRetreatButton;
        squad4.OnRetreat -= HideRetreatButton;

        // Remove HealthBars
        foreach (HealthBar _hb in hBarList)
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

    /// <summary>
    /// UpdateAllHealthBars updates the position of all healthbars
    /// </summary>
    private void UpdateAllHealthBars()
    {
        foreach(HealthBar _hb in hBarList)
        {
            _hb.UpdatePosition();
        }
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
            squad1.Setup(_unit, slowMotionEffect);

            // Subscribe all needed SquadActionPanel events
            squad1.OnSelectionRequest += SelectSquad1;
            OnSquad1Selection += delegate { squad1.SelectSquad(true); } ;
            OnSquad1Selection += ShowMotionEffect;
            OnSquad1Selection += PlayManager.SlowSpeed;

            // Subscribe all needed SquadUnit events
            _unit.OnUnselection += UnselectSquad1;
            _unit.OnUnselection += HideMotionEffect;
            _unit.OnUnselection += PlayManager.NormalSpeed;
        }
        // If the first SquadActionPanel is already active but the second isn't, use it
        else if (!squad2.gameObject.activeSelf)
        {
            squad2.Setup(_unit, slowMotionEffect);

            squad2.OnSelectionRequest += SelectSquad2;
            OnSquad2Selection += delegate { squad2.SelectSquad(true); };
            OnSquad2Selection += ShowMotionEffect;
            OnSquad2Selection += PlayManager.SlowSpeed;

            _unit.OnUnselection += UnselectSquad2;
            _unit.OnUnselection += HideMotionEffect;
            _unit.OnUnselection += PlayManager.NormalSpeed;
        }
        // If the two first SquadActionPanel are already active but the third isn't, use it
        else if (!squad3.gameObject.activeSelf)
        {
            squad3.Setup(_unit, slowMotionEffect);

            squad3.OnSelectionRequest += SelectSquad3;
            OnSquad3Selection += delegate { squad3.SelectSquad(true); };
            OnSquad3Selection += ShowMotionEffect;
            OnSquad3Selection += PlayManager.SlowSpeed;

            _unit.OnUnselection += UnselectSquad3;
            _unit.OnUnselection += HideMotionEffect;
            _unit.OnUnselection += PlayManager.NormalSpeed;
        }
        // If the three first SquadActionPanel are already active but the fourth isn't, use it
        else if (!squad4.gameObject.activeSelf)
        {
            squad4.Setup(_unit, slowMotionEffect);

            //SelectSquad4 += _unit.Select;
            squad4.OnSelectionRequest += SelectSquad4;
            OnSquad4Selection += delegate { squad4.SelectSquad(true); };
            OnSquad4Selection += ShowMotionEffect;
            OnSquad4Selection += PlayManager.SlowSpeed;
            
            _unit.OnUnselection += UnselectSquad4;
            _unit.OnUnselection += HideMotionEffect;
            _unit.OnUnselection += PlayManager.NormalSpeed;
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
    private void SelectSquad1()
    {
        if (selectedSquad == squad1) return;
        if(selectedSquad != null) selectedSquad.SquadUnit.Unselect();
        selectedSquad = squad1;
        lastSelected = 1;
        OnSquad1Selection?.Invoke();
    }

    /// <summary>
    /// SelectSquad2 is used to select the second SquadSelectionPanel
    /// </summary>
    private void SelectSquad2()
    {
        if (selectedSquad == squad2) return;
        if (selectedSquad != null) selectedSquad.SquadUnit.Unselect();
        selectedSquad = squad2;
        lastSelected = 2;
        OnSquad2Selection?.Invoke();
    }

    /// <summary>
    /// SelectSquad3 is used to select the third SquadSelectionPanel
    /// </summary>
    private void SelectSquad3()
    {
        if (selectedSquad == squad3) return;
        if (selectedSquad != null) selectedSquad.SquadUnit.Unselect();
        selectedSquad = squad3;
        lastSelected = 3;
        OnSquad3Selection?.Invoke();
    }

    /// <summary>
    /// SelectSquad4 is used to select the fouth SquadSelectionPanel
    /// </summary>
    private void SelectSquad4()
    {
        if (selectedSquad == squad4) return;
        if (selectedSquad != null) selectedSquad.SquadUnit.Unselect();
        selectedSquad = squad4;
        lastSelected = 4;
        OnSquad4Selection?.Invoke();
    }

    /// <summary>
    /// UnselectSquad1 unselects the first SquadSelectionPanel
    /// </summary>
    public void UnselectSquad1()
    {
        squad1.SelectSquad(false);
        if(selectedSquad == squad1) selectedSquad = null;
    }

    /// <summary>
    /// UnselectSquad2 unselects the second SquadSelectionPanel
    /// </summary>
    public void UnselectSquad2()
    {
        squad2.SelectSquad(false);
        if (selectedSquad == squad2) selectedSquad = null;
    }

    /// <summary>
    /// UnselectSquad3 unselects the third SquadSelectionPanel
    /// </summary>
    public void UnselectSquad3()
    {
        squad3.SelectSquad(false);
        if (selectedSquad == squad3) selectedSquad = null;
    }

    /// <summary>
    /// UnselectSquad4 unselects the fourth SquadSelectionPanel
    /// </summary>
    public void UnselectSquad4()
    {
        squad4.SelectSquad(false);
        if (selectedSquad == squad4) selectedSquad = null;
    }
    #endregion

    /// <summary>
    /// ShowMotionEffect enables the slow motion effect with a fade in
    /// </summary>
    private void ShowMotionEffect()
    {
        // Show motion effect only if the squad is selected and not unselected
        slowMotionEffect.CrossFadeAlpha(0.6f, 0.2f, true);
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
    public override void Show()
    {
        base.Show();

        // Link update events
        GameManager.PlayUpdate += CityCanvasUpdate;

        // Show set (active) SquadActionPanels and retreat button
        if (squad1.isSet) squad1.gameObject.SetActive(true);
        if (squad2.isSet) squad2.gameObject.SetActive(true);
        if (squad3.isSet) squad3.gameObject.SetActive(true);
        if (squad4.isSet) squad4.gameObject.SetActive(true);
        retreatButton.gameObject.SetActive(true);
        foreach (GameObject _go in hideOnRetrat) _go.SetActive(true);

        // Clear the selection to avoid clicking on anything not on City UI
        EventSystem.current.SetSelectedGameObject(null);
        UIManager.lastSelected = null;

        // Update position of all healthbars
        UpdateAllHealthBars();
    }

    /// <summary>
    /// HideRetreatButton method checks if all squads are inactive and hides the retreat button
    /// </summary>
    public void HideRetreatButton()
    {
        if(!squad1.gameObject.activeSelf && !squad2.gameObject.activeSelf && !squad3.gameObject.activeSelf && !squad4.gameObject.activeSelf)
        {
            retreatButton.gameObject.SetActive(false);
            foreach (GameObject _go in hideOnRetrat) _go.SetActive(false);
        }
    }

    /// <summary>
    /// Hide method hides the CityCanvas
    /// </summary>
    public override void Hide()
    {
        // Unlink update events
        GameManager.PlayUpdate -= CityCanvasUpdate;

        // Hide all SquadActionPanels and reatreat button
        squad1.gameObject.SetActive(false);
        squad2.gameObject.SetActive(false);
        squad3.gameObject.SetActive(false);
        squad4.gameObject.SetActive(false);
        retreatButton.gameObject.SetActive(false);
        foreach (GameObject _go in hideOnRetrat) _go.SetActive(false);

        // Hide the SlowMo effect (in case of loading while it is active)
        slowMotionEffect.CrossFadeAlpha(0f, 0f, true);

        base.Hide();
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
    /// CityCanvasUpdate is the Update method of the CityCanvas
    /// </summary>
    private void CityCanvasUpdate()
    {        
        // Selection of SquadActionPanel by Right & Left selection buttons
        if (Input.GetButtonDown("RightSelection"))
        {
            // If no Squad was selected in the whole attack, select first Squad
            if (lastSelected == 0)
            {
                SelectSquad1();
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
                            SelectSquad1();
                        }
                        // else (ie the first squad is selected), select the next active squad
                        else
                        {
                            if (squad2.gameObject.activeSelf)
                            {
                                SelectSquad2();
                            }
                            else if (squad3.gameObject.activeSelf)
                            {
                                SelectSquad3();
                            }
                            else if (squad4.gameObject.activeSelf)
                            {
                                SelectSquad4();
                            }
                        }
                        break;
                    case 2:
                        if (selectedSquad == null && squad2.gameObject.activeSelf)
                        {
                            SelectSquad2();
                        }
                        else
                        {
                            if (squad3.gameObject.activeSelf)
                            {
                                SelectSquad3();
                            }
                            else if (squad4.gameObject.activeSelf)
                            {
                                SelectSquad4();
                            }
                            else if (squad1.gameObject.activeSelf)
                            {
                                SelectSquad1();
                            }
                        }
                        break;
                    case 3:
                        if (selectedSquad == null && squad3.gameObject.activeSelf)
                        {
                            SelectSquad3();
                        }
                        else
                        {
                            if (squad4.gameObject.activeSelf)
                            {
                                SelectSquad4();
                            }
                            else if (squad1.gameObject.activeSelf)
                            {
                                SelectSquad1();
                            }
                            else if (squad2.gameObject.activeSelf)
                            {
                                SelectSquad2();
                            }
                        }
                        break;
                    case 4:
                        if (selectedSquad == null && squad4.gameObject.activeSelf)
                        {
                            SelectSquad4();
                        }
                        else
                        {
                            if (squad1.gameObject.activeSelf)
                            {
                                SelectSquad1();
                            }
                            else if (squad2.gameObject.activeSelf)
                            {
                                SelectSquad2();
                            }
                            else if (squad3.gameObject.activeSelf)
                            {
                                SelectSquad3();
                            }
                        }
                        break;
                }
            }
        }
        else if (Input.GetButtonDown("LeftSelection"))
        {
            // If no Squad was selected in the whole attack, select first Squad
            if (lastSelected == 0)
            {
                SelectSquad1();
            }
            // Else, active the last selected squad or change selection
            else
            {
                switch (lastSelected)
                {
                    // If the last selected squad was the first
                    case 1:
                        // If the selection is empty, select the first squad
                        if (selectedSquad == null && squad1.gameObject.activeSelf)
                        {
                            SelectSquad1();
                        }
                        // else (ie the first squad is selected), select the next active squad
                        else
                        {
                            if (squad4.gameObject.activeSelf)
                            {
                                SelectSquad4();
                            }
                            else if (squad3.gameObject.activeSelf)
                            {
                                SelectSquad3();
                            }
                            else if (squad2.gameObject.activeSelf)
                            {
                                SelectSquad2();
                            }
                        }
                        break;
                    case 2:
                        if (selectedSquad == null && squad2.gameObject.activeSelf)
                        {
                            SelectSquad2();
                        }
                        else
                        {
                            if (squad1.gameObject.activeSelf)
                            {
                                SelectSquad1();
                            }
                            else if (squad4.gameObject.activeSelf)
                            {
                                SelectSquad4();
                            }
                            else if (squad3.gameObject.activeSelf)
                            {
                                SelectSquad3();
                            }
                        }
                        break;
                    case 3:
                        if (selectedSquad == null && squad3.gameObject.activeSelf)
                        {
                            SelectSquad3();
                        }
                        else
                        {
                            if (squad2.gameObject.activeSelf)
                            {
                                SelectSquad2();
                            }
                            else if (squad1.gameObject.activeSelf)
                            {
                                SelectSquad1();
                            }
                            else if (squad4.gameObject.activeSelf)
                            {
                                SelectSquad4();
                            }
                        }
                        break;
                    case 4:
                        if (selectedSquad == null && squad4.gameObject.activeSelf)
                        {
                            SelectSquad4();
                        }
                        else
                        {
                            if (squad3.gameObject.activeSelf)
                            {
                                SelectSquad3();
                            }
                            else if (squad2.gameObject.activeSelf)
                            {
                                SelectSquad2();
                            }
                            else if (squad1.gameObject.activeSelf)
                            {
                                SelectSquad1();
                            }
                        }
                        break;
                }
            }

        }

        // Check Inputs for Primary shortcut button => Retreat All
        if (Input.GetButtonDown("PrimaryShortcut"))
        {
            retreatButton.OnSubmit(new BaseEventData(EventSystem.current));
        }

        if (selectedSquad == null) CursorManager.HideCursorAfterAction();
    }

    /// <summary>
    /// FetchCamera method link the HealthBar update to the camera movement
    /// </summary>
    private void FetchCamera()
    {
        if (activeCamMove != null) activeCamMove.OnCameraMovement -= UpdateAllHealthBars;

        activeCamMove = Camera.main.GetComponent<CameraMovement>();

        if(activeCamMove != null)
        {
            activeCamMove.OnCameraMovement += UpdateAllHealthBars;
        }
        else
        {
            Debug.LogError("[CityCanvas] Cannot find main camera CameraMovement script!");
        }
    }
}
