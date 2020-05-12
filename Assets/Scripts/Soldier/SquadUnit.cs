using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

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
    private bool isSelected = false;

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
    #endregion

    public void Setup(Squad _squad)
    {
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

        mainCamera = Camera.main;

        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = squad.Color;

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;

        soldier1 = CreateSoldier("Soldier1", squadData.soldier1Position, squad.Soldiers[0]);
        soldier2 = CreateSoldier("Soldier2", squadData.soldier2Position, squad.Soldiers[1]);
        soldier3 = CreateSoldier("Soldier3", squadData.soldier3Position, squad.Soldiers[2]);
        soldier4 = CreateSoldier("Soldier4", squadData.soldier4Position, squad.Soldiers[3]);

        // Link update events
        GameManager.PlayUpdate += SquadUpdate;
        PlayManager.RetreatAll += Retreat;
    }

    /// <summary>
    /// On destroy, destoys soldiers and clear event links
    /// </summary>
    private void OnDestroy()
    {
        // Destroys soldiers
        //Destroy(soldier1.gameObject);
        //Destroy(soldier2.gameObject);
        //Destroy(soldier3.gameObject);
        //Destroy(soldier4.gameObject);

        // Unlinks event
        GameManager.PlayUpdate -= SquadUpdate;
        PlayManager.RetreatAll -= Retreat;
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

    #region Target
    public void SetTarget(Tower _t)
    {
        if (target != null) ClearTarget();
        target = _t;
        OnTargetChange?.Invoke(target);
        if (target != null)
        {
            target.OnDestruction += ClearTarget;
        }
    }

    public void ClearTarget()
    {
        target.OnDestruction -= ClearTarget;
        target = null;
        //OnTargetChange?.Invoke(target);
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

        if (retreatActive)
        {
            if((transform.position - PlayManager.hqPos).magnitude <= 15f)
            {
                BackToHQ();
            }
        }

    }

    #region Move
    private Vector3 GetDestinationFromTower(Tower _tower)
    {
        List<Vector3> _lgRange = new List<Vector3>(_tower.LongRangeCells);
        List<Vector3> _mdRange = new List<Vector3>(_tower.MiddleRangeCells);
        List<Vector3> _shRange = new List<Vector3>(_tower.ShortRangeCells);
        Vector3 _squadPos;

        foreach (SquadUnit _su in PlayManager.squadUnitList)
        {
            if (_su == this) continue;
            _squadPos = new Vector3(_su.Destination.x - _tower.transform.position.x,0f, _su.Destination.z - _tower.transform.position.z);
            if (_lgRange.Contains(_squadPos)) _lgRange.Remove(_squadPos);
            if (_mdRange.Contains(_squadPos)) _mdRange.Remove(_squadPos);
            if (_shRange.Contains(_squadPos)) _shRange.Remove(_squadPos);
        }

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
        if (_result.Equals(Vector3.zero))Debug.LogError("[SquadUnit] No destination found for target " + _tower);
        _result += GridAdjustment.GetGridCoordinates(_tower.transform.position);
        return _result;
    }

    private Vector3 GetNearestCellFromList(Transform _target, List<Vector3> _cellList)
    {

        float _distance = Mathf.Infinity;
        Vector3 _result = new Vector3();

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

    public void MoveAfterReplaced()
    {
        if(target != null)
        {
            navAgent.SetDestination(GetDestinationFromTower(target.GetComponent<Tower>()));
        }
        else
        {
            navAgent.SetDestination(GetNearestEmptyDestination());
        }
    }

    private Vector3 GetNearestEmptyDestination()
    {
        int k = 0;
        List<Vector3> list = new List<Vector3>();
        bool found = false;
        bool doNotAdd = false;
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
        return list[Random.Range(0, list.Count)];
    }
    #endregion

    public void Retreat()
    {
        navAgent.SetDestination(PlayManager.hqPos);
        fixedDestination = true;
        retreatActive = true;
    }
    public void BackToHQ()
    {
        OnHQBack?.Invoke();
        PlayManager.RemoveSquadUnit(this);
        Destroy(gameObject);
    }

    public void Die()
    {
        OnDeath?.Invoke();
        PlayManager.RemoveSquadUnit(this);
        Destroy(gameObject);
    }

    public void CheckDeath()
    {
        if (soldier1.HP <= 0 && soldier2.HP <= 0 && soldier3.HP <= 0 && soldier4.HP <= 0) Die();
    }
}
