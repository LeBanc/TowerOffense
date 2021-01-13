using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SoldierData are all data needed to display and compute a Soldier(Type)
/// </summary>
[CreateAssetMenu(fileName = "SoldierData", menuName = "Tower Offense/Soldier Data", order = 101)]
public class SoldierData : ScriptableObject
{
    /// <summary>
    /// Capacities is an enum to list the possible capacities
    /// </summary>
    public enum Capacities
    {
        IncreaseSpeed,
        Heal,
        HQBuild,
        TurretBuild,
        Explosives,
        WoundedSaving
    }

    /// <summary>
    /// SoldierType is an enum to list the base class of the soldier
    /// </summary>
    public enum SoldierType
    {
        Basic,
        Attack,
        Defense,
        Special
    }

    // A soldier first have a soldier Type and a Level
    [Header("Type and Level")]
    public SoldierType soldierType;
    public int soldierLevel;
    public string typeName;

    // The Prefab that will be instantiate in 3D city view
    [Header("Soldier 3D prefab")]
    public GameObject prefab;

    [Header("Attack & Defense")]
    // Attacks values of this soldier type (by ranges)
    public int shortRangeAttack;
    public int middleRangeAttack;
    public int longRangeAttack;

    // Defense values of this soldier type (by ranges + explosives)
    public int shortRangeDefense;
    public int middleRangeDefense;
    public int longRangeDefense;
    public int explosiveDefense;

    [Header("Other Data")]
    // Other soldier data
    public int speed;
    public int maxHP;
    public float shootingDelay;
    public int maxXP;

    [Header("Sounds")]
    // Sound when shooting
    public AudioClip shootingSound;

    [Header("Capacities")]
    // List of capacities
    public List<Capacities> capacities;

    [Header("Improve to")]
    // List of SoldierData the soldier can evolved into
    public List<SoldierData> improveTo;
}
