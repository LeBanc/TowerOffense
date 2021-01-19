using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NewSoldierCanvas class defines the UI Canvas that appears when a new soldier is available
/// </summary>
public class NewSoldierCanvas : UICanvas
{
    public Text soldierNameText;
    public Image soldierAvatar;
    public Button validateButton;

    protected override void Awake()
    {
        base.Awake();

        PlayManager.OnRecruit += Show;
    }

    private void OnDestroy()
    {
        PlayManager.OnRecruit -= Show;
    }

    public override void Show()
    {
        soldierNameText.text = PlayManager.GetRandomSoldierName();
        soldierAvatar.sprite = PlayManager.GetRandomSoldierImage();

        base.Show();

        validateButton.Select();
    }

    public void Recruit()
    {
        Soldier _soldier = ScriptableObject.CreateInstance("Soldier") as Soldier;
        _soldier.InitData(Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData);
        _soldier.ChangeName(soldierNameText.text);
        _soldier.ChangeImage(soldierAvatar.sprite);
        PlayManager.soldierList.Add(_soldier);
        PlayManager.soldierIDList.Add(_soldier.ID);
        Hide();
    }
}
