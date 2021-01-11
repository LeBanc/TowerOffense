using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// SquadSave defines the methods to load and save a Squad
/// </summary>
public class SquadSave
{
    [XmlAttribute("ID")]
    public int iD;

    [XmlAttribute("ColorR")]
    public float colorR;
    [XmlAttribute("ColorG")]
    public float colorG;
    [XmlAttribute("ColorB")]
    public float colorB;

    [XmlAttribute("Type")]
    public string squadTypePath;

    [XmlAttribute("Soldier1ID")]
    public int soldier1;
    [XmlAttribute("Soldier2ID")]
    public int soldier2;
    [XmlAttribute("Soldier3ID")]
    public int soldier3;
    [XmlAttribute("Soldier4ID")]
    public int soldier4;

    [XmlAttribute("Range")]
    public Squad.PreferedRange range;

    [XmlAttribute("PosChoice")]
    public Squad.PositionChoice posChoice;

    [XmlAttribute("Engage")]
    public bool isEngaged;

    /// <summary>
    /// Basic constructor of an empty SquadSave (needed to xml saves)
    /// </summary>
    public SquadSave()
    {
        iD = -1;
        colorR = 1f;
        colorG = 1f;
        colorB = 1f;
        squadTypePath = "";
        soldier1 = -1;
        soldier2 = -1;
        soldier3 = -1;
        soldier4 = -1;
        range = Squad.PreferedRange.MiddleRange;
        posChoice = Squad.PositionChoice.MaximizeAttack;
        isEngaged = false;
    }

    /// <summary>
    /// Construcor of a SquadSave from a Squad
    /// </summary>
    /// <param name="_squad">Squad to save</param>
    public SquadSave(Squad _squad)
    {
        iD = _squad.ID;
        colorR = _squad.Color.r;
        colorG = _squad.Color.g;
        colorB = _squad.Color.b;
        squadTypePath = _squad.SquadType.name;
        soldier1 = _squad.Soldiers[0].ID;
        soldier2 = _squad.Soldiers[1].ID;
        soldier3 = _squad.Soldiers[2].ID;
        soldier4 = _squad.Soldiers[3].ID;
        range = _squad.PrefRange;
        posChoice = _squad.PosChoice;
        isEngaged = _squad.isEngaged;
    }

    /// <summary>
    /// Load create a Squad from a SquadSave
    /// </summary>
    /// <returns>The Squad created from SquadSave data</returns>
    public Squad Load()
    {
        Color _color = new Color(colorR, colorG, colorB);
        Squad _squad = ScriptableObject.CreateInstance("Squad") as Squad;
        _squad.LoadData(iD, _color, squadTypePath, soldier1, soldier2, soldier3, soldier4, range, posChoice, isEngaged);
        return _squad;
    }

}
