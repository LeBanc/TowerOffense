using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : Singleton<PlayManager>
{
    //User defined Game Data
    public GameData gameData;

    // Play mode properties
    public static GameData data;
    
    // Level properties
    public static HQ hq;
    public static Vector3 hqPos;
    public static List<Tower> towerList;
    public static List<Vector3> towerPosList;
    public static List<GameObject> buildingList;
    public static List<Vector3> buildingPosList;

    // HQ properties
    public static int credits;
    public static int nextSoldierID;
    public static int nextSquadID;
    public static SquadData defaultSquadType;
    public static List<Squad> squadList;
    public static List<Soldier> soldierList;
    public static List<int> soldierIDList;

    // Attack properties
    public static List<SquadUnit> squadUnitList;

    private int infirmaryLevel = 0;
    private bool infirmary1;
    private bool infirmary2;
    private bool infirmary3;
    private bool infirmary4;
    private bool infirmary5;

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
        FindObjectOfType<CityCanvas>().enabled = true;

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

        // Launch a new day
        LoadSquadsOnNewDay?.Invoke();
    }

    public void RetreatButton()
    {
        EndOfAttack();
    }
    #endregion

    public static void EndOfAttack()
    {
        RetreatAll?.Invoke();
        FindObjectOfType<CityCanvas>().UnselectSquads();
        NormalSpeed();
        FindObjectOfType<CityCanvas>().enabled = false;
    }

    public static void RemoveSquadUnit(SquadUnit _su)
    {
        squadUnitList.Remove(_su);
        if(squadUnitList.Count <= 0)
        {
            // Switch to HQ
            hq.EndDayAtHQ();
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

        Load();
    }

    /// <summary>
    /// Load method actually gets all the data needed by the PlayManager from scene
    /// </summary>
    public void Load()
    {
        // Load Level
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
        // Load squads
        GameObject _squadHolder = GameObject.Find("SquadHolder");
        if (_squadHolder == null)
        {
            Debug.LogError("[PlayManager] Cannot find SquadHolder GameObject!");
        }
        else
        {
            foreach (Squad _squad in _squadHolder.transform.GetComponentsInChildren<Squad>())
            {
                squadList.Add(_squad);
            }
        }        
        // Load soldiers
        GameObject _soldierHolder = GameObject.Find("SoldierHolder");
        if (_soldierHolder == null)
        {
            Debug.LogError("[PlayManager] Cannot find SoldierHolder GameObject!");
        }
        else
        {
            foreach (Soldier _soldier in _soldierHolder.transform.GetComponentsInChildren<Soldier>())
            {
                soldierList.Add(_soldier);
                soldierIDList.Add(_soldier.ID);
            }
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

    public static HealthBar AddHealthBar(Transform _t, float _maxW = 50f)
    {
        return FindObjectOfType<CityCanvas>().AddHealthBar(_t, _maxW);
    }

    public static void SlowSpeed(bool isSelected)
    {
        if(isSelected) Time.timeScale = 0.5f;
    }

    public static void NormalSpeed()
    {
        Time.timeScale = 1f;
    }
}
