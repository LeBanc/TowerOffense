using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turret class is a Shootable for turrets
/// </summary>
public class Turret : Shootable
{
    // private bool for destroyed state
    private bool isDestroyed;
    // private int for number of turns the turret is alive
    private int turns = 2;

    public int Turns
    {
        get { return turns; }
    }

    /// <summary>
    /// At Start, set the Shootable parameters, subscribe to events and activate the turret
    /// </summary>
    private void Start()
    {
        if(hP == 0) hP = PlayManager.data.turretData.maxHP;
        maxHP = PlayManager.data.turretData.maxHP;
        shortRangeAtk = PlayManager.data.turretData.shortRangeAttack;
        middleRangeAtk = PlayManager.data.turretData.middleRangeAttack;
        longRangeAtk = PlayManager.data.turretData.longRangeAttack;
        explosiveAtk = 0;
        shortRangeDef = PlayManager.data.turretData.shortRangeDefense;
        middleRangeDef = PlayManager.data.turretData.middleRangeDefense;
        longRangeDef = PlayManager.data.turretData.longRangeDefense;
        explosiveDef = PlayManager.data.turretData.explosiveDefense;
        shootingDataDuration = PlayManager.data.turretData.shootingDelay;

        OnHPDown += DestroyTurret;
        PlayManager.OnLoadSquadsOnNewDay += Activate;
        PlayManager.OnEndDay += EndOfTurn;
        PlayManager.OnEndDay += Deactivate;

        SetupHealthBar(30f);
        RaiseOnDamage(hP, maxHP);

        PlayManager.turretList.Add(this);
    }

    /// <summary>
    /// OnDestroy, deactivate the turret and unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        Deactivate();

        PlayManager.turretList.Remove(this);

        OnHPDown -= DestroyTurret;
        PlayManager.OnLoadSquadsOnNewDay -= Activate;
        PlayManager.OnEndDay -= EndOfTurn;
        PlayManager.OnEndDay -= Deactivate;

        base.OnDestroy();
    }

    /// <summary>
    /// SetActive method is used from other class to Activate the turret with a delay (to ensure the Start method was done before)
    /// </summary>
    public void SetActive()
    {
        Invoke("Activate", Time.deltaTime);
    }

    /// <summary>
    /// Activate method clear the turret target, subscribe to PlayUpdate and show the HealthBar
    /// </summary>
    private void Activate()
    {
        selectedTarget = null;
        GameManager.PlayUpdate += TurretUpdate;
        healthBar.Show();
    }

    /// <summary>
    /// Deactivate method unsubscribe from PlayUpdate and hide the HealthBar
    /// </summary>
    private void Deactivate()
    {
        GameManager.PlayUpdate -= TurretUpdate;
        if(healthBar != null) healthBar.Hide();
    }

    /// <summary>
    /// TurretUpdate method search for a target, turn the turret and shoot
    /// </summary>
    private void TurretUpdate()
    {
        FindTarget();
        if(selectedTarget != null)
        {
            if (IsTargetShootable(selectedTarget))
            {
                FaceTarget(selectedTarget);
                if (IsTargetInSight(selectedTarget)) Shoot(selectedTarget);
            }
        }
        shootingDelay = Mathf.Max(0f, shootingDelay - Time.deltaTime);
    }

    /// <summary>
    /// FindTarget method finds the nearest enemy and set the target
    /// </summary>
    private void FindTarget()
    {
        Enemy _enemy = Ranges.GetNearestEnemy(this.transform, shortRangeAtk, middleRangeAtk, longRangeAtk);
        if (_enemy != null)
        {
            if(_enemy != selectedTarget) SetTarget(_enemy);
        }
    }

    /// <summary>
    /// FaceTarget method turns the turret to face the selected target
    /// </summary>
    /// <param name="_target"></param>
    private void FaceTarget(Shootable _target)
    {
        // Gets the projection on XZ plane of the vector between target and squad positions
        Vector3 _diff = _target.transform.position - transform.position;
        _diff = new Vector3(_diff.x, 0f, _diff.z);
        // Gets the angle between the _diff vector and the forward squad axe
        float _angle = Vector3.SignedAngle(transform.forward, Vector3.Normalize(_diff), transform.up);
        // Clamps that angle value to a max angle of 40°/sec and rotates the squad
        _angle = Mathf.Clamp(_angle, -40f * Time.deltaTime, 40f * Time.deltaTime);
        transform.Rotate(transform.up, _angle);
    }

    /// <summary>
    /// EndOfTurn method decrement the turns integer and destroy the turret if it reaches 0
    /// </summary>
    private void EndOfTurn()
    {
        turns--;
        if (turns <= 0) DestroyTurret();
    }

    /// <summary>
    /// DestroyTurret method removes the turret from the PlayManager turrets list and destroy the turret
    /// </summary>
    private void DestroyTurret()
    {
        if(!isDestroyed)
        {
            isDestroyed = true;
            PlayManager.turretList.Remove(this);
            Destroy(gameObject);
        }        
    }

    /// <summary>
    /// GetAvailablePositions method search for the available positions (terrain, not building, not enemies) around the turret
    /// </summary>
    /// <returns>List of the available positions around the turret (List<Vector3>)</returns>
    public List<Vector3> GetAvailablePositions()
    {
        List<Vector3> _availableCells = new List<Vector3>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                Vector3 _cell = new Vector3((float)10 * i, 0f, (float)10 * j);

                RaycastHit _hit;
                Ray _ray = new Ray(GridAdjustment.GetGridCoordinates(transform.position) + _cell + 100 * Vector3.up, -Vector3.up);
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain", "Buildings", "Enemies" })))
                {
                    if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {
                        _hit = new RaycastHit();
                        _ray = new Ray(transform.position, (_cell - 7.5f * Vector3.up).normalized);
                        if (!Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Enemies" })))
                        {
                            if (_cell.magnitude <= PlayManager.ShortRange) _availableCells.Add(GridAdjustment.GetGridCoordinates(transform.position + _cell));
                        }
                    }
                }
            }
        }

        foreach (SquadUnit _su in PlayManager.squadUnitList)
        {
            _availableCells.Remove(GridAdjustment.GetGridCoordinates(_su.Destination));
        }

        return _availableCells;
    }

    /// <summary>
    /// LoadData method changes the turret data and updates its value
    /// </summary>
    /// <param name="_hp">Current turret HP</param>
    /// <param name="_turns">Current turret turns remaining</param>
    public void LoadData(int _hp, int _turns)
    {
        hP = _hp;
        turns = _turns;
    }
}
