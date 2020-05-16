using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro.EditorUtilities;

public class CityCanvas : MonoBehaviour
{
    public SquadActionPanel squad1;
    public SquadActionPanel squad2;
    public SquadActionPanel squad3;
    public SquadActionPanel squad4;
    public Image slowMotionEffect;

    private SquadActionPanel selectedSquad = null;
    private int lastSelected = 0;

    public GameObject healthBar;
    private List<HealthBar> hBarList;

    public delegate void CityCanvasEventHandler(bool isSelected);
    public event CityCanvasEventHandler OnSquad1Selection;
    public event CityCanvasEventHandler OnSquad2Selection;
    public event CityCanvasEventHandler OnSquad3Selection;
    public event CityCanvasEventHandler OnSquad4Selection;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    // Start is called before the first frame update
    void Awake()
    {
        squad1.enabled = false;
        squad2.enabled = false;
        squad3.enabled = false;
        squad4.enabled = false;

        hBarList = new List<HealthBar>();
    }

    private void Start()
    {
        // Link update events
//        GameManager.PlayUpdate += UIUpdate;

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

    public HealthBar AddHealthBar(Transform _t, float _maxW)
    {
        GameObject _go = Instantiate(healthBar, transform);
        HealthBar _hb = _go.GetComponent<HealthBar>();
        _hb.Setup(_t, _maxW);
        _hb.UpdatePosition();
        hBarList.Add(_hb);
        _hb.OnRemove += RemoveHealthBar;
        return _hb;
    }

    private void RemoveHealthBar(HealthBar _hb)
    {
        hBarList.Remove(_hb);
        _hb.OnRemove -= RemoveHealthBar;
    }

    #region Squad
    public void AddSquad(Squad _squad, SquadUnit _unit)
    {
        if (!squad1.gameObject.activeSelf)
        {
            squad1.Setup(_squad, _unit);

            //SelectSquad1 += _unit.Select;
            OnSquad1Selection += SelectSquad1;
            OnSquad1Selection += squad1.SelectSquad;
            OnSquad1Selection += ShowMotionEffect;
            OnSquad1Selection += PlayManager.SlowSpeed;
            OnSquad1Selection?.Invoke(false);

            _unit.Unselect += UnselectSquads;
            _unit.Unselect += HideMotionEffect;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        else if (!squad2.gameObject.activeSelf)
        {
            squad2.Setup(_squad, _unit);

            //SelectSquad2 += _unit.Select;
            OnSquad2Selection += SelectSquad2;
            OnSquad2Selection += squad2.SelectSquad;
            OnSquad2Selection += ShowMotionEffect;
            OnSquad2Selection += PlayManager.SlowSpeed;
            OnSquad2Selection?.Invoke(false);

            _unit.Unselect += UnselectSquads;
            _unit.Unselect += HideMotionEffect;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        else if (!squad3.gameObject.activeSelf)
        {
            squad3.Setup(_squad, _unit);

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
        else if (!squad4.gameObject.activeSelf)
        {
            squad4.Setup(_squad, _unit);

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
        else
        {
            Debug.LogError("[CityCanvas] Trying to instantiate a 5th squad.");
        }
    }

    private void SelectSquad1(bool _b)
    {
        if (_b)
        {
            selectedSquad = squad1;
            lastSelected = 1;
        }
        else
        {
            selectedSquad = null;
        }
    }
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


    public void UnselectSquads()
    {
        OnSquad1Selection?.Invoke(false);
        OnSquad2Selection?.Invoke(false);
        OnSquad3Selection?.Invoke(false);
        OnSquad4Selection?.Invoke(false);
    }
    #endregion

    private void ShowMotionEffect(bool isSelected)
    {
        if(isSelected) slowMotionEffect.CrossFadeAlpha(0.6f, 0.2f, true);
    }

    private void HideMotionEffect()
    {
        slowMotionEffect.CrossFadeAlpha(0f, 0.2f, true);
    }

    private void OnEnable()
    {
        // Link update events
        GameManager.PlayUpdate += UIUpdate;
    }

    private void OnDisable()
    {
        // Unlink update events
        GameManager.PlayUpdate -= UIUpdate;

        // Clear all private parameters
        squad1.gameObject.SetActive(false);
        squad2.gameObject.SetActive(false);
        squad3.gameObject.SetActive(false);
        squad4.gameObject.SetActive(false);
        OnSquad1Selection = null;
        OnSquad2Selection = null;
        OnSquad3Selection = null;
        OnSquad4Selection = null;
    }

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
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                if(result.gameObject == squad1.gameObject || result.gameObject == squad2.gameObject ||
                    result.gameObject == squad3.gameObject || result.gameObject == squad4.gameObject)
                {
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (lastSelected == 0)
            {
                OnSquad1Selection.Invoke(true);
            }
            else
            {
                switch (lastSelected)
                {
                    case 1:
                        if(selectedSquad == null && squad1.gameObject.activeSelf)
                        {
                            OnSquad1Selection?.Invoke(true);
                        }
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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.position += new Vector3(0f, 0f, 1f);
            foreach(HealthBar _hb in hBarList)
            {
                _hb.UpdatePosition();
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.position += new Vector3(0f, 0f, -1f);
            foreach (HealthBar _hb in hBarList)
            {
                _hb.UpdatePosition();
            }
        }
    }
}
