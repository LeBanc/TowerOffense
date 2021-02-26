using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// SoldierUnit is the class that represents a Soldier in an attack session
/// </summary>
public class SoldierUnit : Shootable
{
    // Soldier represented by this SoldierUnit
    private Soldier soldier;

    // SquadUnit in which the SoldierUnit is
    private SquadUnit squadUnit;

    // NavMeshAgent of this SoldierUnit
    private NavMeshAgent navAgent;
        
    // Targets
    private Enemy secondaryTarget;

    // Health
    private bool wounded = false;
    private bool isBuilding = false;
 
    // Events
    public event ShootableEventHandler OnWounded;

    #region Properties access
    public SquadUnit Squad
    {
        get { return squadUnit; }
    }

    public Soldier Soldier
    {
        get { return soldier; }
    }

    public bool IsWounded
    {
        get{ return wounded; }
    }

    public bool IsBuilding
    {
        get { return isBuilding; }
        set { isBuilding = value; }
    }
    #endregion

    /// <summary>
    /// Soldier setup sets the parent squad, the soldier's navMeshAgent and the events used
    /// </summary>
    /// <param name="_squadUnit">SquadUnit the soldier is into</param>
    /// <param name="_soldier">Soldier creating this SoldierUnit</param>
    public void Setup(SquadUnit _squadUnit, Soldier _soldier)
    {
        // Squad
        squadUnit = _squadUnit;
        
        // Soldier
        soldier = _soldier;

        // Health
        hP = soldier.CurrentHP;
        maxHP = soldier.MaxHP;
        SetupHealthBar(30f);
        OnHPDown += WoundSoldier;

        // Attack values
        shortRangeAtk = soldier.ShortRangeAttack;
        middleRangeAtk = soldier.MiddleRangeAttack;
        longRangeAtk = soldier.LongRangeAttack;

        // Defense values
        shortRangeDef = soldier.ShortRangeDefense;
        middleRangeDef = soldier.MiddleRangeDefense;
        longRangeDef = soldier.LongRangeDefense;
        explosiveDef = soldier.ExplosivesDefense;

        // NavAgent        
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = soldier.Speed * navAgent.speed / 100; // Cross product 100% speed is default navAgent speed
        navAgent.enabled = true;

        // Shooting
        fireSFX.clip = soldier.Data.shootingSound;
        shootingDataDuration = soldier.Data.shootingDelay;
        shootingDelay = shootingDataDuration;

        // Setup events
        squadUnit.OnTargetChange += SetTarget;
        squadUnit.OnDeath += Die;
        squadUnit.OnHQBack += BackToHQ;
        OnWounded += squadUnit.CheckDeath;
        GameManager.PlayUpdate += SoldierUpdate;

        // Invoke is used to set up the health bar after a small delay
        Invoke("HealthBarInit", 0.025f);
    }

    /// <summary>
    /// HealthBarInit method follows the Setup. It initializes the health bar
    /// </summary>
    private void HealthBarInit()
    {
        RaiseOnDamage(hP, maxHP);
    }

    /// <summary>
    /// OnDestroy method unsubscribes from all events when soldier is destroyed
    /// </summary>
    protected override void OnDestroy()
    {
        // Unsubscribe events
        GameManager.PlayUpdate -= SoldierUpdate;
        squadUnit.OnTargetChange -= SetTarget;
        OnWounded -= squadUnit.CheckDeath;
        squadUnit.OnDeath -= Die;
        squadUnit.OnHQBack -= BackToHQ;
        OnHPDown -= WoundSoldier;
        base.OnDestroy();
    }

    /// <summary>
    /// Die method is called when the SoldierUnit dies.
    /// It deletes the HealthBar and calls the Die method of the Soldier befor destoying this SoldierUnit
    /// </summary>
    public void Die()
    {
        // Remove HealthBar
        OnDamage -= healthBar.UpdateValue;
        healthBar.Remove();
        soldier.Die();
        SendMessage("DieMessage");
        Destroy(gameObject);
    }

    /// <summary>
    /// BackToHQ method removes the SoldierUnt from city when returning to the HQ
    /// </summary>
    public void BackToHQ()
    {
        // Remove HealthBar
        healthBar.Remove();
        soldier.CurrentHP = hP;
        soldier.Engage(false);
        Destroy(gameObject);
    }

    #region Move and Target
    /// <summary>
    /// SetDestination sets the soldier's NavMeshAgent SetDestination (easily called by Squad this way)
    /// </summary>
    /// <param name="_destination"></param>
    public void SetDestination(Vector3 _destination)
    {
        if(navAgent != null) navAgent.SetDestination(_destination);
    }

    /// <summary>
    /// SetSecondaryTarget is used to define the soldier's secondary target
    /// </summary>
    /// <param name="_target">New Tower target</param>
    private void SetSecondaryTarget(Enemy _target)
    {
        if (secondaryTarget != null) ClearSecondaryTarget();
        secondaryTarget = _target;
        if(secondaryTarget != null)
        {
            secondaryTarget.OnHPDown += ClearSecondaryTarget;
        }
    }

    /// <summary>
    /// ClearSecondaryTarget clears the secondary target and unsubscribes from event if needed
    /// </summary>
    private void ClearSecondaryTarget()
    {
        if(secondaryTarget != null) secondaryTarget.OnHPDown -= ClearSecondaryTarget;
        secondaryTarget = null;
    }

    /// <summary>
    /// FaceTarget method is used to rotate the soldier toward its target
    /// </summary>
    /// <param name="_target"></param>
    private void FaceTarget(Shootable _target)
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
    /// SoldierUpdate is the Update method of Soldier, it changes the soldier forward direction setting if a target is set or not
    /// </summary>
    void SoldierUpdate()
    {
        if (selectedTarget == null)
        {
            navAgent.updateRotation = true;
        }
        else
        {
            navAgent.updateRotation = false;
            // if target reachable
            if (IsTargetShootable(selectedTarget))
            {
                FaceTarget(selectedTarget);
                if (IsTargetInSight(selectedTarget))
                {
                    // shoot target
                    if (!wounded && !isBuilding)
                    {
                        Shoot(selectedTarget);
                    }
                }
            }
            else
            {
                // if a secondary target can be found
                Enemy _enemy = Ranges.GetNearestEnemy(this.transform, shortRangeAtk, middleRangeAtk, longRangeAtk);
                if (_enemy != null)
                {
                    if(secondaryTarget != _enemy) SetSecondaryTarget(_enemy);
                    FaceTarget(secondaryTarget);
                    if (IsTargetInSight(secondaryTarget))
                    {
                        // shoot secondary target
                        if (!wounded && !isBuilding)
                        {
                            Shoot(secondaryTarget);
                        }
                    }
                }
                else
                {
                    //else (no secondary target found)
                    if (secondaryTarget != null) ClearSecondaryTarget();
                    navAgent.updateRotation = true;
                }
            }
        }

        SendMessage("UpdateVelocityMessage", transform.InverseTransformDirection(navAgent.velocity));
        if (!navAgent.isStopped) healthBar.UpdatePosition();

        shootingDelay = Mathf.Max(0f, shootingDelay - Time.deltaTime);
        
        SendMessage("BuildMessage", isBuilding);
        
        isBuilding =  false;
    }

    #region Damages
    /// <summary>
    /// WoundSoldier method sets the SoldierUnit as wounded and reduces its speed
    /// </summary>
    private void WoundSoldier()
    {
        if (wounded) return;
        wounded = true;
        OnWounded?.Invoke();
        // Wounded soldier is slower and slows down the squad
        navAgent.speed = 0.9f * navAgent.speed;
        squadUnit.Speed = 0.9f * squadUnit.Speed;
        SendMessage("WoundedMessage", true);
    }
    #endregion

    /// <summary>
    /// Heal method adds HP amount to the SoldierUnit. If it was wounded, not anymore
    /// </summary>
    /// <param name="_amount"></param>
    public void Heal(int _amount)
    {
        hP += _amount;
        if (hP > maxHP) hP = maxHP;
        if (wounded)
        {
            wounded = false;

            // Healed soldier gets back his/her initial speed and so does the squad
            navAgent.speed = navAgent.speed / 0.9f;
            squadUnit.Speed = squadUnit.Speed / 0.9f;

            SendMessage("WoundedMessage", false);
        }
        RaiseOnDamage(hP, maxHP);
    }
}
