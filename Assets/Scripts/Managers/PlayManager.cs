﻿using System.Collections;
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
    public static float gameSpeed = 1f;

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
    public static bool isRecruiting;
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

    // To display dead soldier
    public static List<Soldier> deadSoldier = new List<Soldier>();

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
    public static event PlayManagerEvents OnNewDayConfirm;
    public static event PlayManagerEvents OnLoadSquadsOnNewDay;
    public static event PlayManagerEvents OnRetreatAll;
    public static event PlayManagerEvents OnEndDay;
    public static event PlayManagerEvents OnHQPhase;
    public static event PlayManagerEvents OnWorkforceUpdate;
    public static event PlayManagerEvents OnLoadGame;
    public static event PlayManagerEvents OnNewGame;
    public static event PlayManagerEvents OnRecruit;
    public static event PlayManagerEvents OnReset;

    public delegate HealthBar PlayManagerHealthBarEvents(Transform _t, float _maxW);
    public static event PlayManagerHealthBarEvents OnNewHealthBarAdded;

    #endregion

    /// <summary>
    /// NewDayButton method is used to open the confirmation Canvas to launch a new day
    /// </summary>
    public void NewDayButton()
    {
        bool engagedSquad = false;
        // If an engaged squad is not full, display an error message and stop
        foreach (Squad _squad in squadList)
        {
            // Check if the squad is engaged and add this data to the engagedSquad boolean
            engagedSquad = engagedSquad || _squad.isEngaged;
        }
        // Check if there is a squad engaged for the new day
        if(!engagedSquad)
        {
            UIManager.InitErrorMessage("You need to engage at least on squad!");
            return;
        }

        // Call the event to show the confirm Canvas
        OnNewDayConfirm?.Invoke();
    }

    /// <summary>
    /// NewDayLaunchAttack method is used to launch a new day and instantiate the squads (after confirmation)
    /// </summary>
    public static void NewDayLaunchAttack()
    {
        // Reset engagement of all soldiers (in case of error at the previous NewDay)
        foreach (Soldier _s in soldierList)
        {
            _s.IsEngaged = false;
        }


        bool engagedSquad = false;
        // If an engaged squad is not full, display an error message and stop
        foreach (Squad _squad in squadList)
        {
            // Check if the squad is engaged and add this data to the engagedSquad boolean
            engagedSquad = engagedSquad || _squad.isEngaged;

            if (_squad.isEngaged)
            {
                if (!_squad.IsFull())
                {
                    UIManager.InitErrorMessage("At least one of the engaged squads has not four soldiers assigned!");
                    return;
                }
                else
                {
                    foreach (Soldier _soldier in _squad.Soldiers)
                    {
                        _soldier.IsEngaged = true;
                    }
                }
            }
        }

        // Get how many doctor stayed in the HQ and add 1 to the heal amount for each one of them
        int _med = 0;
        foreach (Soldier _s in soldierList)
        {
            if (_s.IsEngaged) continue;

            foreach (SoldierData.Capacities _c in _s.Data.capacities)
            {
                if (_c.Equals(SoldierData.Capacities.Heal))
                {
                    _med += 1;
                    break;
                }
            }
        }

        // Get the infirmary bonus
        int _healBonus = ((healLevel >= 1) ? data.facilities.heal1Bonus : 0) + ((healLevel >= 2) ? data.facilities.heal2Bonus : 0) + ((healLevel >= 3) ? data.facilities.heal3Bonus : 0);

        // Set the heal amount as the base heal amount + the medics bonus and the infirmary bonus.
        hq.HealAmount = data.baseHealAmount + _healBonus + _med;

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

    /// <summary>
    /// EndDayRoutine method is called at the end of the day and before the UI initialization
    /// </summary>
    private void EndDayRoutine()
    {
        // Set End of day for all City phase listeners
        OnEndDay?.Invoke();

        // Add recruitment chances and reset the enemy list
        recruitment += enemyList.Count;
        recruitment++;

        // Recruitment test
        int _value = Random.Range(0, 100);
        if (_value <= recruitment)
        {
            isRecruiting = true;
            // Canvas call by event is no more done here to avoid null event system error
            recruitment = data.baseRecruitAmount + ((recruitmentLevel >= 1) ? data.facilities.recruiting1Bonus : 0) + ((recruitmentLevel >= 2) ? data.facilities.recruiting2Bonus : 0) + ((recruitmentLevel >= 3) ? data.facilities.recruiting3Bonus : 0);
        }

        // Clear the enemy list
        enemyList.Clear();

        // Compute PlayManager data
        SetXP();
        UpdateWorkforce(attackWorkforce);

        // Call the End day at HQ to heal soldiers, this method will then call SwitchToHQPhase to continue the end day routine
        hq.EndDayAtHQ();
    }

    /// <summary>
    /// SwitchToHQPhase method is called by HQ when all the end day computation are done
    /// </summary>
    public static void SwitchToHQPhase()
    {
        // Switch UI from City phase to HQ phase
        OnHQPhase?.Invoke();

        // Test if recruiting to display the right Canvas
        if (isRecruiting)
        {
            OnRecruit?.Invoke();
        }
        else // Autosave only if not recruiting (otherwise this will make 2 saves for the same day as the game is saving when the recruit is accepted or dismissed)
        {
            Instance.AutoSaveGame();
        }         
        isRecruiting = false;

        // Display fallen soldiers (after recruiting because the Error Canvas is above the other Canvas)
        switch (deadSoldier.Count)
        {
            case 0:
                break;
            case 1:
                UIManager.InitErrorMessage("The soldiers of the fallen squad have been retrieved but " + deadSoldier[0].Data.typeName + " " + deadSoldier[0].Name + " has succumbed to his/her injuries!", deadSoldier[0].Image);
                break;
            case 2:
                UIManager.InitErrorMessage("The soldiers of the fallen squads have been retrieved but " + deadSoldier[0].Data.typeName + " " + deadSoldier[0].Name + " and 1 other soldier have succumbed to their injuries!", deadSoldier[0].Image);
                break;
            default:
                UIManager.InitErrorMessage("The soldiers of the fallen squads have been retrieved but " + deadSoldier[0].Data.typeName + " " + deadSoldier[0].Name + " and " + (deadSoldier.Count - 1) + " other soldiers have succumbed to their injuries!", deadSoldier[0].Image);
                break;
        }
        deadSoldier.Clear();

    }

    /// <summary>
    /// At Awake, instantiates PlayManager singleton, load game data and subscribe to GameManager events
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

        GameManager.OnPlayToPause += PauseSpeed;
        GameManager.OnPauseToPlay += ResumeSpeed;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        GameManager.OnPlayToPause -= PauseSpeed;
        GameManager.OnPauseToPlay -= ResumeSpeed;
        base.OnDestroy();
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

        // Clear data from PlayManager (scriptable objects data)
        soldierList.Clear();
        soldierIDList.Clear();
        squadList.Clear();

        // Clear data from scene (static)
        towerList.Clear();
        buildingList.Clear();
        turretList.Clear();
        turretBaseList.Clear();
        hqCandidateList.Clear();
        
        // Clear data from scene (dynamic)
        foreach(EnemySoldier _enemy in enemyList)
        {
            if(_enemy != null) Destroy(_enemy.gameObject);
        }
        enemyList.Clear();
        foreach (SquadUnit _squadUnit in squadUnitList)
        {
            if(_squadUnit!= null) Destroy(_squadUnit.gameObject);
        }
        squadUnitList.Clear();
        foreach (Explosives _explosives in explosivesList)
        {
            if(_explosives != null) Destroy(_explosives.gameObject);
        }
        explosivesList.Clear();

        // Clear global PlayManager data
        nextSoldierID = 0;
        nextSquadID = 0;
        gameSpeed = 1f;

        NormalSpeed();
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
        for (int i = 0; i < data.baseSoldierInitNumber; i++)
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
    /// ResetGame static method calls OnReset event to reset game data
    /// </summary>
    public static void ResetGame()
    {
        OnReset?.Invoke();
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
        List<string> firstNames = (Random.Range(0f, 1f) > 0.5f)?data.soldierNames.femaleFirstNamesList:data.soldierNames.maleFirstNamesList;
        return string.Concat(firstNames[Random.Range(0,firstNames.Count-1)], " ", data.soldierNames.lastNamesList[Random.Range(0, data.soldierNames.lastNamesList.Count - 1)]);
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
        Time.timeScale = 0.3f;
    }

    /// <summary>
    /// Return to a speed of 1x
    /// </summary>
    public static void NormalSpeed()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Pause the game and register the current game speed
    /// </summary>
    private void PauseSpeed()
    {
        gameSpeed = Time.timeScale;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Resume the game by setting the last game speed
    /// </summary>
    private void ResumeSpeed()
    {
        Time.timeScale = gameSpeed;
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
                //Debug.Log("Soldier " + _soldier.ID + " at " + _soldier.CurrentXP + " XP");
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
    public static void InitAfterLoad(bool _newGame)
    {
        PlayManager.soldierNav.BuildNavMesh();
        PlayManager.squadNav.BuildNavMesh();

        OnLoadGame?.Invoke();
        OnHQPhase?.Invoke();

        if (_newGame) OnNewGame?.Invoke();
    }
}
