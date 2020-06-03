using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// SoldierUnit is the class that represents a Soldier in an attack session
/// </summary>
public class SoldierUnit : MonoBehaviour
{
    // Soldier represented by this SoldierUnit
    private Soldier soldier;

    // SqaudUnit in which the SoldierUnit is
    private SquadUnit squadUnit;

    // NavMeshAgent of this SoldierUnit
    private NavMeshAgent navAgent;
        
    // Targets
    private Tower target;
    private Tower secondaryTarget;

    // Attack
    private float shootingDelay;
    private float shootingDataDuration;
    // Attack VFX & SFX
    public ParticleSystem fireParticules;
    public AudioSource fireAudio;

    // Health
    private int hP;
    private HealthBar healthBar;
    private bool wounded = false;
 
    // Events
    public delegate void SoldierUnitEventHandler();
    public event SoldierUnitEventHandler OnWounded;

    public delegate void SoldierUnitDamageEventHandler(int hp, int maxHP);
    public event SoldierUnitDamageEventHandler OnDamage;

    #region Properties access
    public SquadUnit Squad
    {
        get { return squadUnit; }
    }

    public int HP
    {
        get { return hP; }
    }

    public bool IsWounded()
    {
        return wounded;
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
        healthBar = PlayManager.AddHealthBar(transform, 30f);
        OnDamage += healthBar.UpdateValue;

        // NavAgent        
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;
        navAgent.speed = soldier.Speed * navAgent.speed / 100; // Cross product 100% speed is default navAgent speed

        // Shooting
        fireAudio.clip = soldier.Data.shootingSound;
        shootingDataDuration = soldier.Data.shootingDelay;
        shootingDelay = shootingDataDuration;

        // Invoke is used as a workaround for enabling NavMeshAgent on NavMeshSurface (did this after Unity errors)
        Invoke("SetEvents", 0.025f);
    }

    /// <summary>
    /// SetEvents method follows the Setup. It enables the NavMeshAgent and subscribes to all needed events
    /// </summary>
    private void SetEvents()
    {
        navAgent.enabled = true;

        squadUnit.OnTargetChange += SetTarget;
        squadUnit.OnDeath += Die;
        squadUnit.OnHQBack += BackToHQ;
        OnWounded += squadUnit.CheckDeath;
        
        GameManager.PlayUpdate += SoldierUpdate;

        OnDamage?.Invoke(hP, soldier.MaxHP);
    }

    /// <summary>
    /// OnDestroy method unsubscribes from all events when soldier is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe events
        GameManager.PlayUpdate -= SoldierUpdate;
        squadUnit.OnTargetChange -= SetTarget;
        OnWounded -= squadUnit.CheckDeath;
        squadUnit.OnDeath -= Die;
        squadUnit.OnHQBack -= BackToHQ;
    }

    /// <summary>
    /// Die method is called when the SoldierUnit dies.
    /// It deletes the HealthBar and calls the Die method of the Soldier befor destoying this SoldierUnit
    /// </summary>
    public void Die()
    {
        // Remove HealthBar
        healthBar.Remove();
        soldier.Die();
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
        navAgent.SetDestination(_destination);
    }

    /// <summary>
    /// SetTarget is used to define the soldier's target
    /// </summary>
    /// <param name="_target">New Tower target</param>
    private void SetTarget(Tower _target)
    {
        if (target != null) ClearTarget();
        target = _target;
        if(target != null)
        {
            target.OnDestruction += ClearTarget;
            ClearSecondaryTarget();
        }
    }

    /// <summary>
    /// ClearTarget clears the current target and unsubscribes from event if needed
    /// </summary>
    private void ClearTarget()
    {
        if (target != null) target.OnDestruction -= ClearTarget;
        target = null;
    }

    /// <summary>
    /// SetSecondaryTarget is used to define the soldier's secondary target
    /// </summary>
    /// <param name="_target">New Tower target</param>
    private void SetSecondaryTarget(Tower _target)
    {
        if (secondaryTarget != null) ClearSecondaryTarget();
        secondaryTarget = _target;
        if(secondaryTarget != null)
        {
            secondaryTarget.OnDestruction += ClearSecondaryTarget;
        }
    }

    /// <summary>
    /// ClearSecondaryTarget clears the secondary target and unsubscribes from event if needed
    /// </summary>
    private void ClearSecondaryTarget()
    {
        if(secondaryTarget != null) secondaryTarget.OnDestruction -= ClearSecondaryTarget;
        secondaryTarget = null;
    }

    /// <summary>
    /// FaceTarget method is used to rotate the soldier toward its target
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

    /// <summary>
    /// SoldierUpdate is the Update method of Soldier, it changes the soldier forward direction setting if a target is set or not
    /// </summary>
    void SoldierUpdate()
    {
        if (target == null)
        {
            navAgent.updateRotation = true;
        }
        else
        {
            navAgent.updateRotation = false;
            // if target reachable
            if (IsTargetShootable(target))
            {
                FaceTarget(target);
                if (IsTargetInSight(target))
                {
                    // shoot target
                    if(!wounded)Shoot(target);
                }
            }
            else
            {
                // if a secondary target can be found
                Tower _t = Ranges.GetNearestTower(this.transform, soldier.ShortRangeAttack, soldier.MiddleRangeAttack, soldier.LongRangeAttack);
                if (_t != null)
                {
                    if(secondaryTarget != _t) SetSecondaryTarget(_t);
                    FaceTarget(secondaryTarget);
                    if (IsTargetInSight(secondaryTarget))
                    {
                        // shoot secondary target
                        if (!wounded) Shoot(secondaryTarget);
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

        if (!navAgent.isStopped) healthBar.UpdatePosition();

        shootingDelay = Mathf.Max(0f, shootingDelay - Time.deltaTime);
    }

    #region Shooting
    /// <summary>
    /// IsTargetShootable retruns true if the Tower target is in shootable range
    /// </summary>
    /// <param name="_t">Targeted Tower</param>
    /// <returns>Shootable or not</returns>
    private bool IsTargetShootable(Tower _t)
    {
        return (soldier.ShortRangeAttack > 0 && Ranges.IsShootableShort(transform, _t)) ||
            (soldier.MiddleRangeAttack > 0 && Ranges.IsShootableMiddle(transform, _t)) ||
            (soldier.LongRangeAttack > 0 && Ranges.IsShootableLong(transform, _t));
    }

    /// <summary>
    /// IsTargetInSight returns true if the Tower target is seen by the SoldierUnit
    /// </summary>
    /// <param name="_t">Targeted Tower</param>
    /// <returns>InSight or not</returns>
    private bool IsTargetInSight(Tower _t)
    {
        Vector3 targetDir = _t.transform.position - transform.position;
        float angle = Vector3.Angle(new Vector3(targetDir.x,0f,targetDir.z), transform.forward);
        return (Mathf.Abs(angle) <= 30f);
    }

    /// <summary>
    /// Shoot method checks if the shooting delay has expired and shoots if it has
    /// </summary>
    /// <param name="_t"></param>
    private void Shoot(Tower _t)
    {
        // Check shooting delay
        if (shootingDelay > 0f) return;
        
        // Check at each range the target is and calls the dedicated damage method
        // This is used to oppose Soldier RangeAttack values to target RangeDefense ones
        if (Ranges.IsInShortRange(transform, _t))
        {
            _t.DamageShortRange(soldier.ShortRangeAttack);
        }
        else if(Ranges.IsInMiddleRange(transform, _t))
        {
            _t.DamageMiddleRange(soldier.MiddleRangeAttack);
        }
        else if(Ranges.IsInLongRange(transform, _t))
        {
            _t.DamageLongRange(soldier.LongRangeAttack);
        }
        else
        {
            return;
        }

        // Launch VFX
        fireParticules.Play();
        Invoke("StopFireParticules", 0.3f);

        // Launch SFX
        fireAudio.pitch = Random.Range(0.8f, 1.2f);
        fireAudio.Play();

        // Reset shootingDelay
        shootingDelay = shootingDataDuration;
    }

    /// <summary>
    /// StopFireParticules method is used to stop shooting VFX after a delay (with Invoke)
    /// </summary>
    private void StopFireParticules()
    {
        fireParticules.Stop();
    }
    #endregion

    #region Damages
    /// <summary>
    /// DamageShortRange method gets the damage amount and substract the soldier short range defense before calling DamageSoldier if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public void DamageShortRange(int dmg)
    {
        int _temp = dmg - soldier.ShortRangeDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    /// <summary>
    /// DamageMiddleRange method gets the damage amount and substract the soldier middle range defense before calling DamageSoldier if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public void DamageMiddleRange(int dmg)
    {
        int _temp = dmg - soldier.MiddleRangeDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    /// <summary>
    /// DamageLongRange method gets the damage amount and substract the soldier long range defense before calling DamageSoldier if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public void DamageLongRange(int dmg)
    {
        int _temp = dmg - soldier.LongRangeDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    /// <summary>
    /// DamageExplosive method gets the damage amount and substract the soldier explosives defense before calling DamageSoldier if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public void DamageExplosive(int dmg)
    {
        int _temp = dmg - soldier.ExplosivesDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    /// <summary>
    /// DamageSoldier reduces the soldier HP of Damage amount and trigger a wound if soldier HP are at 0 or under
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    private void DamageSoldier(int dmg)
    {
        hP -= dmg;
        if (hP <= 0)
        {
            hP = 0;
            WoundSoldier();
            OnWounded?.Invoke();
        }
        OnDamage?.Invoke(hP, soldier.MaxHP);
    }

    /// <summary>
    /// WoundSoldier method sets the SoldierUnit as wounded and reduces its speed
    /// </summary>
    private void WoundSoldier()
    {
        wounded = true;
        GetComponent<MeshRenderer>().material.color = Color.black;
        navAgent.speed = 0.9f * navAgent.speed; // Wounded soldier is slower
    }
    #endregion

    /// <summary>
    /// Heal method adds HP amount to the SoldierUnit. If it was wounded, not anymore
    /// </summary>
    /// <param name="_amount"></param>
    public void Heal(int _amount)
    {
        hP += _amount;
        if (hP > soldier.MaxHP) hP = soldier.MaxHP;
        if (wounded)
        {
            wounded = false;
            GetComponent<MeshRenderer>().material.color = Color.white;
            navAgent.speed = navAgent.speed / 0.9f; // Healed agent get back initial speed
        }
        OnDamage?.Invoke(hP, soldier.MaxHP);
    }
}
