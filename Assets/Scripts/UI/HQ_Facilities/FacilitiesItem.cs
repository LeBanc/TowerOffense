using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FacilitiesItem class defines an item of Facilities UI
/// </summary>
public class FacilitiesItem : MonoBehaviour
{
    // public UI elements
    public Button backgroundButton;
    public Image lockImage;
    public Text title;
    public Text effect;
    public Text cost;

    // Prefab sprites
    public Sprite backgroundSprite;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    // Colors
    public Color unlockedColor;
    public Color availableColor;
    public Color hiddenColor;

    // Next FacilitiesItem (to set it available when unlocking)
    private FacilitiesItem nextItem;

    // private properties
    int costAmount;
    bool canBeUnlocked;

    // Events
    public delegate void FacilitiesItemEventHandler(FacilitiesItem _item);
    public event FacilitiesItemEventHandler OnActivation;

    public int Cost
    {
        get { return costAmount; }
    }

    public FacilitiesItem NextItem
    {
        get { return nextItem; }
    }

    /// <summary>
    /// On Start, setup background and lock images
    /// </summary>
    private void Start()
    {
        if (backgroundSprite != null) backgroundButton.image.sprite = backgroundSprite;
        lockImage.sprite = lockedSprite;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        backgroundButton.onClick.RemoveListener(Activate);
    }

    /// <summary>
    /// Setup method setups the item
    /// </summary>
    /// <param name="_title">Title of the item (string)</param>
    /// <param name="_effect">Effect of the item (string)</param>
    /// <param name="_cost">Cost of the item (int)</param>
    /// <param name="_lockState">State of the item: true=locked, false=unlocked</param>
    public void Setup(string _title, string _effect, int _cost, bool _lockState, bool _canUnlock, FacilitiesItem _next = null)
    {
        title.text = _title;
        effect.text = _effect;
        costAmount = _cost;
        cost.text = costAmount.ToString();

        nextItem = _next;

        // Set availability before lock state to have the right background color
        SetAvailable(_canUnlock);
        Lock(_lockState);        
    }

    /// <summary>
    /// Activate method calls event when the button is activated
    /// </summary>
    public void Activate()
    {
        if(canBeUnlocked)
        {
            OnActivation?.Invoke(this);
        }        
    }

    /// <summary>
    /// Lock method locks or unlocks the item
    /// </summary>
    /// <param name="_lockState">State of the item (bool): true=locked, false=unlocked</param>
    public void Lock(bool _lockState)
    {
        // Start with event removal to be sure to remove all listener for a previous game (when loading, starting new game, etc. in a same session)
        backgroundButton.onClick.RemoveListener(Activate);

        if (_lockState)
        {
            lockImage.sprite = lockedSprite;
            backgroundButton.onClick.AddListener(Activate);
        }
        else
        {
            lockImage.sprite = unlockedSprite;
            backgroundButton.image.color = unlockedColor;
        }
    }

    /// <summary>
    /// SetAvailable method change the background color of the item to display it as available or not
    /// </summary>
    /// <param name="_available">True if the item should be available, false otherwise (boolean)</param>
    public void SetAvailable(bool _available)
    {
        canBeUnlocked = _available;
        if(_available)
        {
            backgroundButton.image.color = availableColor;
        }
        else
        {
            backgroundButton.image.color = hiddenColor;
        }
    }

    /// <summary>
    /// Select method selects the background button (for UI navigation)
    /// </summary>
    public void Select()
    {
        backgroundButton.Select();
    }
}
