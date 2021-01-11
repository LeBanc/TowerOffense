using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// HQLevelUpItem class updates the LevelUp item
/// </summary>
public class HQLevelUpItem : Button
{
    // public UI elements
    public Image selectedBackground;
    public Text typeName;
    public Text maxHP;
    public Text speed;
    public Text shotDelay;
    public Text attackShortRange;
    public Text attackMiddleRange;
    public Text attackLongRange;
    public Text defenseShortRange;
    public Text defenseMiddleRange;
    public Text defenseLongRange;
    public Text defenseExplosives;
    public Text capacity1;
    public Text capacity2;
    public Text capacity3;
    public Text capacity4;

    /// <summary>
    /// Setup method initializes the item with soldier data and new class data
    /// </summary>
    /// <param name="_soldier">Selected soldier</param>
    /// <param name="_data">Selected new class</param>
    public void Setup(Soldier _soldier, SoldierData _data)
    {
        // Set as unselected
        selectedBackground.enabled = false;

        // Setup the Type name
        typeName.text = _data.typeName;
        if (_data.soldierType == SoldierData.SoldierType.Attack)
        {
            typeName.color = Color.red;
        }
        else if (_data.soldierType == SoldierData.SoldierType.Defense)
        {
            typeName.color = Color.blue;
        }
        else if (_data.soldierType == SoldierData.SoldierType.Special)
        {
            typeName.color = new Color(0f, 0.5f, 0.05f, 1f);
        }
        else
        {
            typeName.color = Color.black;
        }

        // Setup Max HP
        maxHP.text = _data.maxHP.ToString();
        maxHP.color = GetColor(_soldier.Data.maxHP, _data.maxHP);
        // Setup speed
        speed.text = _data.speed.ToString();
        speed.color = GetColor(_soldier.Data.speed, _data.speed);
        // Setup shooting delay
        shotDelay.text = _data.shootingDelay.ToString();
        shotDelay.color = GetColor(_soldier.Data.shootingDelay, _data.shootingDelay);
        // Setup attack values
        attackShortRange.text = _data.shortRangeAttack.ToString();
        attackShortRange.color = GetColor(_soldier.Data.shortRangeAttack, _data.shortRangeAttack);
        attackMiddleRange.text = _data.middleRangeAttack.ToString();
        attackMiddleRange.color = GetColor(_soldier.Data.middleRangeAttack, _data.middleRangeAttack);
        attackLongRange.text = _data.longRangeAttack.ToString();
        attackLongRange.color = GetColor(_soldier.Data.longRangeAttack, _data.longRangeAttack);
        // Setup defense values
        defenseShortRange.text = _data.shortRangeDefense.ToString();
        defenseShortRange.color = GetColor(_soldier.Data.shortRangeDefense, _data.shortRangeDefense);
        defenseMiddleRange.text = _data.middleRangeDefense.ToString();
        defenseMiddleRange.color = GetColor(_soldier.Data.middleRangeDefense, _data.middleRangeDefense);
        defenseLongRange.text = _data.longRangeDefense.ToString();
        defenseLongRange.color = GetColor(_soldier.Data.longRangeDefense, _data.longRangeDefense);
        defenseExplosives.text = _data.explosiveDefense.ToString();
        defenseExplosives.color = GetColor(_soldier.Data.explosiveDefense, _data.explosiveDefense);

        // Setup capacities
        GetCapacities(_soldier.Data, _data);
    }

    /// <summary>
    /// GetColor method returns the color (red/green/black) depending of the input
    /// </summary>
    /// <param name="_current">Current data</param>
    /// <param name="_evolve">New data</param>
    /// <returns>Red if new less than old, green if new more than old, black if equal</returns>
    private Color GetColor(int _current, int _evolve)
    {
        if (_current > _evolve) return Color.red;
        if (_current < _evolve) return new Color(0f, 0.5f, 0.05f, 1f);
        return Color.black;
    }

    /// <summary>
    /// GetColor method returns the color (red/green/black) depending of the input
    /// </summary>
    /// <param name="_current">Current data</param>
    /// <param name="_evolve">New data</param>
    /// <returns>Red if new less than old, green if new more than old, black if equal</returns>
    private Color GetColor(float _current, float _evolve)
    {
        if (_current > _evolve) return Color.red;
        if (_current < _evolve) return new Color(0f, 0.5f, 0.05f, 1f);
        return Color.black;
    }

    /// <summary>
    /// GetCapacities initializes the Capacity Text from old and new data
    /// </summary>
    /// <param name="_oldData">Current soldier data</param>
    /// <param name="_newData">New soldier data</param>
    private void GetCapacities (SoldierData _oldData, SoldierData _newData)
    {
        // Get the current capacities
        int _increaseSpeed = 0;
        int _heal = 0;
        int _hqBuild = 0;
        int _turretBuild = 0;
        int _explosives = 0;
        int _woundedSaving = 0;
        foreach (SoldierData.Capacities _soldierCapa in _oldData.capacities)
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

        // Get the upgrade capacities
        int _increaseSpeedNew = 0;
        int _healNew = 0;
        int _hqBuildNew = 0;
        int _turretBuildNew = 0;
        int _explosivesNew = 0;
        int _woundedSavingNew = 0;
        foreach (SoldierData.Capacities _soldierCapa in _newData.capacities)
        {
            switch (_soldierCapa)
            {
                case SoldierData.Capacities.IncreaseSpeed:
                    _increaseSpeedNew++;
                    break;
                case SoldierData.Capacities.Heal:
                    _healNew++;
                    break;
                case SoldierData.Capacities.HQBuild:
                    _hqBuildNew++;
                    break;
                case SoldierData.Capacities.TurretBuild:
                    _turretBuildNew++;
                    break;
                case SoldierData.Capacities.Explosives:
                    _explosivesNew++;
                    break;
                case SoldierData.Capacities.WoundedSaving:
                    _woundedSavingNew++;
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
        int[] _capaNew = { _increaseSpeedNew, _healNew, _hqBuildNew, _turretBuildNew, _explosivesNew, _woundedSavingNew };
        for (int i = 0; i < _capaNew.Length; i++)
        {
            if (_capaNew[i] > 0 || _capa[i] > 0) _indexes.Add(i); // if the capacity exists in one of the data, add it to the list
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
                    if(_increaseSpeedNew > 0)
                    {
                        _capacitiesLabel[i].text = "Increase Speed" + ((_increaseSpeedNew > 1) ? string.Format(" x{0}", _increaseSpeedNew) : "");
                    }
                    else
                    {
                        _capacitiesLabel[i].text = "Increase Speed" + ((_increaseSpeed > 1) ? string.Format(" x{0}", _increaseSpeed) : "");
                    }
                    _capacitiesLabel[i].color = GetColor(_increaseSpeed, _increaseSpeedNew);
                    break;
                case 1:
                    if(_healNew > 0)
                    {
                        _capacitiesLabel[i].text = "Heal" + ((_healNew > 1) ? string.Format(" x{0}", _healNew) : "");
                    }
                    else
                    {
                        _capacitiesLabel[i].text = "Heal" + ((_heal > 1) ? string.Format(" x{0}", _heal) : "");
                    }
                    _capacitiesLabel[i].color = GetColor(_heal, _healNew);
                    break;
                case 2:
                    if (_hqBuildNew > 0)
                    {
                        _capacitiesLabel[i].text = "Build new HQ" + ((_hqBuildNew > 1) ? string.Format(" x{0}", _hqBuildNew) : "");
                    }
                    else
                    {
                        _capacitiesLabel[i].text = "Build new HQ" + ((_hqBuild > 1) ? string.Format(" x{0}", _hqBuild) : "");
                    }
                    _capacitiesLabel[i].color = GetColor(_hqBuild, _hqBuildNew);
                    break;
                case 3:
                    if (_turretBuildNew > 0)
                    {
                        _capacitiesLabel[i].text = "Build Turret" + ((_turretBuildNew > 1) ? string.Format(" x{0}", _turretBuildNew) : "");
                    }
                    else
                    {
                        _capacitiesLabel[i].text = "Build Turret" + ((_turretBuild > 1) ? string.Format(" x{0}", _turretBuild) : "");
                    }
                    _capacitiesLabel[i].color = GetColor(_turretBuild, _turretBuildNew);
                    break;
                case 4:
                    if (_explosivesNew > 0)
                    {
                        _capacitiesLabel[i].text = "Explosives" + ((_explosivesNew > 1) ? string.Format(" x{0}", _explosivesNew) : "");
                    }
                    else
                    {
                        _capacitiesLabel[i].text = "Explosives" + ((_explosives > 1) ? string.Format(" x{0}", _explosives) : "");
                    }
                    _capacitiesLabel[i].color = GetColor(_explosives, _explosivesNew);
                    break;
                case 5:
                    if (_woundedSavingNew > 0)
                    {
                        _capacitiesLabel[i].text = "Save Wounded Soldiers" + ((_woundedSavingNew > 1) ? string.Format(" x{0}", _woundedSavingNew) : "");
                    }
                    else
                    {
                        _capacitiesLabel[i].text = "Save Wounded Soldiers" + ((_woundedSaving > 1) ? string.Format(" x{0}", _woundedSaving) : "");
                    }
                    _capacitiesLabel[i].color = GetColor(_woundedSaving, _woundedSavingNew);
                    break;
            }
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onClick?.Invoke();
    }
}
