using System.Collections.Generic;
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
    private bool isDead = false;

    // Bonuses that can be applied to the soldier
    private int[] attackBonuses = new int[3];
    private int[] defenseBonuses = new int[4];
    private int speedBonus;

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

    public bool IsDead
    {
        get { return isDead; }
    }

    public bool IsEngaged
    {
        set { isEngaged = value; }
        get { return isEngaged; }
    }

    public int ShortRangeAttack
    {
        get { return data.shortRangeAttack + BonusAtkShortRange; }
    }

    public int MiddleRangeAttack
    {
        get { return data.middleRangeAttack + BonusAtkMidRange; }
    }

    public int LongRangeAttack
    {
        get { return data.longRangeAttack + BonusAtkLongRange; }
    }

    public int ShortRangeDefense
    {
        get { return data.shortRangeDefense + BonusDefShortRange; }
    }

    public int MiddleRangeDefense
    {
        get { return data.middleRangeDefense + BonusDefMidRange; }
    }

    public int LongRangeDefense
    {
        get { return data.longRangeDefense + BonusDefLongRange; }
    }

    public int ExplosivesDefense
    {
        get { return data.explosiveDefense + BonusDefExplosives; }
    }

    public int Speed
    {
        get { return data.speed + BonusSpeed; }
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
    public void LoadData(int _ID, string _NAME, string _IMAGE, string _DATA, int _HP, int _XP, int[] _friendshipArray)
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
        isDead = true;
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

        return x.data.soldierType.CompareTo(y.data.soldierType);
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
    #endregion
}
