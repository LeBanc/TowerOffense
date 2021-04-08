using UnityEngine;

/// <summary>
/// HQLevelupCanvas class manages the Level Up Canvas
/// </summary>
public class HQLevelupCanvas : UICanvas
{
    // public UI elements
    public LevelUpItem item1;
    public LevelUpItem item2;
    public LevelUpItem item3;

    // private data
    private Soldier soldier;
    private SoldierData selectedData;

    // Events
    public delegate void LevelUPEventHandler();
    public event LevelUPEventHandler OnLevelUp;

    /// <summary>
    /// On Awake, fetches Canvas and subscribe to events
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (item1 != null) item1.OnSelection += SelectClass1;
        if (item2 != null) item2.OnSelection += SelectClass2;
        if (item3 != null) item3.OnSelection += SelectClass3;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        if (item1 != null) item1.OnSelection -= SelectClass1;
        if (item2 != null) item2.OnSelection -= SelectClass2;
        if (item3 != null) item3.OnSelection -= SelectClass3;
    }

    /// <summary>
    /// Show methods initializes the Canvas and its elements
    /// </summary>
    /// <param name="_soldier"></param>
    public void Show(Soldier _soldier)
    {
        if(_soldier.Data.improveTo.Count == 3) // If there are 3 possible evolutions
        {
            soldier = _soldier;
            item1.gameObject.SetActive(true);
            item1.Setup(soldier, soldier.Data.improveTo[0]);
            item2.gameObject.SetActive(true);
            item2.Setup(soldier, soldier.Data.improveTo[1]);
            item3.gameObject.SetActive(true);
            item3.Setup(soldier, soldier.Data.improveTo[2]);

            Show();
        }
        else if (_soldier.Data.improveTo.Count == 2) // If there are 2 possible evolutions
        {
            soldier = _soldier;
            item1.gameObject.SetActive(true);
            item1.Setup(soldier, soldier.Data.improveTo[0]);
            item2.gameObject.SetActive(false);
            item3.gameObject.SetActive(true);
            item3.Setup(soldier, soldier.Data.improveTo[1]);

            Show();
        }
        else if (_soldier.Data.improveTo.Count == 1) // If only one possible evolution
        {
            soldier = _soldier;
            item1.gameObject.SetActive(false);
            item2.gameObject.SetActive(true);
            item2.Setup(soldier, soldier.Data.improveTo[0]);
            item3.gameObject.SetActive(false);

            Show();
        }
        else
        {
            Debug.LogError("[HQLevelupCanvas] Trying to open the LevelUp Canvas for a soldier with no possible evolution");
        }
        if(item1.isActiveAndEnabled)
        {
            item1.SelectButton();
        }
        else
        {
            item2.SelectButton();
        }
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public override void Hide()
    {
        // reset the selected data
        selectedData = null;

        // Hide the canvas
        base.Hide();
    }

    /// <summary>
    /// Validate method applies the selected changes and hides the canvas
    /// </summary>
    public void Validate()
    {
        if(selectedData != null)
        {
            soldier.Evolve(selectedData);
            OnLevelUp?.Invoke();
        }
        Hide();
    }

    /// <summary>
    /// SelectClass1 method is used through button to select the 1st class to evolve into
    /// </summary>
    public void SelectClass1()
    {
        selectedData = soldier.Data.improveTo[0];
        item2.Unselect();
        item3.Unselect();
    }

    /// <summary>
    /// SelectClass2 method is used through button to select the 2nd class to evolve into
    /// </summary>
    public void SelectClass2()
    {
        selectedData = soldier.Data.improveTo[(soldier.Data.improveTo.Count>1)?1:0]; //2 possibilities depending of 3 or only 1 evolutions
        item1.Unselect();
        item3.Unselect();
    }

    /// <summary>
    /// SelectClass3 method is used through button to select the 3rd class to evolve into
    /// </summary>
    public void SelectClass3()
    {
        selectedData = soldier.Data.improveTo[(soldier.Data.improveTo.Count > 2) ? 2 : 1]; //2 possibilities depending of 3 or only 2 evolutions
        item1.Unselect();
        item2.Unselect();
    }
}
