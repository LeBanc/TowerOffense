using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SoldierPreview is used to preview the soldier in the HQSquadEdition Canvas
/// </summary>
public class SoldierPreview : MonoBehaviour
{
    // Public elements linked into the prefab
    [Header("Global")]
    public SoldierImage soldierImage; //SoldierImage (background, sprite and border)
    public Text soldierName; // Soldier Grade & First and Last names
    public Text soldierType; // Soldier type

    // Health and XP bars
    public HealthBar healthBar;
    public HealthBar experienceBar;

    // Soldier data
    [Header("Values")]
    public Text attackShortValue;
    public Text attackMiddleValue;
    public Text attackLongValue;
    public Text defenseShortValue;
    public Text defenseMiddleValue;
    public Text defenseLongValue;
    public Text defenseExplosivesValue;
    public Text speedValue;

    public Text attackShortBonus;
    public Text attackMiddleBonus;
    public Text attackLongBonus;
    public Text defenseShortBonus;
    public Text defenseMiddleBonus;
    public Text defenseLongBonus;
    public Text defenseExplosivesBonus;
    public Text speedBonus;

    // Soldier capacities
    [Header("Capacities")]
    public Text capacity1;
    public Text capacity2;
    public Text capacity3;
    public Text capacity4;

    // Private element: Soldier displayed in this SoldierPreview
    private Soldier soldier;

    /// <summary>
    /// On Start, subscribe to events
    /// </summary>
    private void Start()
    {
        PlayManager.OnHQPhase += UpdateHPBarValue;
        PlayManager.OnHQPhase += UpdateXPBarValue;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnHQPhase -= UpdateHPBarValue;
        PlayManager.OnHQPhase -= UpdateXPBarValue;
    }

    /// <summary>
    /// Setup method initialize the SoldierPreview with the Soldier data
    /// </summary>
    /// <param name="_soldier">Soldier to display</param>
    public void Setup(Soldier _soldier)
    {
        // Activate the SoldierPreview
        Show();

        // Link the Soldier to this SoldierPreview
        soldier = _soldier;

        if(soldier != null)
        {
            // Update all text and images
            soldierImage.Setup(soldier, true);
            soldierName.text = PlayManager.data.ranks[soldier.Data.soldierLevel] + " " + soldier.Name;
            soldierType.text = soldier.Data.typeName;

            UpdateHPBarValue();
            UpdateXPBarValue();

            attackShortValue.text = soldier.ShortRangeAttack.ToString();
            attackShortBonus.text = (soldier.BonusAtkShortRange > 0) ? ("+" + soldier.BonusAtkShortRange.ToString()) : ((soldier.BonusAtkShortRange < 0) ? soldier.BonusAtkShortRange.ToString() : "");
            attackShortBonus.color = GetBonusColor(soldier.BonusAtkShortRange);
            attackMiddleValue.text = soldier.MiddleRangeAttack.ToString();
            attackMiddleBonus.text = (soldier.BonusAtkMidRange > 0) ? ("+" + soldier.BonusAtkMidRange.ToString()) : ((soldier.BonusAtkMidRange < 0) ? soldier.BonusAtkMidRange.ToString() : "");
            attackMiddleBonus.color = GetBonusColor(soldier.BonusAtkMidRange);
            attackLongValue.text = soldier.LongRangeAttack.ToString();
            attackLongBonus.text = (soldier.BonusAtkLongRange > 0) ? ("+" + soldier.BonusAtkLongRange.ToString()) : ((soldier.BonusAtkLongRange < 0) ? soldier.BonusAtkLongRange.ToString() : "");
            attackLongBonus.color = GetBonusColor(soldier.BonusAtkLongRange);

            defenseShortValue.text = soldier.ShortRangeDefense.ToString();
            defenseShortBonus.text = (soldier.BonusDefShortRange > 0) ? ("+" + soldier.BonusDefShortRange.ToString()) : ((soldier.BonusDefShortRange < 0) ? soldier.BonusDefShortRange.ToString() : "");
            defenseShortBonus.color = GetBonusColor(soldier.BonusDefShortRange);
            defenseMiddleValue.text = soldier.MiddleRangeDefense.ToString();
            defenseMiddleBonus.text = (soldier.BonusDefMidRange > 0) ? ("+" + soldier.BonusDefMidRange.ToString()) : ((soldier.BonusDefMidRange < 0) ? soldier.BonusDefMidRange.ToString() : "");
            defenseMiddleBonus.color = GetBonusColor(soldier.BonusDefMidRange);
            defenseLongValue.text = soldier.LongRangeDefense.ToString();
            defenseLongBonus.text = (soldier.BonusDefLongRange > 0) ? ("+" + soldier.BonusDefLongRange.ToString()) : ((soldier.BonusDefLongRange < 0) ? soldier.BonusDefLongRange.ToString() : "");
            defenseLongBonus.color = GetBonusColor(soldier.BonusDefLongRange);
            defenseExplosivesValue.text = soldier.ExplosivesDefense.ToString();
            defenseExplosivesBonus.text = (soldier.BonusDefExplosives > 0) ? ("+" + soldier.BonusDefExplosives.ToString()) : ((soldier.BonusDefExplosives < 0) ? soldier.BonusDefExplosives.ToString() : "");
            defenseExplosivesBonus.color = GetBonusColor(soldier.BonusDefExplosives);

            speedValue.text = soldier.Speed.ToString();
            speedBonus.text = (soldier.BonusSpeed > 0) ? ("+" + soldier.BonusSpeed.ToString()) : ((soldier.BonusSpeed < 0) ? soldier.BonusSpeed.ToString() : "");
            speedBonus.color = GetBonusColor(soldier.BonusSpeed);

            // Update capacities
            GetCapacities();
        }
        else
        {
            // Reset values
            soldierImage.Setup(null);
            soldierName.text = "";
            soldierType.text = "";
            healthBar.UpdateValue(0, 0);
            experienceBar.UpdateValue(0, 0);

            attackShortValue.text = "";
            attackShortBonus.text = "";
            attackMiddleValue.text = "";
            attackMiddleBonus.text = "";
            attackLongValue.text = "";
            attackLongBonus.text = "";

            defenseShortValue.text = "";
            defenseShortBonus.text = "";
            defenseMiddleValue.text = "";
            defenseMiddleBonus.text = "";
            defenseLongValue.text = "";
            defenseLongBonus.text = "";
            defenseExplosivesValue.text = "";
            defenseExplosivesBonus.text = "";

            speedValue.text = "";
            speedBonus.text = "";

            capacity1.text = "";
            capacity2.text = "";
            capacity3.text = "";
            capacity4.text = "";
        }        
    }

    /// <summary>
    /// Hide method hides the SoldierPreview element
    /// </summary>
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Show method shows the SoldierPreview element
    /// </summary>
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// GetBonusColor method returns green if the bonus is positive, red if negative and black if 0
    /// </summary>
    /// <param name="_bonus">Bonus value to look at</param>
    /// <returns>Color of the bonus</returns>
    private Color GetBonusColor(int _bonus)
    {
        if (_bonus > 0)
        {
            return new Color(0f, 0.5f, 0.05f, 1f);
        }
        else if (_bonus < 0)
        {
            return Color.red;
        }
        else
        {
            return Color.gray;
        }
    }

    /// <summary>
    /// GetCapacities method loops through Soldier capacities and sets the Capacities text labels
    /// </summary>
    private void GetCapacities()
    {
        // Initialize capacities' amount
        int _increaseSpeed = 0;
        int _heal = 0;
        int _hqBuild = 0;
        int _turretBuild = 0;
        int _explosives = 0;
        int _woundedSaving = 0;

        // Get capacities and increment their amount
        foreach (SoldierData.Capacities _soldierCapa in soldier.Data.capacities)
        {
            switch (_soldierCapa)
            {
                case SoldierData.Capacities.IncreaseSpeed:
                    _increaseSpeed++;
                    break;
                case SoldierData.Capacities.Heal:
                    _heal++;
                    break;
                case SoldierData.Capacities.HQBuild:
                    _hqBuild++;
                    break;
                case SoldierData.Capacities.TurretBuild:
                    _turretBuild++;
                    break;
                case SoldierData.Capacities.Explosives:
                    _explosives++;
                    break;
                case SoldierData.Capacities.WoundedSaving:
                    _woundedSaving++;
                    break;
            }
        }

        // Clear text label
        capacity1.text = "";
        capacity2.text = "";
        capacity3.text = "";
        capacity4.text = "";

        // For each capacity, add its index to a list if it is not null
        List<int> _indexes = new List<int>();
        int[] _capa = { _increaseSpeed, _heal, _hqBuild, _turretBuild, _explosives, _woundedSaving };
        for (int i = 0; i < _capa.Length; i++)
        {
            if (_capa[i] > 0) _indexes.Add(i);
        }
        // If their is no capacity enables, return here
        if (_indexes.Count == 0) return;

        Text[] _capacitiesLabel = { capacity1, capacity2, capacity3, capacity4 };
        // For each not null capacity (in _indexes list)
        for (int i = 0; i < Mathf.Min(_indexes.Count, _capacitiesLabel.Length); i++) // There are 4 capacity labels so the loop should not go ever 4
        {
            // Get the index of the capacity and build the right text (with amount if needed)
            switch (_indexes[i])
            {
                case 0:
                    _capacitiesLabel[i].text = "Increase Speed" + ((_increaseSpeed > 1) ? string.Format(" x{0}", _increaseSpeed) : "");
                    break;
                case 1:
                    _capacitiesLabel[i].text = "Heal" + ((_heal > 1) ? string.Format(" x{0}", _heal) : "");
                    break;
                case 2:
                    _capacitiesLabel[i].text = "Build new HQ" + ((_hqBuild > 1) ? string.Format(" x{0}", _hqBuild) : "");
                    break;
                case 3:
                    _capacitiesLabel[i].text = "Build Turret" + ((_turretBuild > 1) ? string.Format(" x{0}", _turretBuild) : "");
                    break;
                case 4:
                    _capacitiesLabel[i].text = "Explosives" + ((_explosives > 1) ? string.Format(" x{0}", _explosives) : "");
                    break;
                case 5:
                    _capacitiesLabel[i].text = "Save Wounded Soldiers" + ((_woundedSaving > 1) ? string.Format(" x{0}", _woundedSaving) : "");
                    break;
            }
        }
    }

    /// <summary>
    /// UpdateHPBarValue method updates the value of the HealthBar
    /// </summary>
    private void UpdateHPBarValue()
    {
        if(soldier!=null) healthBar.UpdateValue(soldier.CurrentHP, soldier.MaxHP);
    }

    /// <summary>
    /// UpdateXPBarValue method updates the value of the ExperienceBar
    /// </summary>
    private void UpdateXPBarValue()
    {
        if (soldier != null) experienceBar.UpdateValue(Mathf.Min(soldier.CurrentXP, soldier.MaxXP), soldier.MaxXP);
    }
}
