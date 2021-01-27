using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameData are the basic game data
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "Tower Offense/Game Data", order = 100)]
public class GameData: ScriptableObject
{
    // Ranges distance
    [Header("Ranges definition")]
    public float shortRange = 10f;
    public float middleRange = 30f;
    public float longRange = 50f;

    // Base time attack
    [Header("Time of the attack phase")]
    public float baseAttackTime = 45f;

    [Header("Basic amounts")]
    // Heal basic amount
    public int baseHealAmount = 30;
    // Recruitment basic amount
    public int baseRecruitAmount = 10;

    [Header("Facilities bonus")]
    // Time bonus
    public int timeBonus = 15;
    // Heal bonus
    public int healBonus = 5;
    // Recruiting Bonus
    public int recruitingBonus = 5;
    // Explosive damages bonus
    public int exploDamagesBonus = 50;

    // Soldier ranks titles
    [Header("Soldier ranks")]
    public string[] ranks = new string[4];
    public string playerRank;

    // First squad data to create the first squad in game
    [Header("First squad")]
    public GameObject squadPrefab;
    public SquadData defaultSquadType;

    // Multiple prefabs instatiated in game
    [Header("Prefabs")]
    public GameObject buildingPrefab;
    public GameObject turretBasePrefab;
    public GameObject explosivesPrefab;
    public GameObject towerPrefab;
    public GameObject hqCandidatePrefab;
    public GameObject turretPrefab;

    // In game resources
    [Header("Lists of resources")]
    public List<Color> squadColors;
    public List<Sprite> soldierImages;
    public List<string> soldierFirstNames;
    public List<string> soldierLastNames;
}
