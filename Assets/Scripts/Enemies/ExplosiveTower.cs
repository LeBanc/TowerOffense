using System.Linq;
using UnityEngine;

/// <summary>
/// ExplosiveTower class is the class of an Explosive Tower
/// </summary>
public class ExplosiveTower : Tower
{
    // public Gameobject for the grenade
    public GameObject grenadePrefab;

    // private Grenade component of the grenade GameObject
    private Grenade grenade;

    /// <summary>
    /// At Start, fetches the Grenade component in addition of the Tower Start method
    /// </summary>
    protected override void Start()
    {
        grenade = grenadePrefab.GetComponent<Grenade>();
        base.Start();
    }

    /// <summary>
    /// Setup method initializes the tower: base init and specific init for Explosive damages
    /// </summary>
    public override void Setup()
    {
        base.Setup();
        if(grenade == null) grenade = grenadePrefab.GetComponent<Grenade>();
        grenade.SetDamages(explosiveAtk);
    }

    /// <summary>
    /// EnableUpdate method calls the Tower base EnableUpdate, disables the Soldier spawn for ExplosiveTower and reset the grenade position
    /// </summary>
    protected override void EnableUpdate()
    {
        base.EnableUpdate();
        GameManager.PlayUpdate -= SpawnUpdate; // No spawn for explosive tower
        grenade.ResetGrenade();
    }

    /// <summary>
    /// Shoot method is changed from Tower Shoot method to LaunchGrenade instead of doing damage
    /// </summary>
    /// <param name="_t"></param>
    protected override void Shoot(Shootable _t)
    {
        // Check shooting delay
        if (shootingDelay > 0f) return;

        // Check at each range the target is and calls the dedicated damage method
        // This is used to oppose attacker RangeAttack values to target RangeDefense ones
        if (Ranges.IsInShortRange(transform, _t) && shortRangeAtk > 0)
        {
            LaunchGrenade(_t);
        }
        else if (Ranges.IsInMiddleRange(transform, _t) && middleRangeAtk > 0)
        {
            LaunchGrenade(_t);
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
        if (fireSFX != null)
        {
            fireSFX.pitch = Random.Range(0.8f, 1.2f);
            fireSFX.Play();
        }

        // Reset shootingDelay
        shootingDelay = shootingDataDuration;
    }

    /// <summary>
    /// LaunchGrenade method gets the closer hitPoint from the target and calls the Launch method of the tower Grenade
    /// </summary>
    /// <param name="_t"></param>
    private void LaunchGrenade(Shootable _t)
    {
        //Get nearest hit point
        Vector3 _launchPosition = hitList.Aggregate((x, y) => (_t.transform.position - x.position).sqrMagnitude < (_t.transform.position - y.position).sqrMagnitude ? x : y).position;

        // Launch grenade from launch position to destination position
        grenade.Launch(_launchPosition,GridAdjustment.GetGridCoordinates(_t.transform.position));
    }
}
