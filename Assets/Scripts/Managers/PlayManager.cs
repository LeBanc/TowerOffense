using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// PlayManager is the manager of the "Play" gamestate
/// It lists all the data needed in Play mode and the basic actions in this play mode
/// </summary>
public class PlayManager : Singleton<PlayManager>
{
    //User defined Game Data
    public GameData gameData;
    public NavMeshSurface soldierNavMeshSurface;
    public NavMeshSurface squadNavMeshSurface;

    // Play mode properties
    public static GameData data;
    public static NavMeshSurface soldierNav;
    public static NavMeshSurface squadNav;

    // Level properties
    public static HQ hq;
    public static DayLight dayLight;
    public static Vector3 hqPos;
    public static List<Tower> towerList = new List<Tower>();
    public static List<GameObject> buildingList = new List<GameObject>();
    public static List<Turret> turretList = new List<Turret>();
    public static List<TurretBase> turretBaseList = new List<TurretBase>();
    public static List<HQCandidate> hqCandidateList = new List<HQCandidate>();

    // HQ properties
    public static int workforce;
    public static int day;
    public static int recruitment;
    public static bool newSoldier;
    public static int nextSoldierID;
    public static int nextSquadID;
    public static SquadData defaultSquadType;
    public static List<Squad> squadList = new List<Squad>();
    public static List<Soldier> soldierList = new List<Soldier>();
    public static List<int> soldierIDList = new List<int>();

    // Attack properties
    public static List<SquadUnit> squadUnitList;
    public static List<EnemySoldier> enemyList = new List<EnemySoldier>();
    public static List<Explosives> explosivesList = new List<Explosives>();
    private static int attackXP;
    private static int attackWorkforce;

    // Facilities properties
    public static int attackTimeLevel = 0;
    public static int healLevel = 0;
    public static int recruitmentLevel = 0;
    public static int explosivesLevel = 0;
    public static int recruitingWithXP = 0;

    #region Properties access
    public static float LongRange
    {
        get{return data.longRange ;}
    }

    public static float MiddleRange
    {
        get { return data.middleRange; }
    }

    public static float ShortRange
    {
        get { return data.shortRange; }
    }

    public static GameObject SquadPrefab
    {
        get { return data.squadPrefab; }
    }
    #endregion

    #region Events
    public delegate void PlayManagerEvents();
    public static event PlayManagerEvents OnLoadSquadsOnNewDay;
    public static event PlayManagerEvents OnRetreatAll;
    public static event PlayManagerEvents OnEndDay;
    public static event PlayManagerEvents OnHQPhase;
    public static event PlayManagerEvents OnWorkforceUpdate;
    public static event PlayManagerEvents OnLoadGame;
    public static event PlayManagerEvents OnRecruit;
    public static event PlayManagerEvents OnReset;

    public delegate HealthBar PlayManagerHealthBarEvents(Transform _t, float _maxW);
    public static event PlayManagerHealthBarEvents OnNewHealthBarAdded;

    /// <summary>
    /// NewDay event is used to launch a new day and instantiate the squads
    /// </summary>
    public void NewDayButton()
    {
        bool engagedSquad = false;
        // If an engaged squad is not full, display an error message and stop
        foreach(Squad _squad in squadList)
        {
            // Check if the squad is engaged and add this data to the engagedSquad boolean
            engagedSquad = engagedSquad || _squad.isEngaged;

            if(_squad.isEngaged && !_squad.IsFull())
            {
                UIManager.InitErrorMessage("At least one of the engaged squads has not four soldiers assigned!");
                return;
            }
        }

        // Check if there is a squad engaged for the new day
        if(!engagedSquad)
        {
            UIManager.InitErrorMessage("You need to engage at least on squad!");
            return;
        }

        // Get how many doctor stayed in the HQ and add 1 to the heal amount for each one of them
        int med = 0;
        foreach(Soldier _s in soldierList)
        {
            foreach (SoldierData.Capacities _c in _s.Data.capacities)
            {
                if (_c.Equals(SoldierData.Capacities.Heal))
                {
                    med += 1;
                    break;
                }
            }
        }

        // Set the heal amount as the base heal amount + the medics bonus and the infirmary bonus (3 per level).
        hq.HealAmount = data.baseHealAmount + data.facilities.healBonus * healLevel + med;
        // Set the attack time as base attack time + unlocked bons
        float attackTime = data.baseAttackTime + attackTimeLevel * data.facilities.timeBonus;
        hq.AttackTime = attackTime;
        // Initialize sun light at morning
        dayLight.Morning(attackTime);

        // Launch a new day
        OnLoadSquadsOnNewDay?.Invoke();
        attackXP = 0;
        attackWorkforce = 0;
    }

    /// <summary>
    /// RetreatButton methods is meant to be called by an UI button to retreat all SquadUnits at the same time
    /// </summary>
    public void RetreatButton()
    {
        EndOfAttack();
    }
    #endregion

    /// <summary>
    /// EndOfAttack (by user call or by time), calls the RetreatAll event, set the game speed to normal and increment the day number
    /// </summary>
    public static void EndOfAttack()
    {
        OnRetreatAll?.Invoke();
        NormalSpeed();
    }

    /// <summary>
    /// RemoveSquadUnit method deletes a SquadUnit from the City
    /// </summary>
    /// <param name="_su">SquadUnit to remove</param>
    public static void RemoveSquadUnit(SquadUnit _su)
    {
        squadUnitList.Remove(_su);
        if(squadUnitList.Count <= 0)
        {
            // Increment day count
            day++;

            // End the day and switch to HQ phase
            Instance.EndDayRoutine();
        }
    }

    private void EndDayRoutine()
    {
        // Set End of day for all City phase listeners
        OnEndDay?.Invoke();

        // Add recruitment chances and reset the enemy list
        recruitment += enemyList.Count;
        recruitment++;
        RecruitTest();
        enemyList.Clear();

        // Compute PlayManager data
        SetXP();
        UpdateWorkforce(attackWorkforce);

        // Switch UI from City phase to HQ phase
        OnHQPhase?.Invoke();

        // Autosave
        AutoSaveGame();

    }

    static void RecruitTest()
    {
        int _value = Random.Range(0, 100);
        if (_value <= recruitment)
        {
            newSoldier = true;
            OnRecruit?.Invoke();
            Debug.Log("New Soldier at " + recruitment + "% chances");
            recruitment = data.baseRecruitAmount;
        }
    }

    /// <summary>
    /// Instantiate PlayManager singleton and load game data
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Data initialization
        data = gameData;
        soldierNav = soldierNavMeshSurface;
        squadNav = squadNavMeshSurface;

        // Default value
        nextSoldierID = 0;
        nextSquadID = 0;
        defaultSquadType = data.defaultSquadType;

        // Lists initialization
        towerList = new List<Tower>();
        //towerPosList = new List<Vector3>();
        buildingList = new List<GameObject>();
        //buildingPosList = new List<Vector3>();
        squadList = new List<Squad>();
        soldierList = new List<Soldier>();
        soldierIDList = new List<int>();
        squadUnitList = new List<SquadUnit>();
    }

    /// <summary>
    /// LoadFromEmptyScene loads the data from an empty scene
    /// </summary>
    public static void LoadFromEmptyScene()
    {
        OnReset?.Invoke();

        // Load DayLight
        GameObject _dayLight = GameObject.Find("Directional Day Light");
        if (_dayLight == null)
        {
            Debug.LogError("[PlayManager] Cannot find DayLight GameObject!");
        }
        else
        {
            dayLight = _dayLight.GetComponent<DayLight>();
        }

        // Load HQ
        GameObject _hq = GameObject.Find("HQ");
        if (_hq == null)
        {
            Debug.LogError("[PlayManager] Cannot find HQ GameObject!");
        }
        else
        {
            hq = _hq.GetComponent<HQ>();
            hqPos = GridAdjustment.GetGridCoordinates(new Vector3(hq.transform.position.x, 0f, hq.transform.position.z));
        }

        // Clear data
        soldierList.Clear();
        soldierIDList.Clear();
        squadList.Clear();
        towerList.Clear();
        buildingList.Clear();
        turretList.Clear();
        turretBaseList.Clear();
        hqCandidateList.Clear();
        nextSoldierID = 0;
        nextSquadID = 0;
    }

    /// <summary>
    /// LoadFromScene method gets all the data needed by the PlayManager from the scene (loaded or created scene for new game)
    /// The method is part loading from scene, part creating default elements
    /// </summary>
    public static void LoadFromScene()
    {       
        // Load towers
        Tower[] _tArray = FindObjectsOfType<Tower>();
        if(_tArray.Length > 0)
        {
            foreach (Tower _t in _tArray) towerList.Add(_t);
        }
        else
        {
            Debug.LogError("[PlayManager] Cannot find Tower GameObjects!");
        }

        // Load buildings
        GameObject _build = GameObject.Find("Buildings");
        if (_build == null)
        {
            Debug.LogError("[PlayManager] Cannot find Buildings GameObject!");
        }
        else
        {
            for(int i=0;i < _build.transform.childCount; i++)
            {
                buildingList.Add(_build.transform.GetChild(i).gameObject);
            }
        }

        // Init HQ data
        day = 1;
        workforce = 0;
        recruitment = data.baseRecruitAmount;
        healLevel = 0;
        attackTimeLevel = 0;
        recruitmentLevel = 0;
        explosivesLevel = 0;
        recruitingWithXP = 0;

        // Create soldiers
        for (int i = 0; i < 8; i++)
        {
            soldierIDList.Add(nextSoldierID);
            Soldier _soldier = ScriptableObject.CreateInstance("Soldier") as Soldier;
            _soldier.InitData(Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData);
            soldierList.Add(_soldier);
        }
        // Create 1st squad and adding soldiers to it
        Squad _squad = ScriptableObject.CreateInstance("Squad") as Squad;
        _squad.InitData();
        squadList.Add(_squad);
        squadList[0].ChangeSoldier(1, soldierList[0]);
        squadList[0].ChangeSoldier(2, soldierList[1]);
        squadList[0].ChangeSoldier(3, soldierList[2]);
        squadList[0].ChangeSoldier(4, soldierList[3]);
        squadList[0].isEngaged = true;
    }

    /// <summary>
    /// SetNewHQPosition method changes the destination of all retreating SquadUnit
    /// </summary>
    public static void SetNewHQPosition()
    {
        // Set new HQ spawn points, they are needed to set a new destination for all squad
        hq.SetSpawnPoints();

        foreach(SquadUnit _su in squadUnitList)
        {
            if (_su.IsRetreating) _su.Retreat(); // Calling retreat again will change the squad destination
        }
    }

    /// <summary>
    /// GetFreeSoldierID returns the current free soldier ID and increments it
    /// </summary>
    /// <returns>Soldier ID</returns>
    public static int GetFreeSoldierID()
    {
        int _result = nextSoldierID;
        nextSoldierID++;
        return _result;
    }

    /// <summary>
    /// GetFreeSquadID returns the current free squad ID and increments it
    /// </summary>
    /// <returns>Squad ID</returns>
    public static int GetFreeSquadID()
    {
        int _result = nextSquadID;
        nextSquadID++;
        return _result;
    }

    /// <summary>
    /// GetSquadColor returns the color defined for the squad ID (0 to 3)
    /// </summary>
    /// <param name="number">squad number</param>
    /// <returns>Color for the defined squad</returns>
    public static Color GetSquadColor(int number)
    {
        return data.squadColors[number];
    }

    /// <summary>
    /// GetRandomSoldierImage returns a random image from the soldier images bank
    /// </summary>
    /// <returns>Soldier sprite</returns>
    public static Sprite GetRandomSoldierImage()
    {
        return data.soldierImages[Random.Range(0, data.soldierImages.Count - 1)];
    }

    /// <summary>
    /// GetRandomSoldierName returns a random name from the soldier names bank
    /// </summary>
    /// <returns>Soldier name</returns>
    public static string GetRandomSoldierName()
    {
        return string.Concat(data.soldierFirstNames[Random.Range(0,data.soldierFirstNames.Count-1)], " ", data.soldierLastNames[Random.Range(0, data.soldierLastNames.Count - 1)]);
    }

    /// <summary>
    /// AddHealthBar methods creates a HealthBar over a Transform
    /// </summary>
    /// <param name="_t">Transform over which the HealthBar will be displayed</param>
    /// <param name="_maxW">Max width of the HealthBar (in pxl)</param>
    /// <returns>Created HealthBar</returns>
    public static HealthBar AddHealthBar(Transform _t, float _maxW = 50f)
    {
        // This has to changed once the UIManager is set up
        return OnNewHealthBarAdded?.Invoke(_t, _maxW);
        //return playUI.cityCanvas.AddHealthBar(_t, _maxW);
    }

    /// <summary>
    /// Reduce the game speed (slow motion effect)
    /// </summary>
    public static void SlowSpeed()
    {
        // There is a boolean because the method is called by events and can be activated with false value
        Time.timeScale = 0.5f;
    }

    /// <summary>
    /// Return to a speed of 1x
    /// </summary>
    public static void NormalSpeed()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// AddXP method adds to attackXP the amount of XP to add
    /// </summary>
    /// <param name="_xp">Amount to add</param>
    public static void AddXP(int _xp)
    {
        attackXP += _xp;
    }

    /// <summary>
    /// SetXP method distributes XP to all living soldiers and extra XP to soldiers that attacked the last round
    /// </summary>
    private void SetXP()
    {
        foreach(Soldier _soldier in soldierList)
        {
            if (!_soldier.IsDead && _soldier.MaxXP != 0)
            {
                _soldier.CurrentXP += attackXP / 2;
                if(_soldier.Squad != null)
                {
                    if (_soldier.Squad.isEngaged) _soldier.CurrentXP += attackXP / 2 + attackXP % 2;
                }
            }
        }
    }

    /// <summary>
    /// AddAttackWorkforce method adds to attackCoins the amount of coins to add
    /// </summary>
    /// <param name="_amount">Amount to add</param>
    public static void AddAttackWorkforce(int _amount)
    {
        attackWorkforce += _amount;
    }

    /// <summary>
    /// UpdateWorkforce method adds the amount of worforce to the main workforce value
    /// </summary>
    /// <param name="_amount">Amount to add</param>
    public static void UpdateWorkforce(int _amount)
    {
        workforce += _amount;
        OnWorkforceUpdate?.Invoke();
    }

    /// <summary>
    /// AutoSaveGame method saves the Game data into AutoSave.xml file
    /// </summary>
    public void AutoSaveGame()
    {
        // Autosave file if not in WebGL mode
#if !UNITY_WEBGL
        StartCoroutine(AutoSaveSequence());
#endif
    }

    /// <summary>
    /// AutoSaveSequence is a coroutine to make an autosave and display the save status
    /// </summary>
    IEnumerator AutoSaveSequence()
    {
        float startTime = Time.time;
        GameManager.ChangeGameStateRequest(GameManager.GameState.save);
        new DataSave().AutoSaveGame();
        while (Time.time < startTime + 1f)
        {
            yield return null;
        }
        GameManager.ChangeGameStateRequest(GameManager.GameState.play);
    }

    /// <summary>
    /// InitAfterLoad loads calls and event to init all things needed to be init
    /// </summary>
    public static void InitAfterLoad()
    {
        PlayManager.soldierNav.BuildNavMesh();
        PlayManager.squadNav.BuildNavMesh();

        OnLoadGame?.Invoke();
        OnHQPhase?.Invoke();
    }
}
