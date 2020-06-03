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
    private Tower target;
    private bool protectionStance = false;
    private float maxStopRange;

    // Destination components
    private bool fixedDestination = false;
    private bool retreatActive = false;

    // Play routine parameters
    // private bool isSelected = false;

    // Events
    public delegate void SquadTargetEventHandler(Tower _target);
    public event SquadTargetEventHandler OnTargetChange;

    public delegate void SquadUnitEventHandler();
    public event SquadUnitEventHandler Unselect;
    public event SquadUnitEventHandler OnDeath;
    public event SquadUnitEventHandler OnHQBack;

    #region Properties access
    public Vector3 Destination
    {
        get { return navAgent.destination; }
    }
    public int ID
    {
        get { return squad.ID; }
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
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = squad.Color;

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
    }

    /// <summary>
    /// On destroy, destoys soldiers and clear event links
    /// </summary>
    private void OnDestroy()
    {
        // Unlinks event
        GameManager.PlayUpdate -= SquadUpdate;
        Unselect = null;
    }

    /// <summary>
    /// CreateSoldier methods creates a soldier ans returns its Soldier component
    /// </summary>
    /// <param name="_name">Name of the gameobject</param>
    /// <param name="_prefab">Prefab of soldier to instanciate</param>
    /// <param name="_position">Relative position where to instanciate the soldier</param>
    /// <returns></returns>
    private SoldierUnit CreateSoldier(string _name, Vector3 _position, Soldier _soldier)
    {
        // Set the soldier as engaged (used to heal soldier not engaged during day)
        _soldier.Engage(true);

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
    /// Select method is used to select the squad and trigged its Move action
    /// </summary>
    /// <param name="select">Boolean to choose if the SquadUnit is selected or not</param>
    /*
    public void Select(bool select)
    {
        if (select)
        {
            GameManager.PlayUpdate += SquadMoveSelection;
        }
        else
        {
            GameManager.PlayUpdate -= SquadMoveSelection;
        }
    }
    */

    /// <summary>
    /// OnMoveActionSelected method trigged the Move action of this SquadUnit
    /// </summary>
    public void OnMoveActionSelected()
    {
        GameManager.PlayUpdate += SquadMoveSelection;
        Unselect += OnMoveActionUnselected;
    }

    /// <summary>
    /// OnMoveActionUnselected method untrigged the Move action of this SquadUnit
    /// </summary>
    public void OnMoveActionUnselected()
    {
        GameManager.PlayUpdate -= SquadMoveSelection;
        Unselect -= OnMoveActionUnselected;
    }

    /// <summary>
    /// OnBuildHQSelected method trigged the BuildHQ action of this SquadUnit
    /// </summary>
    public void OnBuildHQSelected()
    {

    }

    /// <summary>
    /// OnBuildHQUnselected method untrigged the BuildHQ action of this SquadUnit
    /// </summary>
    public void OnBuildHQUnselected()
    {

    }

    /// <summary>
    /// OnBuildTurretSelected method trigged the BuildTurret action of this SquadUnit
    /// </summary>
    public void OnBuildTurretSelected()
    {

    }

    /// <summary>
    /// OnBuildTurretUnselected method untrigged the BuildTurret action of this SquadUnit
    /// </summary>
    public void OnBuildTurretUnselected()
    {

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

        Unselect?.Invoke();
    }

    #region Target
    /// <summary>
    /// SetTarget method set the target of the SquadUnit
    /// </summary>
    /// <param name="_t">Tower that will be the new target</param>
    public void SetTarget(Tower _t)
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
    /// <param name="_target"></param>
    private void FaceTarget(Tower _target)
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

    private void SquadMoveSelection()
    {
        // For now a target is defined by the player with mouse click on terrain or active tower
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Terrain", "Soldiers" })))
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
                            navAgent.SetDestination(tower.transform.position);
                            fixedDestination = false;
                        }

                        // Unselect the squad
                        Unselect?.Invoke();
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Soldiers"))
                {
                    if (hit.collider.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                    {
                        navAgent.SetDestination(GridAdjustment.GetGridCoordinates(hit.point));
                        fixedDestination = true;
                        if (_soldier.Squad.Destination.x == GridAdjustment.GetGridCoordinates(hit.point).x && _soldier.Squad.Destination.z == GridAdjustment.GetGridCoordinates(hit.point).z)
                        {
                            // Move the other squad
                            _soldier.Squad.MoveAfterReplaced();
                        }
                        Unselect?.Invoke();
                    }
                    else if (hit.collider.transform.parent != null)
                    {
                        if (hit.collider.transform.parent.TryGetComponent<SquadUnit>(out SquadUnit _squadUnit))
                        {
                            navAgent.SetDestination(GridAdjustment.GetGridCoordinates(hit.point));
                            fixedDestination = true;
                            if (_squadUnit.Destination.x == GridAdjustment.GetGridCoordinates(hit.point).x && _squadUnit.Destination.z == GridAdjustment.GetGridCoordinates(hit.point).z)
                            {
                                // Move the other squad
                                _squadUnit.MoveAfterReplaced();
                            }
                            Unselect?.Invoke();
                        }
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Terrain"))
                {
                    // If the object hit was the terrain, sets squad destination
                    navAgent.SetDestination(GridAdjustment.GetGridCoordinates(hit.point));
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
                    Unselect?.Invoke();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Unselect?.Invoke();
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
            Tower _t = Ranges.GetNearestTower(this.transform, 1, 1, 0);
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
                    navAgent.SetDestination(GridAdjustment.GetGridCoordinates(transform.position));
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
                    OnTargetChange(target);
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

        // If the retreat has been commanded and the SquadUnit is near teh HQ, gets back in
        if (retreatActive)
        {
            if((transform.position - PlayManager.hqPos).magnitude <= 15f)
            {
                BackToHQ();
            }
        }

    }

    #region Move
    /// <summary>
    /// GetDestinationFromTower method returns the position to which the SquadUnit will attack the selected target
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
        Vector3 _squadPos;
        foreach (SquadUnit _su in PlayManager.squadUnitList)
        {
            if (_su == this) continue;
            _squadPos = new Vector3(_su.Destination.x - _tower.transform.position.x,0f, _su.Destination.z - _tower.transform.position.z);
            if (_lgRange.Contains(_squadPos)) _lgRange.Remove(_squadPos);
            if (_mdRange.Contains(_squadPos)) _mdRange.Remove(_squadPos);
            if (_shRange.Contains(_squadPos)) _shRange.Remove(_squadPos);
        }

        // Get the destination taking into account the Squad preferred range
        Vector3 _result = new Vector3();
        switch (squad.PrefRange)
        {
            case Squad.PreferedRange.LongRange:
                _result = GetNearestCellFromList(_tower.transform, _lgRange);
                if (_result.Equals(Vector3.zero))
                {
                    _result = GetNearestCellFromList(_tower.transform, _mdRange);
                    if (_result.Equals(Vector3.zero))
                    {
                        _result = GetNearestCellFromList(_tower.transform, _shRange);
                    }
                }
                break;
            case Squad.PreferedRange.MiddleRange:
                _result = GetNearestCellFromList(_tower.transform, _mdRange);
                if (_result.Equals(Vector3.zero))
                {
                    _result = GetNearestCellFromList(_tower.transform, _lgRange);
                    if (_result.Equals(Vector3.zero))
                    {
                        _result = GetNearestCellFromList(_tower.transform, _shRange);
                    }
                }
                break;
            case Squad.PreferedRange.ShortRange:
                _result = GetNearestCellFromList(_tower.transform, _shRange);
                if (_result.Equals(Vector3.zero))
                {
                    _result = GetNearestCellFromList(_tower.transform, _mdRange);
                    if (_result.Equals(Vector3.zero))
                    {
                        _result = GetNearestCellFromList(_tower.transform, _lgRange);
                    }
                }
                break;
        }
        // If there is no result, display an error
        if (_result.Equals(Vector3.zero))Debug.LogError("[SquadUnit] No destination found for target " + _tower);

        // Adjust result to the grid and returns it
        _result += GridAdjustment.GetGridCoordinates(_tower.transform.position);
        return _result;
    }

    /// <summary>
    /// GetNearestCellFromList method returns from a list the cell the nearest to the target
    /// </summary>
    /// <param name="_target">Target transform</param>
    /// <param name="_cellList">Cells (Vector3) list</param>
    /// <returns></returns>
    private Vector3 GetNearestCellFromList(Transform _target, List<Vector3> _cellList)
    {
        float _distance = Mathf.Infinity;
        Vector3 _result = new Vector3();

        // For each Vector3 in the list: Check if distance is smaller than _distance and set the _result if it is
        foreach(Vector3 _vect in _cellList)
        {
            if((_target.position + _vect - transform.position).magnitude < _distance)
            {
                _distance = (_target.position + _vect - transform.position).magnitude;
                _result = _vect;
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
        bool doNotAdd = false;
        // While no destination has been found in a circle of 5 cells around the position
        while (!found && k<5)
        {
            k++;
            for (int i = 0; i <= k; i++)
            {
                int j = k - i;
                doNotAdd = false;
                //  i /  j
                Vector3 _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(i * 10f, 0f, j * 10f)) + 20f*Vector3.up;
                Ray _ray = new Ray(_pos, -Vector3.up);
                RaycastHit _hit;
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings")))
                {
                    // If the cell is empty (no tower, no buildings)
                    if (_hit.collider.gameObject.CompareTag("Terrain"))
                    {
                        // Check if it is already a destination of another squad
                        for (int s = 0; s < PlayManager.squadUnitList.Count; s++)
                        {
                            if (_pos.x == PlayManager.squadUnitList[s].Destination.x && _pos.z == PlayManager.squadUnitList[s].Destination.z) doNotAdd = true;
                        }
                        
                        // If the position is not already in the list and is not the destination of another squad, add it to the list of empty destination
                        if (!list.Contains(GridAdjustment.GetGridCoordinates(_hit.point)) && !doNotAdd)
                        {
                            list.Add(GridAdjustment.GetGridCoordinates(_hit.point));
                            found = true;
                        }
                    }
                }
                doNotAdd = false;
                //  i / -j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(i * 10f, 0f, -j * 10f)) + 20f * Vector3.up;
                _ray = new Ray(_pos, -Vector3.up);
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings")))
                {
                    if (_hit.collider.gameObject.CompareTag("Terrain"))
                    {
                        for (int s = 0; s < PlayManager.squadUnitList.Count; s++)
                        {
                            if (_pos.x == PlayManager.squadUnitList[s].Destination.x && _pos.z == PlayManager.squadUnitList[s].Destination.z) doNotAdd = true;
                        }

                        if (!list.Contains(GridAdjustment.GetGridCoordinates(_hit.point)) && !doNotAdd)
                        {
                            list.Add(GridAdjustment.GetGridCoordinates(_hit.point));
                            found = true;
                        }
                    }
                }
                doNotAdd = false;
                // -i /  j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(-i * 10f, 0f, j * 10f)) + 20f * Vector3.up;
                _ray = new Ray(_pos, -Vector3.up);
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings")))
                {
                    if (_hit.collider.gameObject.CompareTag("Terrain"))
                    {
                        for (int s = 0; s < PlayManager.squadUnitList.Count; s++)
                        {
                            if (_pos.x == PlayManager.squadUnitList[s].Destination.x && _pos.z == PlayManager.squadUnitList[s].Destination.z) doNotAdd = true;
                        }

                        if (!list.Contains(GridAdjustment.GetGridCoordinates(_hit.point)) && !doNotAdd)
                        {
                            list.Add(GridAdjustment.GetGridCoordinates(_hit.point));
                            found = true;
                        }
                    }
                }
                doNotAdd = false;
                // -i / -j
                _pos = GridAdjustment.GetGridCoordinates(Destination + new Vector3(-i * 10f, 0f, -j * 10f)) + 20f * Vector3.up;
                _ray = new Ray(_pos, -Vector3.up);
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings")))
                {
                    if (_hit.collider.gameObject.CompareTag("Terrain"))
                    {
                        for (int s = 0; s < PlayManager.squadUnitList.Count; s++)
                        {
                            if (_pos.x == PlayManager.squadUnitList[s].Destination.x && _pos.z == PlayManager.squadUnitList[s].Destination.z) doNotAdd = true;
                        }

                        if (!list.Contains(GridAdjustment.GetGridCoordinates(_hit.point)) && !doNotAdd)
                        {
                            list.Add(GridAdjustment.GetGridCoordinates(_hit.point));
                            found = true;
                        }
                    }
                }
                doNotAdd = false;
            }
        }

        // Return a random destination from the list (destinations at the same distance of the current point)
        return list[Random.Range(0, list.Count)];
    }
    #endregion

    /// <summary>
    /// Retreat method sets the destination to the HS and activate the retreat boolean
    /// </summary>
    public void Retreat()
    {
        navAgent.SetDestination(PlayManager.hqPos);
        fixedDestination = true;
        retreatActive = true;
        Unselect?.Invoke();
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
    /// CheckDeath method checks if all SoldierUnits are dead and calls Die method if they are
    /// </summary>
    public void CheckDeath()
    {
        if (soldier1.HP <= 0 && soldier2.HP <= 0 && soldier3.HP <= 0 && soldier4.HP <= 0) Die();
    }
}
