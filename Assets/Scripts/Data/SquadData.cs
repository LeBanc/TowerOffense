using UnityEngine;

/// <summary>
/// SquadData are all data needed to display and compute a Squad(Type)
/// </summary>
[CreateAssetMenu(fileName = "SquadData", menuName = "Tower Offense/Squad Data", order = 102)]
public class SquadData : ScriptableObject
{
    [Header("Name")]
    // SquadType name
    public string squadTypeName;

    [Header("Sprite")]
    // SquadType sprite to display it in UI Canvas
    public Sprite squadTypeSprite;
    // Positions of soldiers' images over the squadType sprite in UI Canvas
    public Vector3 soldier1SpritePosition;
    public Vector3 soldier2SpritePosition;
    public Vector3 soldier3SpritePosition;
    public Vector3 soldier4SpritePosition;

    [Header("Soldier Units")]
    // Positions of soldiers (SoldierUnit) from the SquadUnit center in 3D city view
    public Vector3 soldier1Position;
    public Vector3 soldier2Position;
    public Vector3 soldier3Position;
    public Vector3 soldier4Position;
}
