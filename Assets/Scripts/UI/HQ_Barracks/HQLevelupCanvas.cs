using UnityEngine;

public class HQLevelupCanvas : MonoBehaviour
{
    // public UI elements
    public HQLevelUpItem levelupItem1;
    public HQLevelUpItem levelupItem2;
    public HQLevelUpItem levelupItem3;

    // private canvas
    private Canvas canvas;

    // private data
    private Soldier soldier;
    private SoldierData selectedData;

    // Events
    public delegate void LevelUPEventHandler();
    public event LevelUPEventHandler OnCanvasHide;

    /// <summary>
    /// On Awke, fetches Canvas
    /// </summary>
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
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
            levelupItem1.gameObject.SetActive(true);
            levelupItem1.Setup(soldier, soldier.Data.improveTo[0]);
            levelupItem2.Setup(soldier, soldier.Data.improveTo[1]);
            levelupItem3.gameObject.SetActive(true);
            levelupItem3.Setup(soldier, soldier.Data.improveTo[2]);

            canvas.enabled = true;
            transform.SetAsLastSibling();
        }
        else if (_soldier.Data.improveTo.Count == 1) // If only one possible evolution
        {
            soldier = _soldier;
            levelupItem1.gameObject.SetActive(false);
            levelupItem2.Setup(soldier, soldier.Data.improveTo[0]);
            levelupItem3.gameObject.SetActive(false);

            canvas.enabled = true;
            transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("[HQLevelupCanvas] Trying to open the LevelUp Canvas for a soldier with no possible evolution");
        }
        if(levelupItem1.isActiveAndEnabled)
        {
            levelupItem1.Select();
        }
        else
        {
            levelupItem2.Select();
        }
    }

    /// <summary>
    /// Hide method hides the canvas
    /// </summary>
    public void Hide()
    {
        // reset the selected data
        selectedData = null;

        // Hide the canvas
        canvas.enabled = false;
        transform.SetAsFirstSibling();
        OnCanvasHide?.Invoke();
    }

    /// <summary>
    /// Validate method applies the selected changes and hides the canvas
    /// </summary>
    public void Validate()
    {
        if(selectedData != null)
        {
            soldier.Evolve(selectedData);
        }
        Hide();
    }

    /// <summary>
    /// SelectClass1 method is used through button to select the 1st class to evolve into
    /// </summary>
    public void SelectClass1()
    {
        selectedData = soldier.Data.improveTo[0];
        levelupItem1.selectedBackground.enabled = true;
        levelupItem2.selectedBackground.enabled = false;
        levelupItem3.selectedBackground.enabled = false;
    }

    /// <summary>
    /// SelectClass2 method is used through button to select the 2nd class to evolve into
    /// </summary>
    public void SelectClass2()
    {
        selectedData = soldier.Data.improveTo[(soldier.Data.improveTo.Count>1)?1:0]; //2 possibilities depending of 3 or only 1 evolutions
        levelupItem1.selectedBackground.enabled = false;
        levelupItem2.selectedBackground.enabled = true;
        levelupItem3.selectedBackground.enabled = false;
    }

    /// <summary>
    /// SelectClass3 method is used through button to select the 3rd class to evolve into
    /// </summary>
    public void SelectClass3()
    {
        selectedData = soldier.Data.improveTo[2];
        levelupItem1.selectedBackground.enabled = false;
        levelupItem2.selectedBackground.enabled = false;
        levelupItem3.selectedBackground.enabled = true;
    }

}
