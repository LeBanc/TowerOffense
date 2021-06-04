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
        
    [Header("Basic amounts")]
    // Base time attack
    public float baseAttackTime = 45f;
    // Heal basic amount
    public int baseHealAmount = 30;
    // Recruitment basic amount
    public int baseRecruitAmount = 10;
    // Explosives basic damages
    public int baseExplosivesDamage = 50;
    // Explosives time to explode
    public float baseExplosivesTime = 5f;
    // Tower heal amount
    public int towerHeal = 100;

    [Header("Build time")]
    public float hqBuildTime = 20f;
    public float explosivesBuildTime = 10f;
    public float turretBuildTime = 10f;

    [Header("Facilities bonus")]
    public FacilitiesData facilities;

    // Soldier ranks titles
    [Header("Soldier ranks")]
    public string[] ranks = new string[5];
    public string playerRank;
    public Sprite[] rankImages = new Sprite[5];

    // Freindship levels
    [Header("Fiendship")]
    public FriendshipLevel[] friendshipLevels = new FriendshipLevel[5];

    // First squad data to create the first squad in game
    [Header("First squad")]
    public GameObject squadPrefab;
    public SquadData defaultSquadType;

    // SoldierData for turret
    [Header("Turret data")]
    public SoldierData turretData;

    // Multiple prefabs instatiated in game
    [Header("Prefabs")]
    public GameObject buildingPrefab;
    public GameObject turretBasePrefab;
    public GameObject explosivesPrefab;
    public GameObject towerPrefab;
    public GameObject towerExploPrefab;
    public GameObject hqCandidatePrefab;
    public GameObject turretPrefab;

    // In game resources
    [Header("Lists of resources")]
    public List<Color> squadColors;
    public List<Sprite> soldierImages;
    public SoldierNamesData soldierNames;
}

[System.Serializable]
public class FriendshipLevel
{
    public int threshold;
    public string levelName;
}
