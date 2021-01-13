using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// TurretSave class defines how to save and load a turret
/// </summary>
public class TurretSave
{
    // Position of the turret on the terrain
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    // Current HP of the turret
    [XmlAttribute("HP")]
    public int currentHP;

    // Turns left before destroying the turret
    [XmlAttribute("Turns")]
    public int turns;

    /// <summary>
    /// Basic constructor of an empty turret
    /// </summary>
    public TurretSave()
    {
        position = new float[] { 0f, 0f, 0f };
        currentHP = 0;
        turns = 0;
    }

    /// <summary>
    /// Constructor of TurretSave from a Turret component
    /// </summary>
    /// <param name="_turret">Turret to save</param>
    public TurretSave(Turret _turret)
    {
        position = new float[] { _turret.transform.position.x, _turret.transform.position.y, _turret.transform.position.z };
        currentHP = _turret.HP;
        turns = _turret.Turns;
    }

    /// <summary>
    /// Load function load turret data from save data and set the Gameobject and its turret component
    /// </summary>
    /// <param name="_prefab">Prefab GameObject with a Turret component</param>
    /// <returns>Turret of the prefab after data loading</returns>
    public Turret Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        Turret _turret = _prefab.GetComponent<Turret>();
        _turret.LoadData(currentHP, turns);
        return _turret;
    }
}
