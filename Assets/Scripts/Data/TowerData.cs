using UnityEngine;

/// <summary>
/// TowerData are all data needed to display and compute a Tower(Type)
/// </summary>
[CreateAssetMenu(fileName = "TowerData", menuName = "Tower Offense/Tower Data", order = 101)]
public class TowerData : ScriptableObject
{
    // A tower first have a tower Type and a Level
    [Header("Level and Type")]
    public int towerLevel;
    public string towerType;
    public bool spawnSoldier;

    [Header("Attack & Defense")]
    // Attacks values of this tower type (by ranges)
    public int shortRangeAttack;
    public int middleRangeAttack;
    public int longRangeAttack;
    public int explosiveAttack;

    // Defense values of this tower type (by ranges + explosives)
    public int shortRangeDefense;
    public int middleRangeDefense;
    public int longRangeDefense;
    public int explosiveDefense;

    [Header("Other Data")]
    // Other soldier data
    public int maxHP;
    public float shootingDelay;
    public float spawnDelay;

    [Header("Sounds")]
    // Sound when shooting
    public AudioClip shootingSound;
}
