using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IntelServiceCanvas class is the Canvas of the Intelligence Services HQ tab
/// </summary>
public class IntelServiceCanvas : UICanvas
{
    // public UI elements
    public RectTransform content;
    public GameObject intelServItemPrefab;

    public delegate void IntelServicesEventHandler(bool _b);
    public event IntelServicesEventHandler OnDisplayTower;

    private List<Tower> seenTowers = new List<Tower>();

    /// <summary>
    /// At Start, subscribe to events
    /// </summary>
    private void Start()
    {
        PlayManager.OnHQPhase += Setup;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnHQPhase -= Setup;
    }

    /// <summary>
    /// Setup method is used to clear the previous items and creates new ones for each active tower on the city field
    /// </summary>
    public void Setup()
    {
        // Remove the previous items
        foreach (Transform _child in content.transform)
        {
            Destroy(_child.gameObject);
        }

        bool _display = false;
        // Create and setup new tower items
        foreach (Tower _t in PlayManager.towerList)
        {
            if(_t.IsActive() && !_t.IsDestroyed())
            {
                if(!seenTowers.Exists(x=> x==_t))
                {
                    seenTowers.Add(_t);
                    _display = true;
                }                
                GameObject _go = Instantiate(intelServItemPrefab, content);
                _go.GetComponent<IntelServTowerItem>().Setup(_t);
            }
        }
        OnDisplayTower?.Invoke(_display);
    }
}
