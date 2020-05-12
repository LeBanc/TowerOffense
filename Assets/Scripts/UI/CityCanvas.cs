using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CityCanvas : MonoBehaviour
{
    public Image squad1Sprite;
    public Image squad2Sprite;
    public Image squad3Sprite;
    public Image squad4Sprite;
    public Image slowMotionEffect;

    public GameObject healthBar;

    public delegate void CityCanvasEventHandler(bool isSelected);
    public event CityCanvasEventHandler SelectSquad1;
    public event CityCanvasEventHandler SelectSquad2;
    public event CityCanvasEventHandler SelectSquad3;
    public event CityCanvasEventHandler SelectSquad4;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    // Start is called before the first frame update
    void Awake()
    {
        squad1Sprite.enabled = false;
        squad2Sprite.enabled = false;
        squad3Sprite.enabled = false;
        squad4Sprite.enabled = false;
    }

    private void Start()
    {
        // Link update events
        GameManager.PlayUpdate += UIUpdate;

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        squad1Sprite.color = new Color(1f, 1f, 1f, 0.5f);
        squad2Sprite.color = new Color(1f, 1f, 1f, 0.5f);
        squad3Sprite.color = new Color(1f, 1f, 1f, 0.5f);
        squad4Sprite.color = new Color(1f, 1f, 1f, 0.5f);

        slowMotionEffect.CrossFadeAlpha(0f, 0f, true);
    }

    private void OnDestroy()
    {
        // Unlink update events
        GameManager.PlayUpdate -= UIUpdate;
    }

    public HealthBar AddHealthBar(Transform _t, float _maxW)
    {
        GameObject _go = Instantiate(healthBar, transform);
        HealthBar _hb = _go.GetComponent<HealthBar>();
        _hb.Setup(_t, _maxW);
        _hb.UpdatePosition();
        return _hb;
    }

    #region Squad
    public void AddSquad(Squad _squad, SquadUnit _unit)
    {
        if (!squad1Sprite.enabled)
        {
            squad1Sprite.color = _squad.Color;
            HighlightSquad1(false);
            squad1Sprite.enabled = true;
            SelectSquad1 += _unit.Select;
            SelectSquad1 += HighlightSquad1;
            SelectSquad1 += PlayManager.SlowSpeed;
            _unit.Unselect += UnselectSquads;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        else if (!squad2Sprite.enabled)
        {
            squad2Sprite.color = _squad.Color;
            HighlightSquad2(false);
            squad2Sprite.enabled = true;
            SelectSquad2 += _unit.Select;
            SelectSquad2 += HighlightSquad2;
            SelectSquad2 += PlayManager.SlowSpeed;
            _unit.Unselect += UnselectSquads;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        else if (!squad3Sprite.enabled)
        {
            squad3Sprite.color = _squad.Color;
            HighlightSquad3(false);
            squad3Sprite.enabled = true;
            SelectSquad3 += _unit.Select;
            SelectSquad3 += HighlightSquad3;
            SelectSquad3 += PlayManager.SlowSpeed;
            _unit.Unselect += UnselectSquads;
            _unit.Unselect += PlayManager.NormalSpeed;
        }
        else if (!squad4Sprite.enabled)
        {
            squad4Sprite.color = _squad.Color;
            HighlightSquad4(false);
            squad4Sprite.enabled = true;
            SelectSquad4 += _unit.Select;
            SelectSquad4 += HighlightSquad4;
            SelectSquad4 += PlayManager.SlowSpeed;
            _unit.Unselect += UnselectSquads;
            _unit.Unselect += PlayManager.NormalSpeed;
        } 
        else
        {
            Debug.LogError("[CityCanvas] Trying to instantiate a 5th squad.");
        }
    }

    private void HighlightSquad1(bool isSelected)
    {
        Color _c = squad1Sprite.color;
        _c.a = isSelected ? 1f : 0.5f;
        squad1Sprite.color = _c;
        if(isSelected) ShowMotionEffect();
    }

    private void HighlightSquad2(bool isSelected)
    {
        Color _c = squad2Sprite.color;
        _c.a = isSelected ? 1f : 0.5f;
        squad2Sprite.color = _c;
        if (isSelected) ShowMotionEffect();
    }

    private void HighlightSquad3(bool isSelected)
    {
        Color _c = squad3Sprite.color;
        _c.a = isSelected ? 1f : 0.5f;
        squad3Sprite.color = _c;
        if (isSelected) ShowMotionEffect();
    }

    private void HighlightSquad4(bool isSelected)
    {
        Color _c = squad4Sprite.color;
        _c.a = isSelected ? 1f : 0.5f;
        squad4Sprite.color = _c;
        if (isSelected) ShowMotionEffect();
    }

    public void UnselectSquads()
    {
        SelectSquad1?.Invoke(false);
        SelectSquad2?.Invoke(false);
        SelectSquad3?.Invoke(false);
        SelectSquad4?.Invoke(false);
        HideMotionEffect();
    }
    #endregion

    private void ShowMotionEffect()
    {
        slowMotionEffect.CrossFadeAlpha(0.6f, 0.2f, true);
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
        squad1Sprite.enabled = false;
        squad2Sprite.enabled = false;
        squad3Sprite.enabled = false;
        squad4Sprite.enabled = false;
        SelectSquad1 = null;
        SelectSquad2 = null;
        SelectSquad3 = null;
        SelectSquad4 = null;
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
                SelectSquad1?.Invoke(result.gameObject == squad1Sprite.gameObject);
                SelectSquad2?.Invoke(result.gameObject == squad2Sprite.gameObject);
                SelectSquad3?.Invoke(result.gameObject == squad3Sprite.gameObject);
                SelectSquad4?.Invoke(result.gameObject == squad4Sprite.gameObject);
            }
        }
    }
}
