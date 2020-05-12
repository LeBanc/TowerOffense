using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : MonoBehaviour
{
    private Vector3[] spawnPoints;
    private Vector3[] positions;

    private float dayTimeCounter = 0f;
    private float nightTimeCounter = 0f;
    private float healDelayCounter = 0f;
    private float healDelay;
    private int healAmount;

    private float attackTime;
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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SetSpawnPoints();
        PlayManager.LoadSquadsOnNewDay += InstantiateSquads;
    }

    private void OnDestroy()
    {
        PlayManager.LoadSquadsOnNewDay -= InstantiateSquads;
        GameManager.PlayUpdate -= HQUpdate;
    }

    #region SpawPoints
    /// <summary>
    /// SetSpawnPoints looks for each SpawnPoints if the cell is empty until it finds 4 available cells
    /// </summary>
    private void SetSpawnPoints()
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
            if (IsSpawnPoint(transform.position + positions[_posIndex]))
            {
                spawnPoints[_index] = transform.position + positions[_posIndex];
                _index++;
            }
            _posIndex++;
        }
        if (_index < 4) Debug.LogError("[HQ] Cannot find 4 spawn points!");
    }

    /// <summary>
    /// IsSpawnPoints returns the avaibility of the transform position to be a spawn point
    /// </summary>
    /// <param name="position">Spawn point position</param>
    /// <returns>True if the cell is an available spawn point, false otherwise</returns>
    private bool IsSpawnPoint(Vector3 position)
    {
        Ray _ray = new Ray(position + Vector3.up * 20, -Vector3.up);
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Buildings")))
        {
            if (_hit.collider.gameObject.CompareTag("Terrain"))
            {
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
        int _index = 0;
        foreach(Squad _squad in PlayManager.squadList)
        {
            if (_squad.isEngaged)
            {
                SquadUnit _su = _squad.InstanciateSquadUnit(spawnPoints[_index]);
                _index++;
                PlayManager.squadUnitList.Add(_su);
            }
        }

        healDelay = (attackTime / HealAmount);

        GameManager.PlayUpdate += HQUpdate;
        attacking = true;

        healDelayCounter = 0f;
        dayTimeCounter = 0f;
        nightTimeCounter = 0f;
    }

    public void EndDayAtHQ()
    {
        GameManager.PlayUpdate -= HQUpdate;
        attacking = false;
        int _heal = (int)Mathf.Floor((180f-attackTime - nightTimeCounter)*healAmount/(180f-attackTime));
        Debug.Log("Enf of attack turn : heal soldiers of " + _heal + " HP.");
        HealSoldiers(_heal);
    }
    

    public void HQUpdate()
    {
        if (healDelayCounter >= healDelay)
        {
            HealSoldiers(1);
        }

        if (dayTimeCounter <= attackTime)
        {
            dayTimeCounter += Time.deltaTime;
        }
        else
        {
            if (attacking)
            {
                PlayManager.EndOfAttack();
                attacking = false;
            }            
            nightTimeCounter += Time.deltaTime;
        }
        healDelayCounter += Time.deltaTime;
    }

    public void HealSoldiers(int _hAmount)
    {
        foreach (Soldier _s in PlayManager.soldierList)
        {
            if (!_s.IsEngaged() && !_s.IsDead()) _s.Heal(_hAmount);
        }
        healDelayCounter = 0f;
    }
}
