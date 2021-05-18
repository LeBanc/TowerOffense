using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HQFacilities : UICanvas
{
    // public UI elements
    public FacilitiesItem squad2Item;
    public FacilitiesItem squad3Item;
    public FacilitiesItem squad4Item;

    public FacilitiesItem attackTime1Item;
    public FacilitiesItem attackTime2Item;
    public FacilitiesItem attackTime3Item;

    public FacilitiesItem healAmount1Item;
    public FacilitiesItem healAmount2Item;
    public FacilitiesItem healAmount3Item;

    public FacilitiesItem recruitingChance1Item;
    public FacilitiesItem recruitingChance2Item;
    public FacilitiesItem recruitingChance3Item;

    public FacilitiesItem explosiveDamages1Item;
    public FacilitiesItem explosiveDamages2Item;
    public FacilitiesItem explosiveDamages3Item;

    public FacilitiesItem recruitingWithXPItem;

    // Prefab sprites for locked and unlocked states
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    // Event
    public delegate void FacilitiesEventHandler();
    public static event FacilitiesEventHandler OnNewSquad;

    /// <summary>
    /// On Awake, subscribe to events
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        squad2Item.OnActivation += UnlockSquad;
        squad3Item.OnActivation += UnlockSquad;
        squad4Item.OnActivation += UnlockSquad;

        attackTime1Item.OnActivation += AttackTimeEnhance;
        attackTime2Item.OnActivation += AttackTimeEnhance;
        attackTime3Item.OnActivation += AttackTimeEnhance;

        healAmount1Item.OnActivation += HealingEnhance;
        healAmount2Item.OnActivation += HealingEnhance;
        healAmount3Item.OnActivation += HealingEnhance;

        recruitingChance1Item.OnActivation += RecruitmentChanceEnhance;
        recruitingChance2Item.OnActivation += RecruitmentChanceEnhance;
        recruitingChance3Item.OnActivation += RecruitmentChanceEnhance;

        explosiveDamages1Item.OnActivation += ExplosiveDamagesEnhance;
        explosiveDamages2Item.OnActivation += ExplosiveDamagesEnhance;
        explosiveDamages3Item.OnActivation += ExplosiveDamagesEnhance;

        recruitingWithXPItem.OnActivation += RecruitmentXPEnhance;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        squad2Item.OnActivation -= UnlockSquad;
        squad3Item.OnActivation -= UnlockSquad;
        squad4Item.OnActivation -= UnlockSquad;

        attackTime1Item.OnActivation -= AttackTimeEnhance;
        attackTime2Item.OnActivation -= AttackTimeEnhance;
        attackTime3Item.OnActivation -= AttackTimeEnhance;

        healAmount1Item.OnActivation -= HealingEnhance;
        healAmount2Item.OnActivation -= HealingEnhance;
        healAmount3Item.OnActivation -= HealingEnhance;

        recruitingChance1Item.OnActivation -= RecruitmentChanceEnhance;
        recruitingChance2Item.OnActivation -= RecruitmentChanceEnhance;
        recruitingChance3Item.OnActivation -= RecruitmentChanceEnhance;

        explosiveDamages1Item.OnActivation -= ExplosiveDamagesEnhance;
        explosiveDamages2Item.OnActivation -= ExplosiveDamagesEnhance;
        explosiveDamages3Item.OnActivation -= ExplosiveDamagesEnhance;

        recruitingWithXPItem.OnActivation -= RecruitmentXPEnhance;
    }

    /// <summary>
    /// Show method displays and sets the Canvas
    /// </summary>
    public override void Show()
    {
        base.Show();
        Setup();
    }

    /// <summary>
    /// Setup method initializes the Canvas and all its item
    /// </summary>
    private void Setup()
    {
        // Get PlayManager static values and setup buttons backgrounds & interactability
        squad2Item.Setup("Operation center", "Unlock a 2nd squad", PlayManager.data.facilities.squad2Cost, (PlayManager.squadList.Count < 2),true, squad3Item);
        squad3Item.Setup("Dormitory", "Unlock a 3rd squad", PlayManager.data.facilities.squad3Cost, (PlayManager.squadList.Count < 3), (PlayManager.squadList.Count > 1), squad4Item);
        squad4Item.Setup("Bunk beds", "Unlock a 4th squad", PlayManager.data.facilities.squad4Cost, (PlayManager.squadList.Count < 4), (PlayManager.squadList.Count > 2));

        string _text = "Attack time +" + PlayManager.data.facilities.timeBonus.ToString() + "s";
        attackTime1Item.Setup("Teamwork", _text, PlayManager.data.facilities.attackTime1Cost, (PlayManager.attackTimeLevel < 1), true, attackTime2Item);
        attackTime2Item.Setup("Advanced communication", _text, PlayManager.data.facilities.attackTime2Cost, (PlayManager.attackTimeLevel < 2), (PlayManager.attackTimeLevel > 0), attackTime3Item);
        attackTime3Item.Setup("Protein pills", _text, PlayManager.data.facilities.attackTime3Cost, (PlayManager.attackTimeLevel < 3), (PlayManager.attackTimeLevel > 1));

        _text = "HQ heal capacity +" + PlayManager.data.facilities.heal1Bonus.ToString();
        healAmount1Item.Setup("Infirmary", _text, PlayManager.data.facilities.healing1Cost, PlayManager.healLevel < 1, true, healAmount2Item);
        _text = "HQ heal capacity +" + PlayManager.data.facilities.heal2Bonus.ToString();
        healAmount2Item.Setup("Surgery", _text, PlayManager.data.facilities.healing2Cost, PlayManager.healLevel < 2, PlayManager.healLevel > 0, healAmount3Item);
        _text = "HQ heal capacity +" + PlayManager.data.facilities.heal3Bonus.ToString();
        healAmount3Item.Setup("Nanomedicine", _text, PlayManager.data.facilities.healing3Cost, PlayManager.healLevel < 3, PlayManager.healLevel > 1);

        _text = "Recruiting chances +" + PlayManager.data.facilities.recruiting1Bonus.ToString() + "%";
        recruitingChance1Item.Setup("Humanitarian effort ", _text, PlayManager.data.facilities.recruiting1Cost, (PlayManager.recruitmentLevel < 1), true, recruitingChance2Item);
        _text = "Recruiting chances +" + PlayManager.data.facilities.recruiting2Bonus.ToString() + "%";
        recruitingChance2Item.Setup("Propaganda", _text, PlayManager.data.facilities.recruiting2Cost, (PlayManager.recruitmentLevel < 2), (PlayManager.recruitmentLevel > 0), recruitingChance3Item);
        _text = "Recruiting chances +" + PlayManager.data.facilities.recruiting3Bonus.ToString() + "%";
        recruitingChance3Item.Setup("City rebuilding", _text, PlayManager.data.facilities.recruiting3Cost, (PlayManager.recruitmentLevel < 3), (PlayManager.recruitmentLevel > 1));

        _text = "Explosives damages +" + PlayManager.data.facilities.exploDamages1Bonus.ToString();
        explosiveDamages1Item.Setup("Enhanced chemistry", _text, PlayManager.data.facilities.explosive1Cost, (PlayManager.explosivesLevel < 1), true, explosiveDamages2Item);
        _text = "Explosives damages +" + PlayManager.data.facilities.exploDamages2Bonus.ToString();
        explosiveDamages2Item.Setup("Enhanced brisance", _text, PlayManager.data.facilities.explosive2Cost, (PlayManager.explosivesLevel < 2), (PlayManager.explosivesLevel > 0), explosiveDamages3Item);
        _text = "Explosives damages +" + PlayManager.data.facilities.exploDamages3Bonus.ToString();
        explosiveDamages3Item.Setup("Chemical weapons", _text, PlayManager.data.facilities.explosive3Cost, (PlayManager.explosivesLevel < 3), (PlayManager.explosivesLevel > 1));

        recruitingWithXPItem.Setup("Basic training", "Recruit soldiers at Private rank", PlayManager.data.facilities.soldierXPCost, (PlayManager.recruitingWithXP < 1), true);
    }

    /// <summary>
    /// Unlock method checks the workforce amount and unlocks the item
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    /// <returns></returns>
    private bool Unlock(FacilitiesItem _item)
    {
        if(PlayManager.workforce >= _item.Cost)
        {
            // Pay the workforce cost
            PlayManager.UpdateWorkforce(-_item.Cost);
            // Unlock the facilities item
            _item.Lock(false);
            // Set the next item as available
            if (_item.NextItem != null)
            {
                _item.NextItem.SetAvailable(true);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// UnlockSquad method add a new squad on item unlock
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    public void UnlockSquad(FacilitiesItem _item)
    {
        if (Unlock(_item))
        {
            Squad _squad = ScriptableObject.CreateInstance("Squad") as Squad;
            _squad.InitData();
            PlayManager.squadList.Add(_squad);
            OnNewSquad?.Invoke();
        }
    }

    /// <summary>
    /// AttackTimeEnhance method add a level to AttackTime on item unlock
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    public void AttackTimeEnhance(FacilitiesItem _item)
    {
        if(Unlock(_item))
        {
            PlayManager.attackTimeLevel++;
        }
    }

    /// <summary>
    /// HealingEnhance method add a level to HealLevel on item unlock
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    public void HealingEnhance(FacilitiesItem _item)
    {
        if(Unlock(_item))
        {
            PlayManager.healLevel++;
        }
    }

    /// <summary>
    /// RecruitmentChanceEnhance method add a level to RecruitmentLevel on item unlock
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    public void RecruitmentChanceEnhance(FacilitiesItem _item)
    {
        if (Unlock(_item))
        {
            PlayManager.recruitmentLevel++;

            // Add immediatelty recruiting bonus to the recruitment variable
            switch(PlayManager.recruitmentLevel)
            {
                case 1:
                    PlayManager.recruitment += PlayManager.data.facilities.recruiting1Bonus;
                    break;
                case 2:
                    PlayManager.recruitment += PlayManager.data.facilities.recruiting2Bonus;
                    break;
                case 3:
                    PlayManager.recruitment += PlayManager.data.facilities.recruiting3Bonus;
                    break;
            }
        }
    }

    /// <summary>
    /// ExplosiveDamagesEnhance method add a level to ExplosivesLevel on item unlock
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    public void ExplosiveDamagesEnhance(FacilitiesItem _item)
    {
        if (Unlock(_item))
        {
            PlayManager.explosivesLevel++;
        }
    }

    /// <summary>
    /// RecruitmentXPEnhance method add a level to RecruitingWithXP on item unlock
    /// </summary>
    /// <param name="_item">Item to unlock (FacilitiesItem)</param>
    public void RecruitmentXPEnhance(FacilitiesItem _item)
    {
        if (Unlock(_item))
        {
            PlayManager.recruitingWithXP++;
        }
    }
}
