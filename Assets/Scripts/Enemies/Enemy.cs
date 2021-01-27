using UnityEngine;

/// <summary>
/// Enemy class is a Shootable for enemies
/// </summary>
public class Enemy : Shootable
{
    // Enemy state
    protected bool active = true;
    protected bool destroyed = false;

    // Enemy events
    public event ShootableEventHandler OnDestruction;

    /// <summary>
    /// At Start, initialize the enemy
    /// </summary>
    protected virtual void Start()
    {
        // Default values
        maxHP = 50;
        hP = maxHP;
        shortRangeAtk = 0;
        middleRangeAtk = 0;
        longRangeAtk = 0;
        explosiveAtk = 0;
        shortRangeDef = 0;
        middleRangeDef = 0;
        longRangeDef = 0;
        explosiveDef = 0;
        shootingDataDuration = 5f;
        shootingDelay = shootingDataDuration;
    }    

    /// <summary>
    /// Setup method subscribes the DestroyEnemy method to OnHPDown event
    /// </summary>
    public virtual void Setup()
    {
        OnHPDown += DestroyEnemy;
    }

    /// <summary>
    /// OnDestroy, unsubscribes from events
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.PlayUpdate -= EnemyUpdate;
        OnHPDown -= DestroyEnemy;
    }

    /// <summary>
    /// EnableUpdate method subscribes to the PlayUpdate event and resets counters
    /// </summary>
    protected virtual void EnableUpdate()
    {
        // Subscribe to events
        GameManager.PlayUpdate += EnemyUpdate;
        // Reset counter
        shootingDelay = shootingDataDuration;
    }

     /// <summary>
    /// DestroyEnemy method is used to set the enemy as destroyed
    /// </summary>
    protected virtual void DestroyEnemy()
    {
        if (destroyed) return;
        hP = 0;
        PlayManager.AddAttackWorkforce(Mathf.Max(1,maxHP/50));
        OnDestruction?.Invoke();
        destroyed = true;
        GameManager.PlayUpdate -= EnemyUpdate;

        // GFX
        OnDamage -= healthBar.UpdateValue;
        healthBar.Hide();
        healthBar.Remove();
    }

    /// <summary>
    /// IsActive method returns true if tower is active and false otherwise
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return active;
    }

    /// <summary>
    /// IsDestroyed method returns the destroy boolean
    /// </summary>
    /// <returns>True if destroyed, false otherwise</returns>
    public bool IsDestroyed()
    {
        return destroyed;
    }   

    /// <summary>
    /// EnemyUpdate is the Update methods of the Enemy
    /// </summary>
    protected virtual void EnemyUpdate()
    {
        
    }

    /// <summary>
    /// DamageShortRange method gets the damage amount and substract the shootable short range defense before calling Damage if still positive
    /// For enemy, return if already destroyed and add XP for damages done (short, middle, long but not explosives)
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public override int DamageShortRange(int dmg)
    {
        if (destroyed) return 0;
        int _temp = base.DamageShortRange(dmg);
        PlayManager.AddXP(_temp);
        return _temp;
    }

    /// <summary>
    /// DamageMiddleRange method gets the damage amount and substract the shootable middle range defense before calling Damage if still positive
    /// For enemy, return if already destroyed and add XP for damages done (short, middle, long but not explosives)
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public override int DamageMiddleRange(int dmg)
    {
        if (destroyed) return 0;
        int _temp = base.DamageMiddleRange(dmg);
        PlayManager.AddXP(_temp);
        return _temp;
    }

    /// <summary>
    /// DamageLongRange method gets the damage amount and substract the shootable long range defense before calling Damage if still positive
    /// For enemy, return if already destroyed and add XP for damages done (short, middle, long but not explosives)
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public override int DamageLongRange(int dmg)
    {
        if (destroyed) return 0;
        int _temp = base.DamageLongRange(dmg);
        PlayManager.AddXP(_temp);
        return _temp;
    }
}
