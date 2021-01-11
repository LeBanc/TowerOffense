using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// SoldierSave defines the methods to load and save a Soldier
/// </summary>
public class SoldierSave
{
    [XmlAttribute("ID")]
    public int iD;
    [XmlAttribute("name")]
    public string soldierName;
    [XmlAttribute("HP")]
    public int currentHP;
    [XmlAttribute("XP")]
    public int currentXP;
    [XmlAttribute("image")]
    public string imagePath;
    [XmlAttribute("data")]
    public string dataPath;

    /// <summary>
    /// Basic constructor of an empty SquadSave (needed to xml saves)
    /// </summary>
    public SoldierSave()
    {
        iD = 0;
        soldierName = "";
        imagePath = "";
        dataPath = "";
        currentHP = 0;
        currentXP = 0;
    }

    /// <summary>
    /// Constructor od SoldierSave from a Soldier
    /// </summary>
    /// <param name="_soldier">Soldier to save</param>
    public SoldierSave(Soldier _soldier)
    {
        iD = _soldier.ID;
        soldierName = _soldier.Name;
        imagePath = _soldier.Image.name;
        dataPath = _soldier.Data.name;
        currentHP = _soldier.CurrentHP;
        currentXP = _soldier.CurrentXP;
    }

    /// <summary>
    /// Load create a Soldier from a SquadSave
    /// </summary>
    /// <returns>The Soldier created from SoldierSave data</returns>
    public Soldier Load()
    {
        Soldier _soldier = ScriptableObject.CreateInstance("Soldier") as Soldier;
        _soldier.LoadData(iD, soldierName, imagePath, dataPath, currentHP, currentXP);
        return _soldier;
    }
}
