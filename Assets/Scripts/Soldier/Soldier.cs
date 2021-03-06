﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Soldier is the class that represents the soldier with all its data
/// </summary>
public class Soldier : ScriptableObject
{
    // iD number that identify the soldier 
    private int iD;

    // Soldier name
    private string soldierName;

    // Soldier current HP
    private int currentHP;

    // Soldier current XP
    private int currentXP;

    // Image path (name - for data save) and sprite
    private string imagePath;
    private Sprite image;
    
    // Data path (name - for data save) and SoldierData
    private string dataPath;
    private SoldierData data;

    // State boolean (engaged / dead)
    private bool isEngaged = false;
    private int dayOfDeath = 0;

    // Bonuses that can be applied to the soldier
    private int[] attackBonuses = new int[3];
    private int[] defenseBonuses = new int[4];
    private int speedBonus;
    private int friendshipValue;

    // Squad attached to
    private Squad squad;

    // private ??? friendships; => to be implemented
    private Dictionary<int, int> friendship = new Dictionary<int, int>();

    // Events
    public delegate void SoldierEventHandler();
    public event SoldierEventHandler OnNameChange;
    public event SoldierEventHandler OnImageChange;
    public event SoldierEventHandler OnDataChange;


    #region Properties access
    public int ID
    {
        get { return iD; }
    }

    public string Name
    {
        get { return soldierName; }
    }

    public Sprite Image
    {
        get { return image; }
    }

    public Dictionary<int,int> Friendship
    {
        get { return friendship; }
    }

    public SoldierData Data
    {
        get { return data; }
    }

    public int MaxHP
    {
        get { return data.maxHP; }
    }

    public int MaxXP
    {
        get { return data.maxXP; }
    }

    public int CurrentHP
    {
        set { currentHP = value; }
        get { return currentHP; }
    }

    public int CurrentXP
    {
        set { currentXP = value; }
        get { return currentXP; }
    }

    public bool CanLevelUp
    {
        get { return ((currentXP >= data.maxXP) && (data.maxXP >0)); }
    }

    public int DayOfDeath
    {
        get { return dayOfDeath; }
    }

    public bool IsDead
    {
        get { return dayOfDeath > 0; }
    }

    public bool IsEngaged
    {
        set { isEngaged = value; }
        get { return isEngaged; }
    }

    public int ShortRangeAttack
    {
        get { return data.shortRangeAttack; }
    }

    public int MiddleRangeAttack
    {
        get { return data.middleRangeAttack; }
    }

    public int LongRangeAttack
    {
        get { return data.longRangeAttack; }
    }

    public int ShortRangeDefense
    {
        get { return data.shortRangeDefense; }
    }

    public int MiddleRangeDefense
    {
        get { return data.middleRangeDefense; }
    }

    public int LongRangeDefense
    {
        get { return data.longRangeDefense; }
    }

    public int ExplosivesDefense
    {
        get { return data.explosiveDefense; }
    }

    public int Speed
    {
        get { return data.speed; }
    }

    public int BonusAtkShortRange
    {
        set { attackBonuses[0] = value; }
        get { return attackBonuses[0]; }
    }

    public int BonusAtkMidRange
    {
        set { attackBonuses[1] = value; }
        get { return attackBonuses[1]; }
    }

    public int BonusAtkLongRange
    {
        set { attackBonuses[2] = value; }
        get { return attackBonuses[2]; }
    }
    public int BonusDefShortRange
    {
        set { defenseBonuses[0] = value; }
        get { return defenseBonuses[0]; }
    }

    public int BonusDefMidRange
    {
        set { defenseBonuses[1] = value; }
        get { return defenseBonuses[1]; }
    }

    public int BonusDefLongRange
    {
        set { defenseBonuses[2] = value; }
        get { return defenseBonuses[2]; }
    }
    public int BonusDefExplosives
    {
        set { defenseBonuses[3] = value; }
        get { return defenseBonuses[3]; }
    }

    public int BonusSpeed
    {
        set { speedBonus = value; }
        get { return speedBonus; }
    }

    public Squad Squad
    {
        get { return squad; }
    }

    #endregion

    /// <summary>
    /// InitData initializes a new Soldier ScriptableObject from a SoldierData
    /// </summary>
    /// <param name="_newData">SoldierData (type)</param>
    public void InitData(SoldierData _newData)
    {
        // Gets values from PlayManager
        iD = PlayManager.GetFreeSoldierID();
        soldierName = PlayManager.GetRandomSoldierName();
        image = PlayManager.GetRandomSoldierImage();
        imagePath = image.name;

        // Sets values from SoldierData
        data = _newData;
        dataPath = data.name;
        currentHP = data.maxHP;

        // Sets default value
        currentXP = 0;
        squad = null;
        friendship = new Dictionary<int, int>();
    }

    /// <summary>
    /// LoadData loads soldier data from dedicated data (from save file)
    /// </summary>
    /// <param name="_ID">iD number</param>
    /// <param name="_NAME">Name</param>
    /// <param name="_IMAGE">Image name</param>
    /// <param name="_DATA">Data name</param>
    /// <param name="_HP">HP current value</param>
    /// <param name="_XP">XP current value</param>
    public void LoadData(int _ID, string _NAME, string _IMAGE, string _DATA, int _HP, int _XP,int _dayOfDeath, int[] _friendshipArray)
    {
        iD = _ID;
        soldierName = _NAME;
        imagePath = _IMAGE;
        image = PlayManager.data.soldierImages.Find(x => x.name.Equals(imagePath));
        dataPath = _DATA;
        data = Resources.Load("SoldierData/" + dataPath) as SoldierData;
        currentHP = _HP;
        currentXP = _XP;
        squad = null;
        dayOfDeath = _dayOfDeath;

        if(_friendshipArray.Length > 1)
        {
            // Load friendship points
            for (int i = 0; i < _friendshipArray.Length - 1; i += 2)
            {
                friendship.Add(_friendshipArray[i], _friendshipArray[i + 1]);
            }
        }        
    }

    /// <summary>
    /// Heal method adds HP to current HP
    /// </summary>
    /// <param name="hp">Heal value</param>
    public void Heal(int hp)
    {
        currentHP += hp;
        if (currentHP > MaxHP) currentHP = MaxHP;
    }

    /// <summary>
    /// Die method sets the Soldier as dead
    /// </summary>
    public void Die()
    {
        dayOfDeath = PlayManager.day;
    }

    /// <summary>
    /// AddFriendshipPoint method add a friendship point for a dedicated soldier
    /// </summary>
    /// <param name="_soldierID">Soldier ID for wich a point has to be added</param>
    public void AddFriendshipPoint(int _soldierID)
    {
        if(friendship.ContainsKey(_soldierID))
        {
            friendship[_soldierID]++;
        }
        else
        {
            friendship.Add(_soldierID, 1);
        }
    }

    /// <summary>
    /// AddToSquad sets the soldier's squad
    /// </summary>
    /// <param name="_squad">Squad to select</param>
    public void AddToSquad(Squad _squad)
    {
        squad = _squad;
    }

    /// <summary>
    /// RemoveFromSquad clears the squad value of this Soldier
    /// </summary>
    public void RemoveFromSquad()
    {
        if(squad != null) squad.RemoveSoldier(this);
        squad = null;
    }

    /// <summary>
    /// ChangeName methods changes the soldier's name
    /// </summary>
    /// <param name="_name">New name</param>
    public void ChangeName(string _name)
    {
        soldierName = _name;
        OnNameChange?.Invoke();
    }

    /// <summary>
    /// ChangeImage method changes the soldier's sprite
    /// </summary>
    /// <param name="_sprite">New sprite</param>
    public void ChangeImage(Sprite _sprite)
    {
        image = _sprite;
        OnImageChange?.Invoke();
    }

    /// <summary>
    /// Evolve method changes the SoldierData
    /// </summary>
    /// <param name="_newData"></param>
    public void Evolve(SoldierData _newData)
    {
        // Get the MaxHP of the current type
        int oldMaxHP = MaxHP;

        // Substract the XP cost of the level gained
        currentXP = currentXP - data.maxXP;
        
        // Set new data
        data = _newData;
        dataPath = data.name;

        // Reset XP if max level
        if (data.maxXP == 0) currentXP = 0;
        
        // Set life proportionnaly of the MaxHP change
        currentHP = Mathf.Max(1,(currentHP * MaxHP) / oldMaxHP);

        OnDataChange?.Invoke();
    }

    #region Friendship and Bonuses
    /// <summary>
    /// ComputeBonuses method
    /// </summary>
    /// <param name="_squad">Squad from which get the soldiers to compute bonuses (Squad)</param>
    public void ComputeBonuses(Squad _squad)
    {
        attackBonuses = new int[3];
        defenseBonuses = new int[4];
        speedBonus = 0;

        foreach (Soldier _s in _squad.Soldiers)
        {
            if(_s != null)
            {
                if (_s != this)
                {
                    ComputeBonuses(_s);
                }
            }
        }
        ComputeMaluses();
    }

    /// <summary>
    /// ComputeBonuses method adds the bonuses given by a dedicated soldier
    /// </summary>
    /// <param name="_soldier">Soldier from which get the bonuses (Soldier)</param>
    private void ComputeBonuses(Soldier _soldier)
    {
        int _mult = 0;
        if (friendship.TryGetValue(_soldier.iD, out int _value))
        {
            if (_value >= PlayManager.data.friendshipLevels[4].threshold)
            {
                _mult = 4;
            }
            else if(_value >= PlayManager.data.friendshipLevels[3].threshold)
            {
                _mult = 3;
            }
            else if(_value >= PlayManager.data.friendshipLevels[2].threshold)
            {
                _mult = 2;
            }
            else if(_value >= PlayManager.data.friendshipLevels[1].threshold)
            {
                _mult = 1;
            }

            attackBonuses[0] += _soldier.data.attackBonus[0] * _mult;
            attackBonuses[1] += _soldier.data.attackBonus[1] * _mult;
            attackBonuses[2] += _soldier.data.attackBonus[2] * _mult;
            defenseBonuses[0] += _soldier.data.defenseBonus[0] * _mult;
            defenseBonuses[1] += _soldier.data.defenseBonus[1] * _mult;
            defenseBonuses[2] += _soldier.data.defenseBonus[2] * _mult;
            defenseBonuses[3] += _soldier.data.defenseBonus[3] * _mult;
            speedBonus += _soldier.data.speedBonus * _mult;
        }
    }

    /// <summary>
    /// ComputeMaluses method computes the negative bonuses for dead soldiers
    /// </summary>
    private void ComputeMaluses()
    {
        int _malus = 0;

        foreach (Soldier _s in PlayManager.soldierList)
        {
            if(_s.IsDead)
            {
                int _mourningDays = 2; // 2 because the day the computing is done is at least one day after the death
                if (friendship.TryGetValue(_s.iD, out int _value))
                {
                    if (_value >= PlayManager.data.friendshipLevels[4].threshold)
                    {
                        _mourningDays += 4;
                    }
                    else if (_value >= PlayManager.data.friendshipLevels[3].threshold)
                    {
                        _mourningDays += 3;
                    }
                    else if (_value >= PlayManager.data.friendshipLevels[2].threshold)
                    {
                        _mourningDays += 2;
                    }
                    else if (_value >= PlayManager.data.friendshipLevels[1].threshold)
                    {
                        _mourningDays += 1;
                    }
                }
                int _daysDelay = PlayManager.day - _s.dayOfDeath;
                if (_daysDelay <= _mourningDays) _malus += (_mourningDays - _daysDelay);
            }
        }

        attackBonuses[0] -= _malus;
        attackBonuses[1] -= _malus;
        attackBonuses[2] -= _malus;
        defenseBonuses[0] -= _malus;
        defenseBonuses[1] -= _malus;
        defenseBonuses[2] -= _malus;
        defenseBonuses[3] -= _malus;
        speedBonus -= 5*_malus;
    }

    /// <summary>
    /// ComputeFriendship method compute the friendship value from the selected squad minus the selected soldier to change
    /// </summary>
    /// <param name="_squad">Selected squad (Squad)</param>
    /// <param name="_selectedSoldier">Selected Soldier (Soldier)</param>
    /// <returns></returns>
    public int ComputeFriendship(Squad _squad, Soldier _selectedSoldier)
    {
        // If no squad, this is an error so return default value
        if (_squad == null) return 0;

        // Init friendship value at 0 then test for each soldier of the squa d(except the selected one) in which friendship category it is
        friendshipValue = 0;
        foreach (Soldier _s in _squad.Soldiers)
        {
            if (_s == _selectedSoldier || _s == this || _s == null) continue;
            if (friendship.TryGetValue(_s.iD, out int _value))
            {
                if (_value >= PlayManager.data.friendshipLevels[4].threshold)
                {
                    friendshipValue += 4;
                }
                else if (_value >= PlayManager.data.friendshipLevels[3].threshold)
                {
                    friendshipValue += 3;
                }
                else if (_value >= PlayManager.data.friendshipLevels[2].threshold)
                {
                    friendshipValue += 2;
                }
                else if (_value >= PlayManager.data.friendshipLevels[1].threshold)
                {
                    friendshipValue += 1;
                }
            }
        }
        return friendshipValue;
    }
    #endregion

    #region Sort methods

    /// <summary>
    /// SortByID method compares two soldiers and returns an int depending of soldiers'ID
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByID(Soldier x, Soldier y)
    {
        if(x == null)
        {
            if(y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return x.ID.CompareTo(y.ID);
    }

    /// <summary>
    /// SortByName method compares two soldiers and returns an int depending of soldiers'Name
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByName(Soldier x, Soldier y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return x.Name.CompareTo(y.Name);
    }

    /// <summary>
    /// SortByType method compares two soldiers and returns an int depending of soldiers'Type
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByType(Soldier x, Soldier y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        int _temp = x.data.soldierType.CompareTo(y.data.soldierType);
        if(_temp == 0)
        {
            return SortByLevelGreater(x, y);
        }
        else
        {
            return _temp;
        }
    }

    /// <summary>
    /// SortByLevelLower method compares two soldiers and returns an int depending of soldiers'Level
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByLevelLower(Soldier x, Soldier y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return x.data.soldierLevel.CompareTo(y.data.soldierLevel);
    }

    /// <summary>
    /// SortByLevelGreater method compares two soldiers and returns an int depending of soldiers'Level
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>1 if lower, 0 if equal and -1 if greater (x compare to y)</returns>
    public static int SortByLevelGreater(Soldier x, Soldier y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return (-1)*x.data.soldierLevel.CompareTo(y.data.soldierLevel);
    }

    /// <summary>
    /// SortBySquad method compares two soldiers and returns an int depending of soldiers'Squad
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>1 if lower, 0 if equal and -1 if greater (x compare to y)</returns>
    public static int SortBySquad(Soldier x, Soldier y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        if(x.squad == null)
        {
            if(y.squad == null) return 0;
            return 1;
        }
        if (y.squad == null) return 1;

        return x.squad.ID.CompareTo(y.squad.ID);
    }

    /// <summary>
    /// SortByFriendship method compares two soldiers and returns an int depending of soldiers'Friendship value
    /// </summary>
    /// <param name="x">First soldier</param>
    /// <param name="y">Second soldier</param>
    /// <returns>-1 if lower, 0 if equal and 1 if greater (x compare to y)</returns>
    public static int SortByFriendship(Soldier x, Soldier y)
    {
        if (x == null)
        {
            if (y == null) return 0; // If both are null, they are equals
            return 1; // x is null so it is greater than y
        }
        if (y == null) return -1; // y is null so x is lower than y

        return (-1)*x.friendshipValue.CompareTo(y.friendshipValue);
    }
    #endregion
}
