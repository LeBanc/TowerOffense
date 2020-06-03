using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameData are the basic game data
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "Tower Offense/Game Data", order = 100)]
public class GameData: ScriptableObject
{
    [Header("Ranges definition")]
    public float shortRange = 10f;
    public float middleRange = 30f;
    public float longRange = 50f;

    [Header("Time of the attack phase")]
    public float baseAttackTime = 90f;

    [Header("Basic amounts")]
    public int baseHealAmount = 30;
    public int[] experienceMaxAmount = { 50, 100, 200, 400 };

    [Header("First squad")]
    public GameObject squadPrefab;
    public SquadData defaultSquadType;

    [Header("Lists of resources")]
    public List<Color> squadColors;
    public List<Sprite> soldierImages;
    public List<string> soldierFirstNames;
    public List<string> soldierLastNames;
}
