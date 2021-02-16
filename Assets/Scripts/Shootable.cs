using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shootable class is used to define an object is Shootable (friend or foe)
/// </summary>
public class Shootable : MonoBehaviour
{
    // Public list of hit points
    public List<Transform> hitList = new List<Transform>();

    // Main properties
    protected int hP;
    protected int maxHP;
    protected int shortRangeAtk;
    protected int middleRangeAtk;
    protected int longRangeAtk;
    protected int explosiveAtk;
    protected int shortRangeDef;
    protected int middleRangeDef;
    protected int longRangeDef;
    protected int explosiveDef;

    // Target & shooting
    protected Shootable selectedTarget;
    protected float shootingDelay;
    protected float shootingDataDuration;

    // SFX & VFX
    public AudioSource fireSFX;
    public ParticleSystem fireParticles;

    // HealthBar
    protected HealthBar healthBar;

    // Events
    public delegate void ShootableDamageEventHandler(int hp, int maxHP);
    public event ShootableDamageEventHandler OnDamage;
    public delegate void ShootableEventHandler();
    public event ShootableEventHandler OnHPDown;

    public int HP
    {
        get { return hP; }
    }

    /// <summary>
    /// On Awake, if there is no hit points in the list, add the object Transform as default value
    /// </summary>
    protected virtual void Awake()
    {
        if (hitList.Count <= 0) hitList.Add(transform);
    }

    /// <summary>
    /// OnDestroy, remove the health bar
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (healthBar != null)
        {
            OnDamage -= healthBar.UpdateValue;
            healthBar.Remove();
        }
    }

    #region HealthBar
    /// <summary>
    /// SetupHealthBar method adds a health bar above the Shootable
    /// </summary>
    /// <param name="_length">Length of the health bar (float)</param>
    protected virtual void SetupHealthBar(float _length = 0f)
    {
        if (_length == 0)
        {
            healthBar = PlayManager.AddHealthBar(transform);
        }
        else
        {
            healthBar = PlayManager.AddHealthBar(transform, _length);
        }
        OnDamage += healthBar.UpdateValue;
    }
    #endregion

    #region Damages
    /// <summary>
    /// RaiseOnDamage methods is used to raise OnDamage event from inhireted classes
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxHP"></param>
    protected virtual void RaiseOnDamage(int hp, int maxHP)
    {
        OnDamage?.Invoke(hp, maxHP);
    }

    /// <summary>
    /// DamageShortRange method gets the damage amount and substract the shootable short range defense before calling Damage if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public virtual int DamageShortRange(int dmg)
    {
        int _temp = Mathf.Max(dmg - shortRangeDef,0);
        if (_temp > 0) Damage(_temp);
        return _temp;
    }

    /// <summary>
    /// DamageMiddleRange method gets the damage amount and substract the shootable middle range defense before calling Damage if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public virtual int DamageMiddleRange(int dmg)
    {
        int _temp = Mathf.Max(dmg - shortRangeDef, 0);
        if (_temp > 0) Damage(_temp);
        return _temp;
    }

    /// <summary>
    /// DamageLongRange method gets the damage amount and substract the shootable long range defense before calling Damage if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public virtual int DamageLongRange(int dmg)
    {
        int _temp = Mathf.Max(dmg - shortRangeDef, 0);
        if (_temp > 0) Damage(_temp);
        return _temp;
    }

    /// <summary>
    /// DamageExplosive method gets the damage amount and substract the shootable explosives defense before calling Damage if still positive
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    public virtual int DamageExplosive(int dmg)
    {
        int _temp = Mathf.Max(dmg - shortRangeDef, 0);
        if (_temp > 0) Damage(_temp);
        return _temp;
    }

    /// <summary>
    /// Damage reduces the shootable HP of Damage amount
    /// </summary>
    /// <param name="dmg">Damage amount</param>
    protected virtual void Damage(int dmg)
    {
        hP -= dmg;
        if (hP <= 0)
        {
            hP = 0;
            OnHPDown?.Invoke();
        }
        OnDamage?.Invoke(hP, maxHP);
    }
    #endregion

    #region Shooting
    /// <summary>
    /// SetTarget method defines the parameter Shootable as current target
    /// </summary>
    /// <param name="_target">Shootable to target</param>
    protected virtual void SetTarget(Shootable _target)
    {
        if (selectedTarget != null) ClearTarget();
        selectedTarget = _target;
        if (selectedTarget != null)
        {
            selectedTarget.OnHPDown += ClearTarget;
        }
    }

    /// <summary>
    /// ClearTarget method clears the current target value
    /// </summary>
    protected virtual void ClearTarget()
    {
        if (selectedTarget != null) selectedTarget.OnHPDown -= ClearTarget;
        selectedTarget = null;
    }

    /// <summary>
    /// IsTargetShootable retruns true if the target is in shootable range
    /// </summary>
    /// <param name="_t">Targeted Shootable</param>
    /// <returns>Shootable or not</returns>
    protected virtual bool IsTargetShootable(Shootable _t)
    {
        //Debug.Log(transform.gameObject.name + " to " + _t.gameObject.name + " distance: " + (transform.position - _t.transform.position).magnitude);
        return (shortRangeAtk > 0 && Ranges.IsShootableShort(transform, _t)) ||
            (middleRangeAtk > 0 && Ranges.IsShootableMiddle(transform, _t)) ||
            (longRangeAtk > 0 && Ranges.IsShootableLong(transform, _t));
    }

    /// <summary>
    /// IsTargetInSight returns true if the target is seen by the SoldierUnit
    /// </summary>
    /// <param name="_t">Targeted Shootable</param>
    /// <returns>InSight or not</returns>
    protected virtual bool IsTargetInSight(Shootable _t)
    {
        Vector3 targetDir = _t.transform.position - transform.position;
        float angle = Vector3.Angle(new Vector3(targetDir.x, 0f, targetDir.z), transform.forward);
        return (Mathf.Abs(angle) <= 30f);
    }

    /// <summary>
    /// Shoot method checks if the shooting delay has expired and shoots if it has
    /// </summary>
    /// <param name="_t"></param>
    protected virtual void Shoot(Shootable _t)
    {
        // Check shooting delay
        if (shootingDelay > 0f) return;

        // Check at each range the target is and calls the dedicated damage method
        // This is used to oppose attacker RangeAttack values to target RangeDefense ones
        if (Ranges.IsInShortRange(transform, _t) && shortRangeAtk>0)
        {
            _t.DamageShortRange(shortRangeAtk);
        }
        else if (Ranges.IsInMiddleRange(transform, _t) && middleRangeAtk>0)
        {
            _t.DamageMiddleRange(middleRangeAtk);
        }
        else if (Ranges.IsInLongRange(transform, _t) && longRangeAtk>0)
        {
            _t.DamageLongRange(longRangeAtk);
        }
        else
        {
            return;
        }

        // Send message for Animator when there is one (soldiers and enemy soldiers)
        SendMessage("ShootMessage", SendMessageOptions.DontRequireReceiver);

        // Launch VFX
        if (fireParticles != null)
        {
            fireParticles.Play();
            Invoke("StopFireParticules", 0.3f);
        }        

        // Launch SFX
        if(fireSFX != null)
        {
            fireSFX.pitch = Random.Range(0.8f, 1.2f);
            fireSFX.Play();
        }        

        // Reset shootingDelay
        shootingDelay = shootingDataDuration;
    }

    /// <summary>
    /// StopFireParticules method is used to stop shooting VFX after a delay (with Invoke)
    /// </summary>
    protected virtual void StopFireParticules()
    {
        fireParticles.Stop();
    }

    #endregion
}
