using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// EnemySoldier is an enemy spawned by Towers
/// </summary>
public class EnemySoldier : Enemy
{
    // NavAgent of the enemy soldier
    private NavMeshAgent navAgent;

    /// <summary>
    /// At Start (when spawn), fetches the NavAgent and subscribes to events
    /// </summary>
    protected override void Start()
    {
        // Get the NavMeshAgent and enable it
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;

        // Add enemy to enemy list and subscribe remove to destruction event
        PlayManager.enemyList.Add(this);
        OnDestruction += delegate { PlayManager.enemyList.Remove(this); };

        // Subscribe to events
        EnableUpdate();
        PlayManager.OnEndDay += RemoveEnemy;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        OnDestruction -= delegate { PlayManager.enemyList.Remove(this); };
        PlayManager.OnEndDay -= RemoveEnemy;
        base.OnDestroy();
    }

    /// <summary>
    /// DestroyEnemy method sets the enemy as destroyed and triggers FX
    /// </summary>
    protected override void DestroyEnemy()
    {
        base.DestroyEnemy();
        GetComponentInChildren<MeshRenderer>().material.color = Color.black;
        //OnDestruction -= delegate { PlayManager.enemyList.Remove(this); };
        Destroy(this.gameObject, Time.deltaTime);
    }

    /// <summary>
    /// RemoveEnemy method destroys the enemy soldier without adding XP and removing it from the PlayManager
    /// </summary>
    protected void RemoveEnemy()
    {
        if (destroyed) return;
        destroyed = true;
        GameManager.PlayUpdate -= EnemyUpdate;
        // GFX
        OnDamage -= healthBar.UpdateValue;
        healthBar.Hide();
        healthBar.Remove();
        //OnDestruction -= delegate { PlayManager.enemyList.Remove(this); };
        Destroy(this.gameObject, Time.deltaTime);
    }

    /// <summary>
    /// Setup method initializes the enemy soldier data
    /// </summary>
    /// <param name="_hp">max hp of the soldier</param>
    /// <param name="_attack">attck of the soldier (short and middle ranges)</param>
    /// <param name="_defense">defense of the soldier (all values but explosive)</param>
    public void Setup(int _hp, int _attack, int _defense)
    {
        base.Setup();

        maxHP = _hp;
        hP = _hp;
        shortRangeAtk = _attack;
        middleRangeAtk = _attack;
        longRangeAtk = 0;
        shortRangeDef = _defense;
        middleRangeDef = _defense;
        longRangeDef = 0;
        shootingDataDuration = 3f;
        shootingDelay = shootingDataDuration;
        active = true;

        // Show the health bar at the init values
        SetupHealthBar(30f);
        healthBar.Show();
    }

    /// <summary>
    /// EnemyUpdate is the Update method of the class
    /// </summary>
    protected override void EnemyUpdate()
    {
        // Search for nearest SoldierUnit target
        Shootable _target = Ranges.GetNearestSoldier(this, shortRangeAtk, middleRangeAtk, longRangeAtk);
        if (_target != null && _target != selectedTarget) SetTarget(_target);

        if (selectedTarget == null)
        {
            navAgent.updateRotation = true;
            navAgent.isStopped = false;

            if(PlayManager.squadUnitList.Count > 0) // Test if PlayManager list is not empty to avoid setting an empty destination and creating an error
            {
                navAgent.SetDestination(GridAdjustment.GetGridCoordinates(Ranges.GetNearestSquad(transform).transform.position));
            }
        }
        else
        {
            // if there is a target, face it
            navAgent.updateRotation = false;
            FaceTarget(selectedTarget);

            // if target reachable
            if (Ranges.IsShootableShort(transform, selectedTarget))
            {
                navAgent.isStopped = true;
                // if target is in sight
                if (IsTargetInSight(selectedTarget))
                {
                    // shoot target
                    if (!destroyed) Shoot(selectedTarget);
                }
            }
            else
            {
                // Move towards the target
                navAgent.isStopped = false;
                navAgent.SetDestination(GridAdjustment.GetGridCoordinates(selectedTarget.transform.position));
            }
        }
        if (!navAgent.isStopped) healthBar.UpdatePosition();
        shootingDelay = Mathf.Max(0f, shootingDelay - Time.deltaTime);
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

    /// <summary>
    /// DamageLongRange method gets the damage amount and substract the shootable long range defense before calling Damage if still positive
    /// For soldier enemy, return if already destroyed and add XP for explosive damages done
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public override int DamageExplosive(int dmg)
    {
        if (destroyed) return 0;
        int _temp = base.DamageExplosive(dmg);
        PlayManager.AddXP(_temp);
        return _temp;
    }
}
