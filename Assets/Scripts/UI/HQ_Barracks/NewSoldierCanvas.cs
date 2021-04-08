using System;
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

    public delegate void NewSoldierCanvasEventHandler();
    public static event NewSoldierCanvasEventHandler OnRecruitWithXP;

    /// <summary>
    /// At Awake, init Canvas and subscribe to event
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        PlayManager.OnRecruit += Show;
    }

    /// <summary>
    /// OnDestroy, unsubscribe events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnRecruit -= Show;
    }

    /// <summary>
    /// Show method generates a new soldier name and avatar and display the Canvas
    /// </summary>
    public override void Show()
    {
        soldierNameText.text = PlayManager.GetRandomSoldierName();
        soldierAvatar.sprite = PlayManager.GetRandomSoldierImage();

        base.Show();

        validateButton.Select();
    }

    /// <summary>
    /// Recruit method add the new soldier to the PlayManager list and hides the Canvas
    /// </summary>
    public void Recruit()
    {
        // Create new soldier with chosen name and avatar
        Soldier _soldier = ScriptableObject.CreateInstance("Soldier") as Soldier;
        _soldier.InitData(Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData);
        _soldier.ChangeName(soldierNameText.text);
        _soldier.ChangeImage(soldierAvatar.sprite);

        // Add soldier to the PlayManager list
        PlayManager.soldierList.Add(_soldier);
        PlayManager.soldierIDList.Add(_soldier.ID);

        // Add XP to soldier if the facility is unlocked
        if (PlayManager.recruitingWithXP > 0)
        {
            _soldier.CurrentXP += _soldier.MaxXP;
            OnRecruitWithXP?.Invoke();
        }       

        // Save and hide the Canvas
        PlayManager.Instance.AutoSaveGame();
        Hide();
    }

    /// <summary>
    /// Dismiss method is used to hide the Canvas when the player click and the "Dismiss" button
    /// It just calls the Hide method after calling an autosave from PlayManager
    /// </summary>
    public void Dismiss()
    {
        // Save and hide the Canvas
        PlayManager.Instance.AutoSaveGame();
        Hide();
    }
}
