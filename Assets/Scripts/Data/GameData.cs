using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Tower Offense/Game Data", order = 100)]
public class GameData: ScriptableObject
{
    public float shortRange = 10f;
    public float middleRange = 30f;
    public float longRange = 50f;
}
