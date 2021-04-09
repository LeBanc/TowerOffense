using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HQ class manages the in game HQ
/// </summary>
public class HQ : MonoBehaviour
{
    // Prefab of an HQCandidate
    public GameObject hqCandidate;
    private static GameObject hqCandidateInstance;

    // Vector3 for spawn positions around the HQ
    private Vector3[] spawnPoints;
    private Vector3[] positions;

    // floats for day/night and heal management
    private float dayTimeCounter = 0f;
    private float nightTimeCounter = 0f;
    private float healDelayCounter = 0f;
    private float healDelay;
    private int healAmount;

    // float for elapsed attack time
    private float attackTime;
    // boolean to set attack or retreat mode
    private bool attacking;

    #region Properties access
    public int HealAmount
    {
        set { healAmount = value; }
        get { return healAmount; }
    }

    public float AttackTime
    {
        set { attackTime = value; }
        get { return attackTime; }
    }

    public Vector3[] SpawnPositions
    {
        get { return spawnPoints; }
    }
    #endregion

    /// <summary>
    /// On Start, computes the possible spawn points and subscribe to the Squad load event
    /// </summary>
    void Start()
    {
        hqCandidateInstance = hqCandidate;
        PlayManager.OnLoadSquadsOnNewDay += InstantiateSquads;
        //PlayManager.OnEndDay += EndDayAtHQ;
    }

    /// <summary>
    /// OnDestroy, unsubscribe events
    /// </summary>
    private void OnDestroy()
    {
        PlayManager.OnLoadSquadsOnNewDay -= InstantiateSquads;
        //PlayManager.OnEndDay -= EndDayAtHQ;
        GameManager.PlayUpdate -= HQUpdate;
    }

    #region SpawPoints
    /// <summary>
    /// SetSpawnPoints looks for each SpawnPoints if the cell is empty until it finds 4 available cells
    /// </summary>
    public void SetSpawnPoints()
    {
        // SpawnPoints is a list of the 4 nearest spaw points (for the 4 squads at max)
        spawnPoints = new Vector3[4];

        // Positions is the list of all possible spaw positions around the HQ
        positions = new Vector3[20];
        positions[0] = new Vector3(0f, 0f, 10f);
        positions[1] = new Vector3(10f, 0f, 0f);
        positions[2] = new Vector3(0f, 0f, -10f);
        positions[3] = new Vector3(-10f, 0f, 0f);
        positions[4] = new Vector3(-10f, 0f, 10f);
        positions[5] = new Vector3(10f, 0f, 10f);
        positions[6] = new Vector3(10f, 0f, -10f);
        positions[7] = new Vector3(-10f, 0f, -10f);
        positions[8] = new Vector3(0f, 0f, 20f);
        positions[9] = new Vector3(20f, 0f, 0f);
        positions[10] = new Vector3(0f, 0f, -20f);
        positions[11] = new Vector3(-20f, 0f, 0f);
        positions[12] = new Vector3(-20f, 0f, 10f);
        positions[13] = new Vector3(-10f, 0f, 20f);
        positions[14] = new Vector3(10f, 0f, 20f);
        positions[15] = new Vector3(20f, 0f, 10f);
        positions[16] = new Vector3(20f, 0f, -10f);
        positions[17] = new Vector3(10f, 0f, -20f);
        positions[18] = new Vector3(-10f, 0f, -20f);
        positions[19] = new Vector3(-20f, 0f, -10f);

        int _index = 0;
        int _posIndex = 0;

        while (_index < 4 && _posIndex <positions.Length)
        {
            Vector3 _posToTest = transform.position + positions[_posIndex];
            if (IsSpawnPoint(ref _posToTest))
            {
                spawnPoints[_index] = _posToTest;
                _index++;
            }
            _posIndex++;
        }
        if (_index < 4) Debug.LogError("[HQ] Cannot find 4 spawn points!");
    }

    /// <summary>
    /// IsSpawnPoints returns the avaibility of the transform position to be a spawn point
    /// </summary>
    /// <param name="position">Spawn point position, the position is changed to the raycast hit if it is an valid spawn point </param>
    /// <returns>True if the cell is an available spawn point, false otherwise</returns>
    private bool IsSpawnPoint(ref Vector3 position)
    {
        Ray _ray = new Ray(position + Vector3.up * 20, -Vector3.up);
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings", "Enemies")))
        {
            if (_hit.collider.gameObject.CompareTag("Terrain"))
            {
                position = _hit.point + 0.01f * Vector3.up;
                return true;
            }
            else
            {
                return false;
            }
        }
        else return false;
    }

    /// <summary>
    /// GetSpawnPoints returns the spawn points of the HQ
    /// </summary>
    /// <returns>Vector3[] spawnPoints</returns>
    public Vector3[] GetSpawnPoints()
    {
        return spawnPoints;
    }
    #endregion

    /// <summary>
    /// InstantiateSquads instantiate all the engaged squads on the field, around the HQ
    /// </summary>
    public void InstantiateSquads()
    {
        // Set new spawn points before instanciating squads
        SetSpawnPoints();

        int _index = 0;
        foreach(Squad _squad in PlayManager.squadList)
        {
            if (_squad.isEngaged)
            {
                // Instanciate SqaudUnit to a possible spawn point and adds it to the PlayManager list
                SquadUnit _su = _squad.InstanciateSquadUnit(spawnPoints[_index]);
                _index++;
                PlayManager.squadUnitList.Add(_su);
            }
        }

        // Initialize the healDelay (to heal the whole HealAmount over the whole attackTime)
        healDelay = (attackTime / HealAmount);

        // Subscribe to the PlayUpdate event
        GameManager.PlayUpdate += HQUpdate;

        // Set the attack mode as true
        attacking = true;

        // Initialize the counter
        healDelayCounter = 0f;
        dayTimeCounter = 0f;
        nightTimeCounter = 0f;
    }

    /// <summary>
    /// EndDayAtHQ ends the attack and heals all soldiers of the not elapsed healAmount
    /// </summary>
    public void EndDayAtHQ()
    {
        // Unsubscribe from PlayUpdate event
        GameManager.PlayUpdate -= HQUpdate;
        // Set attack as finished
        attacking = false;
        // Heal soldiers of an amount corresponding to the rest of the night time
        int _heal = (int)Mathf.Floor((attackTime - nightTimeCounter)*healAmount/attackTime);
        HealSoldiers(_heal);

        // Notify the PlayManager that the HQ End of day actions are ended by calling SwitchToHQPhase
        PlayManager.SwitchToHQPhase();
    }

    /// <summary>
    /// HQUpdate is the Update method of the HQ
    /// </summary>
    public void HQUpdate()
    {
        // If the healDelayCounter as expires, heal soldiers of 1 HP
        if (healDelayCounter >= healDelay)
        {
            HealSoldiers(1);
        }

        // If the dayTime has not expired, increments it
        if (dayTimeCounter <= attackTime)
        {
            dayTimeCounter += Time.deltaTime;
        }
        // else sets the end of the attack (once) and increment the nightCounter
        else
        {
            if (attacking)
            {
                PlayManager.EndOfAttack();
                attacking = false;
            }            
            nightTimeCounter += Time.deltaTime;
        }

        // increment the heal counter (night and day)
        healDelayCounter += Time.deltaTime;
    }

    /// <summary>
    /// HealSoldiers heals all not engaged and not dead soldiers of Amount and resets the healDelayCounter
    /// </summary>
    /// <param name="_hAmount">heal Amount</param>
    public void HealSoldiers(int _hAmount)
    {
        foreach (Soldier _s in PlayManager.soldierList)
        {
            if (!_s.IsEngaged && !_s.IsDead) _s.Heal(_hAmount);
        }
        healDelayCounter = 0f;
    }

    /// <summary>
    /// InstantiateHQCandidate method instantiates an HQCandidate building at the given position
    /// </summary>
    /// <param name="_position">Position where to create the HQQCandidate (Vector3)</param>
    public static void InstantiateHQCandidate(Vector3 _position)
    {
        GameObject _candidate = Instantiate(hqCandidateInstance, _position, Quaternion.identity, GameObject.Find("HQCandidates").transform);
        PlayManager.hqCandidateList.Add(_candidate.GetComponent<HQCandidate>());
        PlayManager.soldierNav.UpdateNavMesh(PlayManager.soldierNav.navMeshData);
        PlayManager.squadNav.UpdateNavMesh(PlayManager.squadNav.navMeshData);
    }
}
