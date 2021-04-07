using UnityEngine;

/// <summary>
/// Facilities are the facilities data (upgrade & costs)
/// </summary>
[CreateAssetMenu(fileName = "FacilitiesData", menuName = "Tower Offense/Facilities Data", order = 101)]
public class FacilitiesData : ScriptableObject
{
    [Header("New squads")]
    // 2nd squad cost
    public int squad2Cost = 15;
    // 3rd squad cost
    public int squad3Cost = 50;
    // 4th squad cost
    public int squad4Cost = 150;

    [Header("Attack time bonus")]
    // Time bonus
    public int timeBonus = 15;
    // Attack time enhancement costs
    public int attackTime1Cost = 50;
    public int attackTime2Cost = 150;
    public int attackTime3Cost = 300;

    [Header("Healing bonus")]
    // Healing bonus amount
    public int heal1Bonus = 5;
    public int heal2Bonus = 5;
    public int heal3Bonus = 10;
    // Attack time enhancement costs
    public int healing1Cost = 40;
    public int healing2Cost = 100;
    public int healing3Cost = 400;

    [Header("Recruiting chance bonus")]
    // Recruiting bonus amount
    public int recruiting1Bonus = 5;
    public int recruiting2Bonus = 5;
    public int recruiting3Bonus = 10;
    // Attack time enhancement costs
    public int recruiting1Cost = 25;
    public int recruiting2Cost = 100;
    public int recruiting3Cost = 300;

    [Header("Explosives damage bonus")]
    // Explosives damage bonus amount
    public int exploDamages1Bonus = 50;
    public int exploDamages2Bonus = 50;
    public int exploDamages3Bonus = 100;
    // Attack time enhancement costs
    public int explosive1Cost = 150;
    public int explosive2Cost = 400;
    public int explosive3Cost = 700;

    [Header("Soldier experience")]
    // Attack time enhancement costs
    public int soldierXPCost = 250;
}
