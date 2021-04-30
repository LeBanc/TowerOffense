using UnityEngine;

/// <summary>
/// Squad is the class that represents the squad with all its data
/// </summary>
public class Squad : ScriptableObject
{
    // PreferedRange is the range at which the squad will attack if it is possible
    public enum PreferedRange
    {
        ShortRange,
        MiddleRange,
        LongRange
    }

    // PositionChoice enum is the choice given to the player to define the computing of PreferedRange
    public enum PositionChoice
    {
        MaximizeAttack,
        MaximizeDefense,
        PlayerChoice
    }

    // If the squad is engaged, it will be deploy in the city
    public bool isEngaged = false;

    // iD is a private iD to identify the squad
    private int iD;
    // color is the background color of the squad used in UI and game
    private Color color;
    // squad type is the SquadData that defines the shape of the squad
    private SquadData squadType;
    // soldierLisr is the array of 4 soldiers composing the squad
    private Soldier[] soldierList = new Soldier[4];
    // preferedRange is the current range of the squad
    private PreferedRange range;
    // posChoice is the current choice of position computing of the squad
    private PositionChoice posChoice;

    // Attack, defense and speed value of the squad
    private int atkShortRange = 0;
    private int atkMiddleRange = 0;
    private int atkLongRange = 0;
    private int defShortRange = 0;
    private int defMiddleRange = 0;
    private int defLongRange = 0;
    private int defExplosives = 0;
    private int speed = 0;

    // Squad events
    public delegate void SquadEventHandler();
    public event SquadEventHandler OnTypeChange; // When changing SquadData
    public event SquadEventHandler OnColorChange; // When changing color
    public event SquadEventHandler OnSoldier1Change; // When changing soldier1
    public event SquadEventHandler OnSoldier2Change; // When changing soldier2
    public event SquadEventHandler OnSoldier3Change; // When changing soldier3
    public event SquadEventHandler OnSoldier4Change; // When changing soldier4
    public event SquadEventHandler OnValueChange; // When changing any attack, defense or speed value
    public event SquadEventHandler OnPrefRangeChange; // When changing the preferred range
    public event SquadEventHandler OnEngageChange; // When changing the engage value

    #region Properties access
    public int ID
    {
        get { return iD; }
    }

    public Color Color
    {
        get { return color; }
    }

    public SquadData SquadType
    {
        get { return squadType; }
    }

    public PositionChoice PosChoice
    {
        set { posChoice = value; }
        get { return posChoice; }
    }

    public PreferedRange PrefRange
    {
        set { range = value; }
        get { return range; }
    }

    public Soldier[] Soldiers
    {
        get { return soldierList; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public int[] AttackValues
    {
        get { return new int[] { atkShortRange, atkMiddleRange, atkLongRange }; }
    }

    public int[] DefenseValues
    {
        get { return new int[] { defShortRange, defMiddleRange, defLongRange, defExplosives }; }
    }
    #endregion

    /// <summary>
    /// LoadData sets the squad data from dedicated data (save file)
    /// </summary>
    /// <param name="_ID">Squad iD</param>
    /// <param name="_COLOR">Squad color</param>
    /// <param name="_SQUADTYPE">Squad SqaudData</param>
    /// <param name="_SOLDIER1ID">Soldier1</param>
    /// <param name="_SOLDIER2ID">Soldier2</param>
    /// <param name="_SOLDIER3ID">Soldier3</param>
    /// <param name="_SOLDIER4ID">Soldier4</param>
    /// <param name="_RANGE">Squad preferred range</param>
    /// <param name="_POS">Squad position choice</param>
    /// <param name="_ENGAGED">Squad engaged boolean</param>
    public void LoadData(int _ID, Color _COLOR, string _SQUADTYPE, int _SOLDIER1ID, int _SOLDIER2ID, int _SOLDIER3ID, int _SOLDIER4ID, PreferedRange _RANGE, PositionChoice _POS, bool _ENGAGED)
    {
        iD = _ID;
        color = _COLOR;
        squadType = Resources.Load("SquadData/" + _SQUADTYPE) as SquadData;
        soldierList[0] = null;
        soldierList[1] = null;
        soldierList[2] = null;
        soldierList[3] = null;
        if (_SOLDIER1ID > -1)ChangeSoldier(1, PlayManager.soldierList[PlayManager.soldierIDList.FindIndex(x => x == _SOLDIER1ID)]);
        if (_SOLDIER2ID > -1) ChangeSoldier(2, PlayManager.soldierList[PlayManager.soldierIDList.FindIndex(x => x == _SOLDIER2ID)]);
        if (_SOLDIER3ID > -1) ChangeSoldier(3, PlayManager.soldierList[PlayManager.soldierIDList.FindIndex(x => x == _SOLDIER3ID)]);
        if (_SOLDIER4ID > -1) ChangeSoldier(4, PlayManager.soldierList[PlayManager.soldierIDList.FindIndex(x => x == _SOLDIER4ID)]);
        range = _RANGE;
        ComputeSquadValues();
        UpdatePosChoice(_POS);
        isEngaged = _ENGAGED;
    }

    /// <summary>
    /// At Awake, compute preferred range from chosen position
    /// </summary>
    private void Awake()
    {
        UpdatePosChoice(posChoice);
    }

    /// <summary>
    /// InitData method sets up the squad data with default value
    /// </summary>
    public void InitData()
    {
        iD = PlayManager.GetFreeSquadID();
        color = PlayManager.GetSquadColor(iD);
        squadType = PlayManager.defaultSquadType;
        UpdatePosChoice(PositionChoice.MaximizeAttack);
        range = PreferedRange.MiddleRange;
    }

    /// <summary>
    /// Engage method sets the isEngaged boolean
    /// </summary>
    /// <param name="_engage">Boolean to set isEngaged to</param>
    public void Engage(bool _engage)
    {
        isEngaged = _engage;
        OnEngageChange?.Invoke();
    }

    /// <summary>
    /// ChangeSquadType method sets the new SquadData and invoke OnTypeChange event
    /// </summary>
    /// <param name="_newType">SquadData to set squadType to</param>
    public void ChangeSquadType(SquadData _newType)
    {
        squadType = _newType;
        OnTypeChange?.Invoke();
    }

    /// <summary>
    /// ChangeColor method sets the new squad color and invoke OnColorChange event
    /// </summary>
    /// <param name="_color">Color to set squad color to</param>
    public void ChangeColor(Color _color)
    {
        color = _color;
        OnColorChange?.Invoke();
    }

    /// <summary>
    /// UpdatePosChoice method sets the new player choice and compute the preferred range and invoke OnPrefRangeChange event
    /// </summary>
    /// <param name="_newChoice">PositionChoice to set</param>
    public void UpdatePosChoice(PositionChoice _newChoice)
    {
        // Set the new choice
        posChoice = _newChoice;
        // Compute the preferred range from the new choice
        switch(posChoice)
        {
            case PositionChoice.MaximizeAttack:
                range = GetMaxAttackRange();
                break;
            case PositionChoice.MaximizeDefense:
                range = GetMaxDefenseRange();
                break;
            case PositionChoice.PlayerChoice:
                break;
            default:
                break;
        }
        // Invoke event
        OnPrefRangeChange?.Invoke();
    }

    /// <summary>
    /// GetMaxAttackRange method compute the preferred range from the attack values
    /// </summary>
    /// <returns>PreferedRange that maximize attack</returns>
    private PreferedRange GetMaxAttackRange()
    {
        if (atkLongRange >= atkMiddleRange && atkLongRange >= atkShortRange) return PreferedRange.LongRange;
        else if (atkMiddleRange >= atkShortRange) return PreferedRange.MiddleRange;
        else return PreferedRange.ShortRange;
    }

    /// <summary>
    /// GetMaxDefenseRange method compute the preferred range from the defense values
    /// </summary>
    /// <returns>PreferedRange that maximize defense</returns>
    private PreferedRange GetMaxDefenseRange()
    {
        if (defLongRange >= defMiddleRange && defLongRange >= defShortRange) return PreferedRange.LongRange;
        else if (defMiddleRange >= defShortRange) return PreferedRange.MiddleRange;
        else return PreferedRange.ShortRange;
    }

    /// <summary>
    /// GetSoldierIndex method searches if the soldier is part of the squad and return its index
    /// </summary>
    /// <param name="_soldier">Soldier to find</param>
    /// <returns>Index at which the soldier is, 0 if not in the squad</returns>
    public int GetSoldierIndex(Soldier _soldier)
    {
        if (_soldier == soldierList[0]) return 1;
        if (_soldier == soldierList[1]) return 2;
        if (_soldier == soldierList[2]) return 3;
        if (_soldier == soldierList[3]) return 4;
        return 0;
    }
    
    /// <summary>
    /// ChangeSoldier method update a soldier of the squad and invoke the right event
    /// </summary>
    /// <param name="_number">Number of the soldier to change</param>
    /// <param name="_soldier">Soldier to link to the squad</param>
    public void ChangeSoldier(int _number, Soldier _soldier)
    {
        switch (_number)
        {
            case 1: // Change soldier 1
                // If the slot is not null, removes the attached soldier from it
                if(soldierList[0] != null) soldierList[0].RemoveFromSquad();
                if(_soldier != null)
                {
                    // Remove the selected soldier to its previous squad
                    _soldier.RemoveFromSquad();
                    // Add the soldier to this squad
                    soldierList[0] = _soldier;
                    soldierList[0].AddToSquad(this);
                    // Add soldier events for refreshing squad data
                    soldierList[0].OnNameChange += RefreshSquad;
                    soldierList[0].OnImageChange += RefreshSquad;
                    soldierList[0].OnDataChange += RefreshSquad;
                }
                OnSoldier1Change?.Invoke();
                break;
            case 2: // Change soldier 2
                if (soldierList[1] != null) soldierList[1].RemoveFromSquad();
                if(_soldier != null)
                {
                    _soldier.RemoveFromSquad();
                    soldierList[1] = _soldier;
                    soldierList[1].AddToSquad(this);
                    // Add soldier events for refreshing squad data
                    soldierList[1].OnNameChange += RefreshSquad;
                    soldierList[1].OnImageChange += RefreshSquad;
                    soldierList[1].OnDataChange += RefreshSquad;
                }
                OnSoldier2Change?.Invoke();
                break;
            case 3: // Change soldier 3
                if (soldierList[2] != null) soldierList[2].RemoveFromSquad();
                if(_soldier != null)
                {
                    _soldier.RemoveFromSquad();
                    soldierList[2] = _soldier;
                    soldierList[2].AddToSquad(this);
                    // Add soldier events for refreshing squad data
                    soldierList[2].OnNameChange += RefreshSquad;
                    soldierList[2].OnImageChange += RefreshSquad;
                    soldierList[2].OnDataChange += RefreshSquad;
                }
                OnSoldier3Change?.Invoke();
                break;
            case 4: // Change soldier 4
                if (soldierList[3] != null) soldierList[3].RemoveFromSquad();
                if(_soldier != null)
                {
                    _soldier.RemoveFromSquad();
                    soldierList[3] = _soldier;
                    soldierList[3].AddToSquad(this);
                    // Add soldier events for refreshing squad data
                    soldierList[3].OnNameChange += RefreshSquad;
                    soldierList[3].OnImageChange += RefreshSquad;
                    soldierList[3].OnDataChange += RefreshSquad;
                }
                OnSoldier4Change?.Invoke();
                break;
            default: // if _number is not between 1 to 4 (inclusive), display an error
                Debug.LogError("[Squad] Trying to change a soldier not between 1st and 4th position (inclusive)");
                break;
        }
        // Compute new value after soldier change
        ComputeSquadValues();
        // Update position choice (to compute new preferred range id needed)
        UpdatePosChoice(posChoice);
    }

    /// <summary>
    /// RemoveSoldier removes a dedicated soldier of the squad
    /// </summary>
    /// <param name="_toRemove">Soldier to remove</param>
    public void RemoveSoldier(Soldier _toRemove)
    {
        switch (GetSoldierIndex(_toRemove))
        {
            case 1:
                soldierList[0] = null;
                OnSoldier1Change?.Invoke();
                break;
            case 2:
                soldierList[1] = null;
                OnSoldier2Change?.Invoke();
                break;
            case 3:
                soldierList[2] = null;
                OnSoldier3Change?.Invoke();
                break;
            case 4:
                soldierList[3] = null;
                OnSoldier4Change?.Invoke();
                break;
            default:
                break;
        }

        // Remove soldier events for refreshing squad data
        _toRemove.OnNameChange -= RefreshSquad;
        _toRemove.OnImageChange -= RefreshSquad;
        _toRemove.OnDataChange -= RefreshSquad;
    }

    /// <summary>
    /// RefreshSquad method shall be used to refresh squad data after soldier level up
    /// </summary>
    public void RefreshSquad()
    {
        OnSoldier1Change?.Invoke();
        OnSoldier2Change?.Invoke();
        OnSoldier3Change?.Invoke();
        OnSoldier4Change?.Invoke();
        // Compute new value after soldier change
        ComputeSquadValues();
        // Update position choice (to compute new preferred range id needed)
        UpdatePosChoice(posChoice);
    }

    public void ComputeBonuses()
    {
        foreach(Soldier _s in soldierList)
        {
            if (_s != null) _s.ComputeBonuses(this);
        }
    }

    /// <summary>
    /// ComputeSquadValues method computes all the squad data from the soldier data and invoke OnValueChange event
    /// </summary>
    public void ComputeSquadValues()
    {
        // Compute attack data
        atkShortRange = 0;
        atkShortRange += soldierList[0] == null ? 0 : Mathf.Max(0,(soldierList[0].ShortRangeAttack + soldierList[0].BonusAtkShortRange));
        atkShortRange += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].ShortRangeAttack + soldierList[1].BonusAtkShortRange));
        atkShortRange += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].ShortRangeAttack + soldierList[2].BonusAtkShortRange));
        atkShortRange += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].ShortRangeAttack + soldierList[3].BonusAtkShortRange));

        atkMiddleRange = 0;
        atkMiddleRange += soldierList[0] == null ? 0 : Mathf.Max(0, (soldierList[0].MiddleRangeAttack + soldierList[0].BonusAtkMidRange));
        atkMiddleRange += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].MiddleRangeAttack + soldierList[1].BonusAtkMidRange));
        atkMiddleRange += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].MiddleRangeAttack + soldierList[2].BonusAtkMidRange));
        atkMiddleRange += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].MiddleRangeAttack + soldierList[3].BonusAtkMidRange));

        atkLongRange = 0;
        atkLongRange += soldierList[0] == null ? 0 : Mathf.Max(0, (soldierList[0].LongRangeAttack + soldierList[0].BonusAtkLongRange));
        atkLongRange += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].LongRangeAttack + soldierList[1].BonusAtkLongRange));
        atkLongRange += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].LongRangeAttack + soldierList[2].BonusAtkLongRange));
        atkLongRange += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].LongRangeAttack + soldierList[3].BonusAtkLongRange));

        // Compute defense data
        defShortRange = 0;
        defShortRange += soldierList[0] == null ? 0 : Mathf.Max(0, (soldierList[0].ShortRangeDefense + soldierList[0].BonusDefShortRange));
        defShortRange += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].ShortRangeDefense + soldierList[1].BonusDefShortRange));
        defShortRange += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].ShortRangeDefense + soldierList[2].BonusDefShortRange));
        defShortRange += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].ShortRangeDefense + soldierList[3].BonusDefShortRange));

        defMiddleRange = 0;
        defMiddleRange += soldierList[0] == null ? 0 : Mathf.Max(0, (soldierList[0].MiddleRangeDefense + soldierList[0].BonusDefMidRange));
        defMiddleRange += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].MiddleRangeDefense + soldierList[1].BonusDefMidRange));
        defMiddleRange += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].MiddleRangeDefense + soldierList[2].BonusDefMidRange));
        defMiddleRange += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].MiddleRangeDefense + soldierList[3].BonusDefMidRange));

        defLongRange = 0;
        defLongRange += soldierList[0] == null ? 0 : Mathf.Max(0, (soldierList[0].LongRangeDefense + soldierList[0].BonusDefLongRange));
        defLongRange += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].LongRangeDefense + soldierList[1].BonusDefLongRange));
        defLongRange += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].LongRangeDefense + soldierList[2].BonusDefLongRange));
        defLongRange += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].LongRangeDefense + soldierList[3].BonusDefLongRange));

        defExplosives = 0;
        defExplosives += soldierList[0] == null ? 0 : Mathf.Max(0, (soldierList[0].ExplosivesDefense + soldierList[0].BonusDefExplosives));
        defExplosives += soldierList[1] == null ? 0 : Mathf.Max(0, (soldierList[1].ExplosivesDefense + soldierList[1].BonusDefExplosives));
        defExplosives += soldierList[2] == null ? 0 : Mathf.Max(0, (soldierList[2].ExplosivesDefense + soldierList[2].BonusDefExplosives));
        defExplosives += soldierList[3] == null ? 0 : Mathf.Max(0, (soldierList[3].ExplosivesDefense + soldierList[3].BonusDefExplosives));

        // Compute speed
        UpdateSpeed();

        // Invoke event
        OnValueChange?.Invoke();
    }

    /// <summary>
    /// UpdateSpeed method updates the squad speed value from the soldier value
    /// </summary>
    public void UpdateSpeed()
    {
        speed = 999;
        foreach (Soldier _soldier in soldierList)
        {
            // The squad speed is the minimum speed of all soldiers
            if (_soldier != null)
            {
                speed = Mathf.Min(speed, Mathf.Max(40, _soldier.Speed + _soldier.BonusSpeed));
            }
        }
        if (speed == 999) speed = 0;
    }

    /// <summary>
    /// InstantiateSquadUnit method instantiate a SquadUnit in the City Scene from Squad data
    /// </summary>
    /// <param name="spawPoint">Vector3 where to spawn the SquadUnit</param>
    /// <returns>The SquadUnit created</returns>
    public SquadUnit InstanciateSquadUnit(Vector3 spawPoint)
    {
        // Instantiate SquadUnit at spawnPoint
        GameObject _go = Instantiate(PlayManager.SquadPrefab, GridAdjustment.GetGridCoordinates(spawPoint), Quaternion.identity);
        SquadUnit _su = _go.GetComponent<SquadUnit>();
        // Setup SquadUnit with Squad
        _su.Setup(this);
        // Add Squad to CityCanvas => To be changed when updating UIManager
        FindObjectOfType<CityCanvas>().AddSquad(_su);
        return _su;
    }

    /// <summary>
    /// IsFull method returns true if the Squad has 4 soldiers attached to it and false otherwise
    /// </summary>
    /// <returns>True if full</returns>
    public bool IsFull()
    {
        return (soldierList[0] != null && soldierList[1] != null && soldierList[2] != null && soldierList[3] != null);
    }

    /// <summary>
    /// ComputeFriendshipPoints method adds friendship points to soldier of the squad
    /// </summary>
    public void ComputeFriendshipPoints()
    {
        soldierList[0].AddFriendshipPoint(soldierList[1].ID);
        soldierList[0].AddFriendshipPoint(soldierList[2].ID);
        soldierList[0].AddFriendshipPoint(soldierList[3].ID);

        soldierList[1].AddFriendshipPoint(soldierList[0].ID);
        soldierList[1].AddFriendshipPoint(soldierList[2].ID);
        soldierList[1].AddFriendshipPoint(soldierList[3].ID);

        soldierList[2].AddFriendshipPoint(soldierList[0].ID);
        soldierList[2].AddFriendshipPoint(soldierList[1].ID);
        soldierList[2].AddFriendshipPoint(soldierList[3].ID);

        soldierList[3].AddFriendshipPoint(soldierList[0].ID);
        soldierList[3].AddFriendshipPoint(soldierList[1].ID);
        soldierList[3].AddFriendshipPoint(soldierList[2].ID);
    }

    /// <summary>
    /// ComputeSoldierDeath method randomly choose one soldier of the squad and sets it as dead
    /// </summary>
    public void ComputeSoldierDeath()
    {
        int _r = Random.Range(0, 4);
        soldierList[_r].Die();
        PlayManager.deadSoldier.Add(soldierList[_r]);
        RemoveSoldier(soldierList[_r]);        
    }
}
