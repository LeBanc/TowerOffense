using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Squad : MonoBehaviour
{
    // Soldier positions relative to the squad center point
    public SquadData squadData;
    private Vector3 soldier1Position;
    private Vector3 soldier2Position;
    private Vector3 soldier3Position;
    private Vector3 soldier4Position;

    // Soldier data
    public SoldierData soldier1data;
    public SoldierData soldier2data;
    public SoldierData soldier3data;
    public SoldierData soldier4data;

    // Soldier prefab
    [SerializeField]
    private GameObject soldierPrefab;

    // Basic components
    private Camera mainCamera;
    private NavMeshAgent navAgent;
    
    // Soldier components
    private Soldier soldier1;
    private Soldier soldier2;
    private Soldier soldier3;
    private Soldier soldier4;

    // Target components
    private Transform target;
    private bool protectionStance = false;

    // Events
    public delegate void SquadTargetEventHandler(Transform _target);
    public event SquadTargetEventHandler OnTargetChange;

    /// <summary>
    /// On start, define basic components and creates soldiers
    /// </summary>
    void Start()
    {
        mainCamera = Camera.main;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;

        soldier1Position = squadData.soldier1Position;
        soldier2Position = squadData.soldier2Position;
        soldier3Position = squadData.soldier3Position;
        soldier4Position = squadData.soldier4Position;

        soldier1 = CreateSoldier("Soldier1", soldierPrefab, soldier1Position, soldier1data);
        soldier2 = CreateSoldier("Soldier2", soldierPrefab, soldier2Position, soldier2data);
        soldier3 = CreateSoldier("Soldier3", soldierPrefab, soldier3Position, soldier3data);
        soldier4 = CreateSoldier("Soldier4", soldierPrefab, soldier4Position, soldier4data);

        // Link update events
        GameManager.PlayUpdate += SquadUpdate;
    }

    /// <summary>
    /// On destroy, destoys soldiers and clear event links
    /// </summary>
    private void OnDestroy()
    {
        // Destroys soldiers
        Destroy(soldier1.gameObject);
        Destroy(soldier2.gameObject);
        Destroy(soldier3.gameObject);
        Destroy(soldier4.gameObject);

        // Unlinks event
        GameManager.PlayUpdate -= SquadUpdate;
    }

    /// <summary>
    /// CreateSoldier methods creates a soldier ans returns its Soldier component
    /// </summary>
    /// <param name="_name">Name of the gameobject</param>
    /// <param name="_prefab">Prefab of soldier to instanciate</param>
    /// <param name="_position">Relative position where to instanciate the soldier</param>
    /// <returns></returns>
    private Soldier CreateSoldier(string _name, GameObject _prefab, Vector3 _position, SoldierData _data)
    {
        // Creates a gameObject from prefab and changes its name
        GameObject _soldierGO = Instantiate(_prefab, transform.position + _position, transform.rotation, transform);
        _soldierGO.name = _name;

        // Gets the Soldier component and calls its setup if found
        Soldier _soldier = _soldierGO.GetComponent<Soldier>();
        if (_soldier == null)
        {
            Debug.LogError("[Squad] Trying to instantiate GameObject that is not a Soldier");
        }
        else
        {
            _soldier.Setup(this, _data, _data.maxHP);
        }

        return _soldier;
    }

    /// <summary>
    /// FaceTarget method is used to rotate the squad toward its target
    /// </summary>
    /// <param name="_target"></param>
    private void FaceTarget(Transform _target)
    {
        // Gets the projection on XZ plane of the vector between target and squad positions
        Vector3 _diff = _target.position - transform.position;
        _diff = new Vector3(_diff.x, 0f, _diff.z);

        // Gets the angle between the _diff vector and the forward squad axe
        float _angle = Vector3.SignedAngle(transform.forward, Vector3.Normalize(_diff), transform.up);
        // Clamps that angle value depending of the NavMeshAgent parameters and rotates the squad
        _angle = Mathf.Clamp(_angle, -navAgent.angularSpeed * Time.deltaTime, navAgent.angularSpeed * Time.deltaTime);
        transform.Rotate(transform.up, _angle);
    }

    /// <summary>
    /// SquadUpdate is the Update method of the squad
    /// It defines the squad destination, the squad target and the soldiers destination
    /// </summary>
    void SquadUpdate()
    {

        // For now a target is defined by the player with mouse click on terrain or active tower
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Terrain" })))
            {
                // If the player click on a tower
                if(hit.collider.TryGetComponent<Tower>(out Tower tower))
                {
                    // Checks if its active (unactive towers behave like buildings and nothing happens)
                    if (tower.IsActive())
                    {
                        // Resets protectionStance to allow the rotation towards new target and sets squad target and destination
                        protectionStance = false;
                        navAgent.SetDestination(tower.transform.position);
                        target = tower.transform;
                        OnTargetChange(target);
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Terrain"))
                {
                    // If the object hit was the terrain, sets sqaud destination
                    navAgent.SetDestination(GridAdjustment.GetGridCoordinates(hit.point));
                }
            }
        }

        // If there is no target defined, search for the nearest in range tower
        if (target == null)
        {
            target = Ranges.GetNearestTower(this.transform, 1,1,0);
            if (target != null)
            {
                OnTargetChange(target);
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
            if ((target.position - transform.position).magnitude <= PlayManager.LongRange)
            {
                if (!protectionStance) protectionStance = true;
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
            soldier1.SetDestination(transform.position + soldier1Position.x * transform.right + soldier1Position.y * transform.up + soldier1Position.z * transform.forward);
            soldier2.SetDestination(transform.position + soldier2Position.x * transform.right + soldier2Position.y * transform.up + soldier2Position.z * transform.forward);
            soldier3.SetDestination(transform.position + soldier3Position.x * transform.right + soldier3Position.y * transform.up + soldier3Position.z * transform.forward);
            soldier4.SetDestination(transform.position + soldier4Position.x * transform.right + soldier4Position.y * transform.up + soldier4Position.z * transform.forward);
        }
    }
}
