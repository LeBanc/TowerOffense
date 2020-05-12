using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : MonoBehaviour
{

    private NavMeshAgent navAgent;
    private SquadUnit squadUnit;
    private Soldier soldier;
    private Tower target;
    private Tower secondaryTarget;
    private SoldierData data;
    private int hP;
    private HealthBar healthBar;

    private bool wounded = false;

    public ParticleSystem fireParticules;
    public AudioSource fireAudio;

    // Events
    public delegate void SoldierUnitEventHandler();
    public event SoldierUnitEventHandler OnWounded;

    // temp
    private float shootingDelay;
    private float shootingDataDuration;

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

    /// <summary>
    /// Soldier setup sets the parent squad, the soldier's navMeshAgent and the events used
    /// </summary>
    /// <param name="_squadUnit"></param>
    public void Setup(SquadUnit _squadUnit, Soldier _soldier)
    {
        squadUnit = _squadUnit;
        
        soldier = _soldier;
        data = soldier.Data;
        hP = soldier.CurrentHP;

        healthBar = PlayManager.AddHealthBar(transform, 30f);
        healthBar.UpdateValue(hP,data.maxHP);

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;

        fireAudio.clip = data.shootingSound;
        shootingDataDuration = data.shootingDelay;
        shootingDelay = shootingDataDuration;

        // Invoke is used as a workaround for enabling NavMeshAgent on NavMeshSurface
        Invoke("SetEvents", 0.025f);
    }

    private void SetEvents()
    {
        navAgent.enabled = true;

        squadUnit.OnTargetChange += SetTarget;
        squadUnit.OnDeath += Die;
        squadUnit.OnHQBack += BackToHQ;
        OnWounded += squadUnit.CheckDeath;
        
        GameManager.PlayUpdate += SoldierUpdate;
    }

    /// <summary>
    /// Clears event links when soldier is destroyed
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

    public void Die()
    {
        // Remove HealthBar
        healthBar.Remove();
        soldier.Die();
        Destroy(gameObject);
    }

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
    /// SetTarget is used to define the soldier's target, it is called when the squad OnTargetChange event is triggered
    /// </summary>
    /// <param name="_target"></param>
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

    private void ClearTarget()
    {
        if (target != null) target.OnDestruction -= ClearTarget;
        target = null;
    }

    private void SetSecondaryTarget(Tower _target)
    {
        if (secondaryTarget != null) ClearSecondaryTarget();
        secondaryTarget = _target;
        if(secondaryTarget != null)
        {
            secondaryTarget.OnDestruction += ClearSecondaryTarget;
        }
    }

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
                Tower _t = Ranges.GetNearestTower(this.transform, data.shortRangeAttack, data.middleRangeAttack, data.longRangeAttack);
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
    private bool IsTargetShootable(Tower _t)
    {
        return (data.shortRangeAttack > 0 && Ranges.IsShootableShort(transform, _t)) ||
            (data.middleRangeAttack > 0 && Ranges.IsShootableMiddle(transform, _t)) ||
            (data.longRangeAttack > 0 && Ranges.IsShootableLong(transform, _t));
    }

    private bool IsTargetInSight(Tower _t)
    {
        Vector3 targetDir = _t.transform.position - transform.position;
        float angle = Vector3.Angle(new Vector3(targetDir.x,0f,targetDir.z), transform.forward);
        return (Mathf.Abs(angle) <= 30f);
    }

    private void Shoot(Tower _t)
    {
        if (shootingDelay > 0f) return;
        
        if (Ranges.IsInShortRange(transform, _t))
        {
            _t.DamageShortRange(data.shortRangeAttack);
        }
        else if(Ranges.IsInMiddleRange(transform, _t))
        {
            _t.DamageMiddleRange(data.middleRangeAttack);
        }
        else if(Ranges.IsInLongRange(transform, _t))
        {
            _t.DamageLongRange(data.longRangeAttack);
        }
        else
        {
            return;
        }

        fireParticules.Play();
        fireAudio.pitch = Random.Range(0.8f, 1.2f);
        fireAudio.Play();
        shootingDelay = shootingDataDuration;
        
        Invoke("StopFireParticules", 0.3f);
    }

    private void StopFireParticules()
    {
        fireParticules.Stop();
    }
    #endregion

    #region Damages
    public void DamageShortRange(int dmg)
    {
        int _temp = dmg - data.shortRangeDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    public void DamageMiddleRange(int dmg)
    {
        int _temp = dmg - data.middleRangeDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    public void DamageLongRange(int dmg)
    {
        int _temp = dmg - data.longRangeDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    public void DamageExplosive(int dmg)
    {
        int _temp = dmg - data.explosiveDefense;
        if (_temp > 0) DamageSoldier(_temp);
    }

    private void DamageSoldier(int dmg)
    {
        hP -= dmg;
        if (hP <= 0)
        {
            hP = 0;
            WoundSoldier();
            OnWounded?.Invoke();
        }
        healthBar.UpdateValue(hP, data.maxHP);
    }

    private void WoundSoldier()
    {
        wounded = true;
        GetComponent<MeshRenderer>().material.color = Color.black;
    }
    #endregion
}
