using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemorialCanvas : UICanvas
{
    // public UI elements
    public RectTransform content;
    public GameObject memorialItemPrefab;

    public delegate void MemorialCanvasEventHandler(bool _b);
    public event MemorialCanvasEventHandler OnDisplayDead;

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
    /// Setup method is used to clear the previous items and creates new ones for each dead soldier
    /// </summary>
    public void Setup()
    {
        bool _mourning = false;
        // Remove the previous items
        foreach (Transform _child in content.transform)
        {
            Destroy(_child.gameObject);
        }

        // Create and setup new tower items
        foreach (Soldier _s in PlayManager.soldierList)
        {
            if (_s.IsDead)
            {
                GameObject _go = Instantiate(memorialItemPrefab, content);
                _mourning = _mourning || _go.GetComponent<DeadSoldierItem>().Setup(_s);
            }
        }

        OnDisplayDead?.Invoke(_mourning);
    }
}
