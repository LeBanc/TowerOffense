using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadData", menuName = "Tower Offense/Squad Data", order = 102)]
public class SquadData : ScriptableObject
{
    public string squadTypeName;
    public Vector3 soldier1Position;
    public Vector3 soldier2Position;
    public Vector3 soldier3Position;
    public Vector3 soldier4Position;
}
