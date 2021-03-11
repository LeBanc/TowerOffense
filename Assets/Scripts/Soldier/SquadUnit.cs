using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// SquadUnit is the class that represents a Squad in an attack session
/// </summary>
public class SquadUnit : MonoBehaviour
{
    // Squad and squad data
    private Squad squad;
    private SquadData squadData;

    // Basic components
    private Camera mainCamera;
    private NavMeshAgent navAgent;
    
    // Soldier components
    private SoldierUnit soldier1;
    private SoldierUnit soldier2;
    private SoldierUnit soldier3;
    private SoldierUnit soldier4;

    // Target components
    private Enemy target;
    private bool protectionStance = false;
    private float maxStopRange;
    private HQCandidate hqCandidate;

    // Destination components
    private bool fixedDestination = false;
    private bool retreatActive = false;

    // Play routine parameters
    // private bool isSelected = false;

    // Events
    public delegate void SquadTargetEventHandler(Enemy _target);
    public event SquadTargetEventHandler OnTargetChange;

    public delegate void SquadUnitEventHandler();
    public event SquadUnitEventHandler OnUnselection;
    public event SquadUnitEventHandler OnDeath;
    public event SquadUnitEventHandler OnHQBack;
    public event SquadUnitEventHandler OnActionDone;

    #region Properties access
    public Vector3 Destination
    {
        get { return navAgent.destination; }
    }
    public int ID
    {
        get { return squad.ID; }
    }
    public bool IsRetreating
    {
        get { return retreatActive; }
    }

    public SoldierUnit Soldier1
    {
        get { return soldier1; }
    }
    public SoldierUnit Soldier2
    {
        get { return soldier2; }
    }
    public SoldierUnit Soldier3
    {
        get { return soldier3; }
    }
    public SoldierUnit Soldier4
    {
        get { return soldier4; }
    }
    public float Speed
    {
        set { navAgent.speed = value; }
        get { return navAgent.speed; }
    }

    public Squad Squad
    {
        get { return squad; }
    }

    #endregion

    /// <summary>
    /// Setup method initializes the SquadUnit from Squad data
    /// </summary>
    /// <param name="_squad">Squad represented by this SquadUnit</param>
    public void Setup(Squad _squad)
    {
        // Get main Camera
        mainCamera = Camera.main;

        // Set squad, squadData and maxStopRange
        squad = _squad;
        squadData = squad.SquadType;
        switch (squad.PrefRange)
        {
            case Squad.PreferedRange.LongRange:
                maxStopRange = PlayManager.LongRange;
                break;
            case Squad.PreferedRange.MiddleRange:
                maxStopRange = PlayManager.MiddleRange;
                break;
            case Squad.PreferedRange.ShortRange:
                maxStopRange = PlayManager.ShortRange;
                break;
            default:
                maxStopRange = PlayManager.MiddleRange;
                break;
        }

        // Set the squad floor color
        //transform.GetChild(0).GetComponent<MeshRenderer>().material.color = squad.Color; // For Standard
        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", squad.Color); // For URP

        // Get navAgent, enables it and set its speed
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;
        navAgent.speed = squad.Speed * navAgent.speed / 100;

        // Create SoldierUnits from Soldiers in the Squad
        soldier1 = CreateSoldier("Soldier1", squadData.soldier1Position + 1.3f * Vector3.up, squad.Soldiers[0]);
        soldier2 = CreateSoldier("Soldier2", squadData.soldier2Position + 1.3f * Vector3.up, squad.Soldiers[1]);
        soldier3 = CreateSoldier("Soldier3", squadData.soldier3Position + 1.3f * Vector3.up, squad.Soldiers[2]);
        soldier4 = CreateSoldier("Soldier4", squadData.soldier4Position + 1.3f * Vector3.up, squad.Soldiers[3]);

        // Link update events
        GameManager.PlayUpdate += SquadUpdate;
        PlayManager.OnReset += ResetSquadUnit;
    }

    /// <summary>
    /// On destroy, destoys soldiers and clear event links
    /// </summary>
    private void OnDestroy()
    {
        // Unlinks event
        GameManager.PlayUpdate -= SquadUpdate;
        PlayManager.OnReset -= ResetSquadUnit;
        OnUnselection = null;
        OnActionDone = null;
    }

    /// <summary>
    /// CreateSoldier methods creates a soldier ans returns its Soldier component
    /// </summary>
    /// <param name="_name">Name of the gameobject</param>
    /// <param name="_position">Relative position where to instanciate the soldier</param>
    /// <param name="_soldier">Soldier from which instantiate the SoldierUnit</param>
    /// <returns></returns>
    private SoldierUnit CreateSoldier(string _name, Vector3 _position, Soldier _soldier)
    {
        // Set the soldier as engaged (used to heal soldier not engaged during day)
        _soldier.IsEngaged = true;

        // Creates a gameObject from prefab and changes its name
        GameObject _soldierGO = Instantiate(_soldier.Data.prefab, transform.position + _position, transform.rotation, transform);
        _soldierGO.name = _name;

        // Gets the Soldier component and calls its setup if found
        SoldierUnit _instance = _soldierGO.GetComponent<SoldierUnit>();
        if (_instance == null)
        {
            Debug.LogError("[Squad] Trying to instantiate GameObject that is not a Soldier");
        }
        else
        {
            _instance.Setup(this, _soldier);
        }

        return _instance;
    }

    /// <summary>
    /// Unselct method unselects the SquadUnit and call the OnUnselection event
    /// </summary>
    public void Unselect()
    {
        OnUnselection?.Invoke();
        //OnActionDone = null;
    }

    /// <summary>
    /// OnActionSelected method is used to enable or disable the SquadUnselection method when the Squad is selected
    /// </summary>
    /// <param name="_isOn"></param>
    public void OnActionSelected(bool _isOn)
    {
        if(_isOn)
        {
            GameManager.PlayUpdate += SquadUnselection;
        }
        else
        {
            GameManager.PlayUpdate -= SquadUnselection;
        }
    }

    /// <summary>
    /// OnMoveActionSelected method trigged the Move action of this SquadUnit
    /// </summary>
    public void OnMoveActionSelected(bool _isOn)
    {
        if(_isOn)
        {
            GameManager.PlayUpdate += SquadMoveSelection;
        }
        else
        {
            GameManager.PlayUpdate -= SquadMoveSelection;
        }
    }

    /// <summary>
    /// OnBuildHQSelected method trigged the BuildHQ action of this SquadUnit
    /// </summary>
    public void OnBuildHQSelected(bool _isOn)
    {
        if(_isOn)
        {
            GameManager.PlayUpdate += SquadBuildHQSelection;
        }
        else
        {
            GameManager.PlayUpdate -= SquadBuildHQSelection;
        }
    }

    /// <summary>
    /// OnBuildTurretSelected method trigged the BuildTurret action of this SquadUnit
    /// </summary>
    public void OnBuildTurretSelected(bool _isOn)
    {
        if (_isOn)
        {
            GameManager.PlayUpdate += SquadBuildTurretSelection;
        }
        else
        {
            GameManager.PlayUpdate -= SquadBuildTurretSelection;
        }
    }

    /// <summary>
    /// OnExplosivesSelected method trigged the Explosive action of this SquadUnit
    /// </summary>
    public void OnExplosivesSelected(bool _isOn)
    {
        if (_isOn)
        {
            GameManager.PlayUpdate += SquadBuildExplosivesSelection;
        }
        else
        {
            GameManager.PlayUpdate -= SquadBuildExplosivesSelection;
        }
    }

    /// <summary>
    /// Heal method heals the SoldierUnit of the SquadUnit and unselect the SquadUnit
    /// </summary>
    public void Heal()
    {
        Soldier1.Heal(25);
        Soldier2.Heal(25);
        Soldier3.Heal(25);
        Soldier4.Heal(25);

        Unselect();
    }

    #region Target
    /// <summary>
    /// SetTarget method set the target of the SquadUnit
    /// </summary>
    /// <param name="_t">Enemy that will be the new target</param>
    public void SetTarget(Enemy _t)
    {
        // Clear the current target if there is one
        if (target != null) ClearTarget();
        // Set the new target and invoke the change event
        target = _t;
        OnTargetChange?.Invoke(target);
        // If the new target is not null, subscribe to its OnDestruction event to clear it when it is detroyed
        if (target != null)
        {
            target.OnDestruction += ClearTarget;
        }
    }

    /// <summary>
    /// ClearTarget method clears the current target
    /// </summary>
    public void ClearTarget()
    {
        // If there is a target, unsubscribe to its OnDestruction event
        if(target!=null) target.OnDestruction -= ClearTarget;
        // Set the target to null
        target = null;
    }

    /// <summary>
    /// FaceTarget method is used to rotate the squad toward its target
    /// </summary>
    /// <param name="_target">Current target (Enemy)</param>
    private void FaceTarget(Enemy _target)
    {
        // Gets the projection on XZ plane of the vector between target and squad positions
        Vector3 _diff = _target.transform.position - transform.position;
        _diff = new Vector3(_diff.x, 0f, _diff.z);

        // Gets the angle between the _diff vector and the forward squad axe
        float _angle = Vector3.SignedAngle(transform.forward, Vector3.Normalize(_diff), transform.up);
        // Clamps that angle value depending of the NavMeshAgent parameters and rotates the squad
        _angle = Mathf.Clamp(_angle, -navAgent.angularSpeed * Time.deltaTime, navAgent.angularSpeed * Time.deltaTime);
        transform.Rotate(transform.up, _angle);
    }
    #endregion

    /// <summary>
    /// SquadUnselection method unselect the SqaudUnit when the Cancel button is trigged
    /// </summary>
    private void SquadUnselection()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Unselect();
        }
    }

    /// <summary>
    /// SquadMoveSelection method is used to select a SquadUnit destination
    /// </summary>
    private void SquadMoveSelection()
    {
        // For now a target is defined by the player with mouse click on terrain or active tower
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Enemies", "Buildings", "Terrain", "Soldiers" })))
            {
                // If the player click on a tower
                if (hit.collider.TryGetComponent<Tower>(out Tower tower))
                {
                    // Checks if its active (unactive towers behave like buildings and nothing happens)
                    if (tower.IsActive())
                    {
                        // Resets protectionStance to allow the rotation towards new target and sets squad target and destination
                        protectionStance = false;

                        // Set tower as new target for squad and its soldiers
                        SetTarget(tower);

                        // Move toward the target
                        Vector3 _dest = GetDestinationFromTower(tower);
                        if (_dest != GridAdjustment.GetGridCoordinates(tower.transform.position))
                        {
                            navAgent.SetDestination(_dest);
                            fixedDestination = true;
                        }
                        else
                        {
                            SetDestination(tower.transform.position);
                            fixedDestination = false;
                        }

                        // Unselect the squad
                        Unselect();
                    }
                }
                else if (hit.collider.TryGetComponent<EnemySoldier>(out EnemySoldier enemy))
                {
                    // Resets protectionStance to allow the rotation towards new target and sets squad target and destination
                    protectionStance = false;
                    // Set tower as new target for squad and its soldiers
                    SetTarget(enemy);
                    navAgent.SetDestination(enemy.transform.position);
                    fixedDestination = false;
                    // Unselect the squad
                    Unselect();
                }
                else if (hit.collider.gameObject.CompareTag("Soldiers"))
                {
                    if (hit.collider.TryGetComponent<Turret>(out Turret _turret))
                    {
                        SetDestination(GetNearestCellFromList(_turret.GetAvailablePositions()));
                        fixedDestination = true;
                        Unselect();
                    }
                    else if (hit.collider.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                    {
                        SetDestination(hit.point);
                        fixedDestination = true;
                        Unselect();
                    }
                    else if (hit.collider.transform.parent != null)
                    {
                        if (hit.collider.transform.parent.TryGetComponent<SquadUnit>(out SquadUnit _squadUnit))
                        {
                            SetDestination(hit.point);
                            fixedDestination = true;
                            Unselect();
                        }
                    }
                }
                else if(hit.collider.TryGetComponent<HQCandidate>(out HQCandidate _candidate))
                {
                    SetDestination(GetNearestCellFromList(_candidate.GetAvailablePositions()));
                    //fixedDestination = true;
                    Unselect();
                }
                else if (hit.collider.TryGetComponent<TurretBase>(out TurretBase _turretBase))
                {
                    SetDestination(GetNearestCellFromList(_turretBase.GetAvailablePositions()));
                    //fixedDestination = true;
                    Unselect();
                }
                else if (hit.collider.gameObject.CompareTag("Terrain"))
                {
                    bool foundEnemy = false;
                    // To allow to click on the same cell as an enemy and to select the enemy instead of the terrain
                    foreach (EnemySoldier _enemy in PlayManager.enemyList)
                    {
                        if (GridAdjustment.IsSameOnGrid(_enemy.transform.position, hit.point))
                        {
                            // Resets protectionStance to allow the rotation towards new target and sets squad target and destination
                            protectionStance = false;
                            // Set tower as new target for squad and its soldiers
                            SetTarget(_enemy);
                            navAgent.SetDestination(_enemy.transform.position);
                            fixedDestination = false;
                            // Unselect the squad
                            Unselect();
                            foundEnemy = true;
                        }
                    }

                    if (!foundEnemy)
                    {
                        // If the object hit was the terrain, sets squad destination
                        SetDestination(hit.point);
                        fixedDestination = true;
                        foreach (SquadUnit _squadUnit in PlayManager.squadUnitList)
                        {
                            if (_squadUnit == this) continue;
                            if (_squadUnit.Destination.x == GridAdjustment.GetGridCoordinates(hit.point).x && _squadUnit.Destination.z == GridAdjustment.GetGridCoordinates(hit.point).z)
                            {
                                // Move the other squad
                                _squadUnit.MoveAfterReplaced();
                            }
                        }
                        Unselect();
                    }                    
                }
            }
        }
    }

    /// <summary>
    /// SquadBuildHQSelection method is used to build a new HQ on a HQCandidate
    /// </summary>
    private void SquadBuildHQSelection()
    {
        // For now a target is defined by the player with mouse click on destroyed tower
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings" })))
            {
                // If the player click on a HQCandidate
                if (hit.collider.TryGetComponent<HQCandidate>(out HQCandidate _candidate))
                {
                    if(!_candidate.IsBuilding)
                    {
                        hqCandidate = _candidate;
                        hqCandidate.StartBuilding();
                        OnActionDone?.Invoke();                        
                    }
                    SetDestination(GetNearestCellFromList(_candidate.GetAvailablePositions()));
                    fixedDestination = true;
                    Unselect();
                }
            }
        }
    }

    /// <summary>
    /// SquadBuildTurretSelection method is used to build a new turret
    /// </summary>
    private void SquadBuildTurretSelection()
    {
        // For now a target is defined by the player with mouse click on destroyed tower
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain", "Buildings", "Enemies", "Soldiers" })))
            {
                // If the player clicked on a enemy or a building => Nothing

                // If the player clicked on a SquadUnit or a Soldier => build and move the SquadUnit clicked
                if(hit.collider.CompareTag("Soldiers"))
                {
                    GameObject _turret = Instantiate(PlayManager.data.turretBasePrefab, GridAdjustment.GetGridCoordinates(hit.point), Quaternion.identity);
                    _turret.transform.parent = GameObject.Find("Turrets").transform;
                    SetDestination(GetNearestCellFromList(_turret.GetComponent<TurretBase>().GetAvailablePositions()));
                    fixedDestination = true;

                    if (hit.collider.TryGetComponent<SquadUnit>(out SquadUnit _squad))
                    {
                        _squad.MoveAfterReplaced();
                    }
                    else if(hit.collider.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                    {
                        _soldier.Squad.MoveAfterReplaced();
                    }
                    OnActionDone?.Invoke();
                    Unselect();
                }

                // If the player clicked on the terrain => build if empty (no enemy soldier)
                if (hit.collider.CompareTag("Terrain"))
                {
                    bool foundEnemy = false;
                    // If an enemy is on the spot => do nothing
                    foreach (EnemySoldier _enemy in PlayManager.enemyList)
                    {
                        if (GridAdjustment.IsSameOnGrid(_enemy.transform.position, hit.point))
                        {
                            foundEnemy = true;
                        }
                    }
                    
                    if(!foundEnemy)
                    {
                        GameObject _turret = Instantiate(PlayManager.data.turretBasePrefab, GridAdjustment.GetGridCoordinates(hit.point), Quaternion.identity);
                        SetDestination(GetNearestCellFromList(_turret.GetComponent<TurretBase>().GetAvailablePositions()));
                        fixedDestination = true;
                        // If an squad is on the spot => Move the squad
                        foreach (SquadUnit _squad in PlayManager.squadUnitList)
                        {
                            if (GridAdjustment.IsSameOnGrid(_squad.Destination, hit.point))
                            {
                                _squad.MoveAfterReplaced();
                            }
                        }
                        OnActionDone?.Invoke();
                        Unselect();
                    }
                }
            }
        }
    }

    /// <summary>
    /// SquadBuildExplosivesSelection method is used to build new explosives on a Tower
    /// </summary>
    private void SquadBuildExplosivesSelection()
    {
        // For now a target is defined by the player with mouse click on destroyed tower
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Enemies" })))
            {
                // If the player click on a Tower
                if (hit.collider.TryGetComponent<Tower>(out Tower _tower))
                {
                    if(_tower.IsActive())
                    {

                        // Build a new list of vector with the 4 adjacent cells of the tower if they are accessible (in ShortT=Range list)
                        List<Vector3> _towerWalls = new List<Vector3>();
                        if(_tower.ShortRangeCells.Contains(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.right *10f)))
                        {
                            _towerWalls.Add(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.right * 10f));
                        }
                        if (_tower.ShortRangeCells.Contains(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.right * -10f)))
                        {
                            _towerWalls.Add(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.right * -10f));
                        }
                        if (_tower.ShortRangeCells.Contains(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.forward * 10f)))
                        {
                            _towerWalls.Add(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.forward * 10f));
                        }
                        if (_tower.ShortRangeCells.Contains(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.forward * -10f)))
                        {
                            _towerWalls.Add(GridAdjustment.GetGridCoordinates(_tower.transform.position + Vector3.forward * -10f));
                        }

                        // Get the nearest cell from this list
                        Vector3 _dest = GetNearestCellFromList(_towerWalls);

                        // Test witch cell is chosen to rotate the explosives
                        Quaternion _q = Quaternion.identity;
                        if ((_dest - _tower.transform.position).x > 5f)
                        {
                            _q = Quaternion.Euler(0f, -90f, 0f);
                        }
                        else if ((_dest - _tower.transform.position).x < -5f)
                        {
                            _q = Quaternion.Euler(0f, 90f, 0f);
                        }
                        else if ((_dest - _tower.transform.position).z > 5f)
                        {
                            _q = Quaternion.Euler(0f, 180f, 0f);
                        }

                        // Instantiate and setup the explosives prefab
                        GameObject _explosives = Instantiate(PlayManager.data.explosivesPrefab, _dest, _q);
                        _explosives.GetComponent<Explosives>().SetTarget(_tower);

                        // Set the squad destination and unselect
                        SetDestination(_dest);
                        fixedDestination = true;
                        // If an squad is on the spot => Move the squad
                        foreach (SquadUnit _squad in PlayManager.squadUnitList)
                        {
                            if (GridAdjustment.IsSameOnGrid(_squad.Destination, hit.point))
                            {
                                _squad.MoveAfterReplaced();
                            }
                        }

                        OnActionDone?.Invoke();
                        Unselect();
                    }                    
                }
            }
        }
    }

    /// <summary>
    /// SetDestination method set a new destination for the SquadUnit
    /// </summary>
    /// <param name="_destination">Destination for the SquadUnit (Vector3)</param>
    public void SetDestination(Vector3 _destination)
    {
        navAgent.SetDestination(GridAdjustment.GetGridCoordinates(_destination));
        foreach(SquadUnit _squad in PlayManager.squadUnitList)
        {
            if (_squad == this) continue;
            if (!_squad.IsRetreating && GridAdjustment.IsSameOnGrid(_squad.Destination, _destination)) _squad.MoveAfterReplaced();
        }
    }

    /// <summary>
    /// SquadUpdate is the Update method of the squad
    /// It defines the squad destination, the squad target and the soldiers destination
    /// </summary>
    void SquadUpdate()
    {
        // If there is no target defined, search for the nearest in range tower
        if (target == null)
        {
            Enemy _t = Ranges.GetNearestEnemy(this.transform, 1, 1, 0);
            if (_t != null)
            {
                SetTarget(_t);
            }
        }
        // If there is still no target found, allow the squad rotation to be managed by NavMeshAgent (forward = movement direction)
        if(target == null)
        {
            navAgent.updateRotation = true;
        }
        else
        {
            // But if there is a target, the rotation is managed by FaceTarget to face the target
            navAgent.updateRotation = false;
            FaceTarget(target);

            // If the target enters the longest attack range, sets the protection stance at true
            if ((target.transform.position - transform.position).magnitude <= PlayManager.LongRange)
            {
                if (!protectionStance) protectionStance = true;

                if (!fixedDestination && (GridAdjustment.GetGridCoordinates(target.transform.position) - GridAdjustment.GetGridCoordinates(transform.position)).magnitude <= maxStopRange)
                {
                    SetDestination(transform.position);
                    fixedDestination = true;
                }

            }
            else
            {
                // If the target is at more than the longest attack range
                // Checks if the squad was once at shooting distance and resets it along with the target
                // This is useful for retreat, the squad keeps its stance facing the target until it is not in range anymore
                if (protectionStance)
                {
                    protectionStance = false;
                    target = null;
                    OnTargetChange?.Invoke(target);
                }
            }
        }

        // If the squad is moving, defines the soldiers position relatively from the current squad position
        if (!navAgent.isStopped)
        {
            soldier1.SetDestination(transform.position + squadData.soldier1Position.x * transform.right + squadData.soldier1Position.y * transform.up + squadData.soldier1Position.z * transform.forward);
            soldier2.SetDestination(transform.position + squadData.soldier2Position.x * transform.right + squadData.soldier2Position.y * transform.up + squadData.soldier2Position.z * transform.forward);
            soldier3.SetDestination(transform.position + squadData.soldier3Position.x * transform.right + squadData.soldier3Position.y * transform.up + squadData.soldier3Position.z * transform.forward);
            soldier4.SetDestination(transform.position + squadData.soldier4Position.x * transform.right + squadData.soldier4Position.y * transform.up + squadData.soldier4Position.z * transform.forward);
        }

        // If the retreat has been commanded and the SquadUnit is near the HQ, gets back in
        if (retreatActive)
        {
            if ((transform.position - PlayManager.hqPos).magnitude <= 15f)
            {
                BackToHQ();
            }
        }
    }

    #region Move
    /// <summary>
    /// GetDestinationFromTower method returns the position to which the SquadUnit will attack the targeted tower
    /// </summary>
    /// <param name="_tower">Tower to attack</param>
    /// <returns>Vector3 to go to</returns>
    private Vector3 GetDestinationFromTower(Tower _tower)
    {
        // Get the empty position from Tower
        List<Vector3> _lgRange = new List<Vector3>(_tower.LongRangeCells);
        List<Vector3> _mdRange = new List<Vector3>(_tower.MiddleRangeCells);
        List<Vector3> _shRange = new List<Vector3>(_tower.ShortRangeCells);

        // Remove the other SquadUnit destination from the lists
        foreach (SquadUnit _su in PlayManager.squadUnitList)
        {
            if (_su == this) continue;
            _lgRange.Remove(GridAdjustment.GetGridCoordinates(_su.Destination));
            _mdRange.Remove(GridAdjustment.GetGridCoordinates(_su.Destination));
            _shRange.Remove(GridAdjustment.GetGridCoordinates(_su.Destination));
        }

        // Get the destination taking into account the Squad preferred range
        Vector3 _result = new Vector3();
        switch (squad.PrefRange)
        {
            case Squad.PreferedRange.LongRange:
                _result = GetNearestCellFromList(_lgRange);
                if (_result.Equals(Vector3.zero))
                {
                    _result = GetNearestCellFromList(_mdRange);
                    if (_result.Equals(Vector3.zero))
                    {
                        _result = GetNearestCellFromList(_shRange);
                    }
                }
                break;
            case Squad.PreferedRange.MiddleRange:
                _result = GetNearestCellFromList(_mdRange);
                if (_result.Equals(Vector3.zero))
                {
                    _result = GetNearestCellFromList(_lgRange);
                    if (_result.Equals(Vector3.zero))
                    {
                        _result = GetNearestCellFromList(_shRange);
                    }
                }
                break;
            case Squad.PreferedRange.ShortRange:
                _result = GetNearestCellFromList(_shRange);
                if (_result.Equals(Vector3.zero))
                {
                    _result = GetNearestCellFromList(_mdRange);
                    if (_result.Equals(Vector3.zero))
                    {
                        _result = GetNearestCellFromList(_lgRange);
                    }
                }
                break;
        }
        // If there is no result, display an error
        if (_result.Equals(Vector3.zero))Debug.LogError("[SquadUnit] No destination found for target " + _tower);

        return _result;
    }

    /// <summary>
    /// GetNearestCellFromList method returns from a list the cell the nearest to the target
    /// </summary>
    /// <param name="_cellList">Cells (Vector3) list</param>
    /// <returns></returns>
    private Vector3 GetNearestCellFromList(List<Vector3> _cellList)
    {
        float _distance = Mathf.Infinity;
        Vector3 _result = new Vector3();

        // For each Vector3 in the list: Check if distance is smaller than _distance and set the _result if it is
        foreach(Vector3 _cell in _cellList)
        {
            if((_cell - transform.position).magnitude < _distance)
            {
                _distance = (_cell - transform.position).magnitude;
                _result = _cell;
            }
        }
        return _result;
    }

    /// <summary>
    /// MoveAfterReplace method sets the SquadUnit destination to the nearest available position
    /// </summary>
    public void MoveAfterReplaced()
    {
        // If there is a target, the nearest position to go is from it
        if(target != null)
        {
            navAgent.SetDestination(GetDestinationFromTower(target.GetComponent<Tower>()));
        }
        else // else the nearest destination is around the SquadUnit
        {
            navAgent.SetDestination(GetNearestEmptyDestination());
        }
    }

    /// <summary>
    /// GetNearestEmptyDestination method returns the nearest empty position around the SquadUnit
    /// </summary>
    /// <returns>Destination (Vector3)</returns>
    private Vector3 GetNearestEmptyDestination()
    {
        // Turn around the Squad destination in spiral and get the empty cells
        int k = 0;
        List<Vector3> list = new List<Vector3>();
        bool found = false;
        Vector3 _pos = Vector3.zero;
        // While no destination has been found in a circle of 5 cells around the position
        while (!found && k<5)
        {
            k++;
            for (int i = 0; i <= k; i++)
            {
                int j = k - i;
               //  i /  j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(i * 10f, 0f, j * 10f));
                if(PositionIsEmpty(_pos))
                {
                    if(!list.Contains(GridAdjustment.GetGridCoordinates(_pos)))list.Add(GridAdjustment.GetGridCoordinates(_pos));
                    found = true;
                }

                //  i / -j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(i * 10f, 0f, -j * 10f)) + 20f * Vector3.up;
                if (PositionIsEmpty(_pos))
                {
                    if (!list.Contains(GridAdjustment.GetGridCoordinates(_pos))) list.Add(GridAdjustment.GetGridCoordinates(_pos));
                    found = true;
                }

                // -i /  j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(-i * 10f, 0f, j * 10f)) + 20f * Vector3.up;
                if (PositionIsEmpty(_pos))
                {
                    if (!list.Contains(GridAdjustment.GetGridCoordinates(_pos))) list.Add(GridAdjustment.GetGridCoordinates(_pos));
                    found = true;
                }

                // -i / -j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(-i * 10f, 0f, -j * 10f)) + 20f * Vector3.up;
                if (PositionIsEmpty(_pos))
                {
                    if (!list.Contains(GridAdjustment.GetGridCoordinates(_pos))) list.Add(GridAdjustment.GetGridCoordinates(_pos));
                    found = true;
                }
            }
        }

        // Return a random destination from the list (destinations at the same distance of the current point)
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// PositionIsEmpty method is used to know if the input position is available for a squad movement
    /// The position is empty if there is no building, tower, HQ, explosives, enemy soldier on it and if it is not a destination from another squad
    /// </summary>
    /// <param name="_pos">Position (Vector3)</param>
    /// <returns>True if empty, false otherwise</returns>
    private bool PositionIsEmpty(Vector3 _pos)
    {
        Ray _ray = new Ray(_pos + 20f * Vector3.up, -Vector3.up);
        RaycastHit _hit;
        // Position is empty if there is no Buildings, HQ, Turret or Tower (terrain is hit)
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings", "Enemies", "Soldiers")))
        {
            if (_hit.collider.gameObject.CompareTag("Terrain"))
            {
                // If the position is already a SquadUnit destination, position is not considered as empty
                foreach (SquadUnit _squad in PlayManager.squadUnitList)
                {
                    if (_squad == this) continue;
                    if (GridAdjustment.IsSameOnGrid(_squad.Destination, _pos)) return false;
                }

                // If the position is the position of an explosives (from SoldierUnit), it is not empty
                foreach (Explosives _explosives in PlayManager.explosivesList)
                {
                    if (GridAdjustment.IsSameOnGrid(_explosives.transform.position, _pos)) return false;
                }

                // If the position is the position of an enemy soldier, it is not empty
                foreach (EnemySoldier _enemy in PlayManager.enemyList)
                {
                    if (GridAdjustment.IsSameOnGrid(_enemy.transform.position, _pos)) return false;
                }

                // If the position is Terrain, not explosives, not enemy soldier and not another Squad destination : it is empty
                return true;
            }
        }
        // If nothing is hit by the raycast or anything else than the terrain is hit, position is not empty
        return false;
    }
    #endregion

    /// <summary>
    /// Retreat method sets the destination to the HS and activate the retreat boolean
    /// </summary>
    public void Retreat()
    {
        // As setting destination to HQ pos is not working anymore (issue with NavMesh and maybe empty buildings)
        // The destination is the first spawn point (should be a spawn point at max 10m of the HQ, so the squad will retrun to HQ)
        navAgent.SetDestination(PlayManager.hq.SpawnPositions[0]);

        fixedDestination = true;
        retreatActive = true;
        Unselect();
    }

    /// <summary>
    /// BackToHQ method is used to destroy the SquadUnit when it goes back into HQ (after retreat)
    /// </summary>
    public void BackToHQ()
    {
        OnHQBack?.Invoke();
        PlayManager.RemoveSquadUnit(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Die method destroys the SquadUnit
    /// </summary>
    public void Die()
    {
        OnDeath?.Invoke();
        PlayManager.RemoveSquadUnit(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// ResetSquadUnit method removes the SquadUnit, its soldiers and its SquadActionPanel without triggering the PlayManager EndDayRoutine
    /// </summary>
    private void ResetSquadUnit()
    {
        OnHQBack?.Invoke();
        PlayManager.squadUnitList.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// CheckDeath method checks if all SoldierUnits are dead and calls Die method if they are
    /// </summary>
    public void CheckDeath()
    {
        if (soldier1.IsWounded && soldier2.IsWounded && soldier3.IsWounded && soldier4.IsWounded) Die();
    }
}
