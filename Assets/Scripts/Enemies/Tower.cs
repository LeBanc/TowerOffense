using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower class is the class of a Tower
/// </summary>
public class Tower : Enemy
{
    // Prefab of soldier to spawn
    public GameObject enemySoldier;
    public GameObject activeFlag;

    public TowerData data;
    private string dataPath = "";

    private bool isSet;

    private float spawnDelayRandom;
    private float spawnCounter = 0f;
    private int soldierHP;
    private int soldierAttack;
    private int soldierDefense;

     // Tower available cells in ranges
    private List<Vector3> shortRangeCells;
    private List<Vector3> middleRangeCells;
    private List<Vector3> longRangeCells;

    /// <summary>
    /// On Start, initialize HealthBar, events and available cells
    /// </summary>
    protected override void Start()
    {
        // Initialize Tower data
        if (!isSet) Setup();

        // Subscribe to events
        PlayManager.OnLoadSquadsOnNewDay += EnableUpdate;
        PlayManager.OnEndDay += DisableUpdate;
        PlayManager.OnRetreatAll += StopSpawning;

        shortRangeCells = new List<Vector3>();
        middleRangeCells = new List<Vector3>();
        longRangeCells = new List<Vector3>();
    }

    /// <summary>
    /// OnDestroy, unsubscribe from all events
    /// </summary>
    protected override void OnDestroy()
    {
        // Disable update and spawning
        DisableUpdate();
        // Unsubscribe to events
        PlayManager.OnLoadSquadsOnNewDay -= EnableUpdate;
        PlayManager.OnEndDay -= DisableUpdate;
        PlayManager.OnRetreatAll -= StopSpawning;
        base.OnDestroy();
    }

    /// <summary>
    /// Setup method is used to setup all enemy and tower classes data from TowerData
    /// </summary>
    public override void Setup()
    {
        base.Setup();

        dataPath = data.name;

        active = false;
        activeFlag.SetActive(false);

        maxHP = data.maxHP;
        hP = maxHP;
        // Attack values
        shortRangeAtk = data.shortRangeAttack;
        middleRangeAtk = data.middleRangeAttack;
        longRangeAtk = data.longRangeAttack;
        explosiveAtk = data.explosiveAttack;
        // Defense values
        shortRangeDef = data.shortRangeDefense;
        middleRangeDef = data.middleRangeDefense;
        longRangeDef = data.longRangeDefense;
        explosiveDef = data.explosiveDefense;

        // Delays
        shootingDataDuration = data.shootingDelay;
        // Set a random spawn delay around the fixed one
        spawnDelayRandom = Random.Range(data.spawnDelay - 5f, data.spawnDelay + 5f);

        // Compute the soldier data
        soldierHP = maxHP / 5;
        soldierAttack = Mathf.FloorToInt(Mathf.Max(1, Mathf.Max(shortRangeAtk, middleRangeAtk, longRangeAtk)/10));
        soldierDefense = Mathf.FloorToInt(Mathf.Max(0,(Mathf.Max(shortRangeDef, middleRangeDef, longRangeDef)/2)));

        //SFX
        fireSFX.clip = data.shootingSound;

        // Initialize health bar
        SetupHealthBar();
        healthBar.Hide();

        // Set the Tower as already setup
        isSet = true;
    }


    /// <summary>
    /// LoadData method changes the tower data and updates its value
    /// </summary>
    /// <param name="_dataPath">Path to the tower data to load</param>
    /// <param name="_hp">Current tower HP</param>
    /// <param name="_active">Current tower active status</param>
    public void LoadData(string _dataPath, int _hp, bool _active)
    {
        data = Resources.Load("TowerData/" + _dataPath) as TowerData;
        OnHPDown -= DestroyEnemy;
        Setup();
        hP = _hp;
        if (_active) Activate(); ;
    }

    /// <summary>
    /// Activate method activates the tower and do anything linked to the activation (GFX, SFX, etc.)
    /// </summary>
    private void Activate()
    {
        active = true;
        // GFX
        healthBar.Show();
        activeFlag.SetActive(true);

        // SFX
    }

    /// <summary>
    /// DestroyEnemy method sets the enemy as destroyed and triggers FX
    /// </summary>
    protected override void DestroyEnemy()
    {
        base.DestroyEnemy();

        // Deactive tower
        active = false;
        activeFlag.SetActive(false);

        StopSpawning();

        // Remove tower from tower list
        PlayManager.towerList.Remove(this);

        // GFX
        HQ.InstantiateHQCandidate(transform.position);
        //GetComponent<MeshRenderer>().material.color = Color.black;

        // SFX

        Destroy(gameObject);
    }

    /// <summary>
    /// EnableUpdate method subscribes to the PlayUpdate event and resets counters
    /// </summary>
    protected override void EnableUpdate()
    {
        base.EnableUpdate();
        spawnCounter = 0f;
        GameManager.PlayUpdate += SpawnUpdate;
        InitializeAvailableCells(); // Init cells at each day to avoid error at init when loading
        healthBar.UpdateValue(hP, maxHP);
    }

    /// <summary>
    /// DisableUpdate method unsubscribes from the PlayUpdate event
    /// </summary>
    protected virtual void DisableUpdate()
    {
        // Unsubscribe to events
        GameManager.PlayUpdate -= EnemyUpdate;
        StopSpawning();
    }

    /// <summary>
    /// StopSpawning method is called by event to stop spawning soldiers when squad are all retreating
    /// </summary>
    private void StopSpawning()
    {
        GameManager.PlayUpdate -= SpawnUpdate;
    }

    /// <summary>
    /// EnemyUpdate is the Update methods of the Enemy
    /// </summary>
    protected override void EnemyUpdate()
    {
        if (destroyed) return;
        Shootable _target = Ranges.GetNearestSoldier(this, shortRangeAtk, middleRangeAtk, longRangeAtk);
        if (_target != selectedTarget) SetTarget(_target);

        if (!active && !destroyed && selectedTarget != null) Activate();
        if (selectedTarget != null) Shoot(selectedTarget);
        shootingDelay = Mathf.Max(0f, shootingDelay - Time.deltaTime);

        // For debug purpose
        // Checks if a target is selected and draws a line to it
        if (selectedTarget != null)
        {
            Vector3[] _positions = new Vector3[] { transform.position, selectedTarget.transform.position };
        }
    }

    /// <summary>
    /// SpawnUpdate is an update specific for spawning soldiers. It starts with the EnemyUpdate but stops with RetreatAll event
    /// </summary>
    private void SpawnUpdate()
    {
        if (destroyed) return;

        // Spawn enemy soldier every spawnDelay +- 5 seconds
        if (active) spawnCounter += Time.deltaTime;
        if (spawnCounter >= spawnDelayRandom)
        {
            SpawnSoldier();
            spawnCounter = 0f;
            spawnDelayRandom = Random.Range(data.spawnDelay - 5f, data.spawnDelay + 5f);
        }
    }

    /// <summary>
    /// InitializeAvailableCells method gets the empty cells (no building) around the tower
    /// </summary>
    private void InitializeAvailableCells()
    {
        shortRangeCells.Clear();
        middleRangeCells.Clear();
        LongRangeCells.Clear();

        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                if (i == 0 && j == 0) continue;

                Vector3 _cell = new Vector3((float)10 * i, 0f, (float)10 * j);

                // Check if cell is empty (Raycast from above hits the terrain)
                RaycastHit _hitPosition;
                Ray _ray = new Ray(GridAdjustment.GetGridCoordinates(transform.position) + _cell + 100 * Vector3.up, -Vector3.up);
                if (Physics.Raycast(_ray, out _hitPosition, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain", "Buildings", "Enemies", "Soldiers" })))
                {
                    if (_hitPosition.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {

                        // If cell is empty, check if it is in sight
                        RaycastHit _hitSight = new RaycastHit();
                        bool isInSight = false;
                        foreach(Transform _t in hitList)
                        {
                            _ray = new Ray(_t.position, (_hitPosition.point - _t.position).normalized);
                            if (!Physics.Raycast(_ray, out _hitSight, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Enemies", "Soldiers" })))
                            {
                                isInSight = true;
                                break;
                            }                                
                        }

                        // If the cell is empty and in sight, add it to the cells' list
                        if (isInSight)
                        {
                            if (_cell.magnitude <= (PlayManager.ShortRange + 5f)) shortRangeCells.Add(GridAdjustment.GetGridCoordinates(transform.position + _cell));
                            else if (_cell.magnitude <= (PlayManager.MiddleRange + 5f)) middleRangeCells.Add(GridAdjustment.GetGridCoordinates(transform.position + _cell));
                            else if (_cell.magnitude <= (PlayManager.LongRange + 5f)) longRangeCells.Add(GridAdjustment.GetGridCoordinates(transform.position + _cell));
                        }
                    }
                }
            }
        }
    }

    public List<Vector3> ShortRangeCells
    {
        get { return shortRangeCells; }
    }
    public List<Vector3> MiddleRangeCells
    {
        get { return middleRangeCells; }
    }
    public List<Vector3> LongRangeCells
    {
        get { return longRangeCells; }
    }

    /// <summary>
    /// SpawnSoldier spawns an enemy soldier at the first available cells around the tower
    /// </summary>
    public void SpawnSoldier()
    {
        // Get all the available cells around the tower
        List<Vector3> _cells = new List<Vector3>();
        _cells.AddRange(shortRangeCells);
        _cells.AddRange(middleRangeCells);
        _cells.AddRange(longRangeCells);

        // Remove a cell from the list if a squad unit is on it or go at it
        foreach(SquadUnit _squad in PlayManager.squadUnitList)
        {
            _cells.Remove(GridAdjustment.GetGridCoordinates(_squad.transform.position));
            _cells.Remove(GridAdjustment.GetGridCoordinates(_squad.Destination));
        }

        // Remove a cell from the list if a turret or a turret base is on it
        foreach(Turret _turret in PlayManager.turretList)
        {
            _cells.Remove(GridAdjustment.GetGridCoordinates(_turret.transform.position));
        }
        foreach(TurretBase _turretBase in PlayManager.turretBaseList)
        {
            _cells.Remove(GridAdjustment.GetGridCoordinates(_turretBase.transform.position));
        }

        // Remove a cell from the list if there is already a soldier on it
        foreach(EnemySoldier _enemy in PlayManager.enemyList)
        {
            _cells.Remove(GridAdjustment.GetGridCoordinates(_enemy.transform.position));
        }

        // If the list is not empty, get the first cell and spawn a new soldier at it
        if(_cells.Count > 0)
        {
            GameObject _soldierInstance = Instantiate(enemySoldier, _cells[0], Quaternion.identity);
            _soldierInstance.GetComponent<EnemySoldier>().Setup(soldierHP, soldierAttack, soldierDefense);
        }
    }
}
