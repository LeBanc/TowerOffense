using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// TowerSave class defines how to save and load a tower
/// </summary>
public class TowerSave
{
    // Position on the terrain
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    // Current HP of the tower
    [XmlAttribute("HP")]
    public int currentHP;

    // Path of the tower data
    [XmlAttribute("data")]
    public string dataPath;

    // Active state of the tower
    [XmlAttribute("Active")]
    public bool active;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public TowerSave()
    {
        position = new float[] {0f,0f,0f};
        currentHP = 0;
        dataPath = "";
        active = false;
    }

    /// <summary>
    /// Constructor of TowerSave from a Tower component
    /// </summary>
    /// <param name="_tower">Tower component to save</param>
    public TowerSave(Tower _tower)
    {
        position = new float[] { _tower.transform.position.x, _tower.transform.position.y, _tower.transform.position.z };
        currentHP = _tower.HP;
        dataPath = _tower.data.name;
        active = _tower.IsActive();
    }

    /// <summary>
    /// Load method inits the Tower component of a GameObject with the TowerSave data
    /// </summary>
    /// <param name="_prefab">GameObject with Tower component</param>
    /// <returns>Tower componenet of the input GameObject after data loading</returns>
    public Tower Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0],position[1],position[2]);
        Tower _tower = _prefab.GetComponent<Tower>();
        _tower.LoadData(dataPath, currentHP, active);
        return _tower;
    }
}
