using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SoldierData", menuName = "Tower Offense/Soldier Data", order = 101)]
public class SoldierData : ScriptableObject
{
    public enum Capacities
    {
        IncreaseSpeed,
        Heal,
        HQBuild,
        TurretBuild,
        Explosives,
        WoundedSaving
    }

    public GameObject prefab;

    public int shortRangeAttack;
    public int middleRangeAttack;
    public int longRangeAttack;

    public int shortRangeDefense;
    public int middleRangeDefense;
    public int longRangeDefense;
    public int explosiveDefense;

    public int speed;
    public int maxHP;
    public float shootingDelay;
    public AudioClip shootingSound;

    public List<Capacities> capacities;

    public List<SoldierData> improveTo;

}
