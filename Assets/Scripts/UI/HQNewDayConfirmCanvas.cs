using UnityEngine.UI;

/// <summary>
/// HQNewDayConfirmCanvas class is used to display a canvas to confirm the new day launch
/// </summary>
public class HQNewDayConfirmCanvas : CancelableUICanvas
{
    // public UI elements
    public HQSquadHeader squad1Header;
    public HQSquadHeader squad2Header;
    public HQSquadHeader squad3Header;
    public HQSquadHeader squad4Header;
    public Button okButton;

    /// <summary>
    /// At Awake, subscribe to events
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        PlayManager.OnNewDayConfirm += Show;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        PlayManager.OnNewDayConfirm -= Show;
        base.OnDestroy();
    }

    /// <summary>
    /// At Start, set the SquadHeader as not interactable and Hide (default)
    /// </summary>
    private void Start()
    {
        squad1Header.select.interactable = false;
        squad2Header.select.interactable = false;
        squad3Header.select.interactable = false;
        squad4Header.select.interactable = false;
        Hide();
    }

    /// <summary>
    /// Show method shows the canvas and setup the squad headers to only display the engaged ones
    /// </summary>
    public override void Show()
    {
        base.Show();

        squad1Header.gameObject.SetActive(false);
        squad2Header.gameObject.SetActive(false);
        squad3Header.gameObject.SetActive(false);
        squad4Header.gameObject.SetActive(false);

        if(PlayManager.squadList.Count>0)
        {
            if(PlayManager.squadList[0].isEngaged)
            {
                squad1Header.gameObject.SetActive(true);
                squad1Header.Setup(PlayManager.squadList[0]);
            }
        }

        if (PlayManager.squadList.Count > 1)
        {
            if (PlayManager.squadList[1].isEngaged)
            {
                squad2Header.gameObject.SetActive(true);
                squad2Header.Setup(PlayManager.squadList[1]);
            }
        }

        if (PlayManager.squadList.Count > 2)
        {
            if (PlayManager.squadList[2].isEngaged)
            {
                squad3Header.gameObject.SetActive(true);
                squad3Header.Setup(PlayManager.squadList[2]);
            }
        }

        if (PlayManager.squadList.Count > 3)
        {
            if (PlayManager.squadList[3].isEngaged)
            {
                squad4Header.gameObject.SetActive(true);
                squad4Header.Setup(PlayManager.squadList[3]);
            }
        }

        // OK button is selected by default
        okButton.Select();
    }

    /// <summary>
    /// Confirm method launches the New day routine (in PlayManager)
    /// </summary>
    public void Confirm()
    {
        PlayManager.NewDayLaunchAttack();
        Hide();
    }
}
