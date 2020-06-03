using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayManager is the manager of the "Play" gamestate
/// It lists all the data needed in Play mode and the basic actions in this play mode
/// </summary>
public class PlayManager : Singleton<PlayManager>
{
    //User defined Game Data
    public GameData gameData;

    // Play mode properties
    public static GameData data;
    
    // Level properties
    public static HQ hq;
    public static DayLight dayLight;
    public static Vector3 hqPos;
    public static List<Tower> towerList = new List<Tower>();
    public static List<Vector3> towerPosList = new List<Vector3>();
    public static List<GameObject> buildingList = new List<GameObject>();
    public static List<Vector3> buildingPosList = new List<Vector3>();

    // HQ properties
    public static int coins;
    public static int day;
    public static int nextSoldierID;
    public static int nextSquadID;
    public static SquadData defaultSquadType;
    public static List<Squad> squadList = new List<Squad>();
    public static List<Soldier> soldierList = new List<Soldier>();
    public static List<int> soldierIDList = new List<int>();

    // Attack properties
    public static List<SquadUnit> squadUnitList;

    public static int infirmaryLevel = 0;

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
    public static event PlayManagerEvents LoadSquadsOnNewDay;
    public static event PlayManagerEvents RetreatAll;

    /// <summary>
    /// NewDay event is used to launch a new day and instantiate the squads
    /// </summary>
    public void NewDayButton()
    {
        // At the attack launch, hide HQ canvas and show City canvas
        FindObjectOfType<PlayUI>().ShowHQCanvas(false);
        FindObjectOfType<PlayUI>().ShowCityCanvas(true);

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
        hq.HealAmount = data.baseHealAmount + 3 * infirmaryLevel + med;
        // Set the attack time as base attack time + unlocked bons (TBD)
        hq.AttackTime = data.baseAttackTime;
        // Initialize sun light at morning
        dayLight.Morning(data.baseAttackTime);

        // Launch a new day
        LoadSquadsOnNewDay?.Invoke();
        day++;
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
        RetreatAll?.Invoke();
        NormalSpeed();
        day++;
    }

    public static void RemoveSquadUnit(SquadUnit _su)
    {
        squadUnitList.Remove(_su);
        if(squadUnitList.Count <= 0)
        {
            // Switch to HQ
            hq.EndDayAtHQ();
            dayLight.Night();
            FindObjectOfType<PlayUI>().ShowHQCanvas(true);
            FindObjectOfType<PlayUI>().ShowCityCanvas(false);
            FindObjectOfType<PlayUI>().ResetCityCanvas();
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
        nextSoldierID = 0;
        nextSquadID = 0;
        defaultSquadType = data.defaultSquadType;


        // Lists initialization
        towerList = new List<Tower>();
        towerPosList = new List<Vector3>();
        buildingList = new List<GameObject>();
        buildingPosList = new List<Vector3>();
        squadList = new List<Squad>();
        soldierList = new List<Soldier>();
        soldierIDList = new List<int>();
        squadUnitList = new List<SquadUnit>();

        LoadFromScene();
    }

    /// <summary>
    /// Load method actually gets all the data needed by the PlayManager from scene
    /// The method is part loading from scene, part creating default elements
    /// This is still a work in progress
    /// </summary>
    public void LoadFromScene()
    {
        // Load Level
        day = 0;
        coins = 1000;
        // Load HQ
        GameObject _hq = GameObject.Find("HQ");
        if (_hq == null)
        {
            Debug.LogError("[PlayManager] Cannot find HQ GameObject!");
        }
        else
        {
            hq = _hq.GetComponent<HQ>();
            hqPos = GridAdjustment.GetGridCoordinates(new Vector3(hq.transform.position.x,0f,hq.transform.position.z));
        }
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


        // Load HQ data
        // Create soldiers
        for (int i = 0; i < 5; i++)
        {
            soldierIDList.Add(nextSoldierID);
            soldierList.Add(new Soldier(Resources.Load("SoldierData/0_Basic_SoldierData") as SoldierData));
        }
        // Create 1st squad and adding soldiers to it
        squadList.Add(new Squad());
        squadList[0].ChangeSoldier(1, soldierList[0]);
        squadList[0].ChangeSoldier(2, soldierList[1]);
        squadList[0].ChangeSoldier(3, soldierList[2]);
        squadList[0].ChangeSoldier(4, soldierList[3]);
        squadList[0].isEngaged = true;

        FindObjectOfType<PlayUI>().ShowHQCanvas(true);
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
        return FindObjectOfType<PlayUI>().cityCanvas.AddHealthBar(_t, _maxW);
    }

    /// <summary>
    /// Reduce the game speed (slow motion effect)
    /// </summary>
    /// <param name="isSelected">Bool to enable slow speed</param>
    public static void SlowSpeed(bool isSelected)
    {
        // There is a boolean because the method is called by events and can be activated with false value
        if(isSelected) Time.timeScale = 0.5f;
    }

    /// <summary>
    /// Return to a speed of 1x
    /// </summary>
    public static void NormalSpeed()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// AutoSaveGame method saves the Game data into AutoSave.xml file
    /// </summary>
    public void AutoSaveGame()
    {
        new DataSave().AutoSaveGame();
    }

    /// <summary>
    /// LoadAutoSaveData loads the Game data from AutoSave.xml file
    /// </summary>
    public void LoadAutoSavedData()
    {
        LoadDataSave(DataSave.LoadAutoSavedGame());
    }

    /// <summary>
    /// LoadDataSave loads GameData from a DataSave
    /// </summary>
    /// <param name="_save">DataSave to load data from</param>
    private void LoadDataSave(DataSave _save)
    {
        // Global data
        coins = _save.coins;
        day = _save.day;
        infirmaryLevel = _save.infirmary;

        // Load Soldiers
        soldierList.Clear();
        soldierIDList.Clear();
        foreach (SoldierSave _soldierSave in _save.soldierList)
        {
            Soldier _soldier = _soldierSave.LoadSoldier();
            soldierList.Add(_soldier);
            soldierIDList.Add(_soldier.ID);
        }
        nextSoldierID = soldierList.Count;

        // Load Squads
        squadList.Clear();
        foreach (SquadSave _squadSave in _save.squadList)
        {
            Squad _squad = _squadSave.LoadSquad();
            squadList.Add(_squad);
        }
        nextSquadID = squadList.Count;

        FindObjectOfType<PlayUI>().UpdateHQCanvas();
    }
}
