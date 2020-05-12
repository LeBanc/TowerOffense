using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameData", menuName = "Tower Offense/Game Data", order = 100)]
public class GameData: ScriptableObject
{
    public float shortRange = 10f;
    public float middleRange = 30f;
    public float longRange = 50f;

    public float baseAttackTime = 90f;
    public int baseHealAmount = 30;

    public GameObject squadPrefab;
    public SquadData defaultSquadType;

    public List<Color> squadColors;
    public List<Sprite> soldierImages;
    public List<string> soldierFirstNames;
    public List<string> soldierLastNames;
}
