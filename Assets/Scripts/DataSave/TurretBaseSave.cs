using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// TurretBaseSave class defines how to save and load a turretBase
/// </summary>
public class TurretBaseSave
{
    // Position on the terrain
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    // Building time spent
    [XmlAttribute("Counter")]
    public float counter;

    // Building state
    [XmlAttribute("Building")]
    public bool isBuilding;

    /// <summary>
    /// Base constructor of empty TurretBaseSave
    /// </summary>
    public TurretBaseSave()
    {
        position = new float[] { 0f, 0f, 0f };
        counter = 0f;
        isBuilding = false;
    }

    /// <summary>
    /// Constructor of TurretBaseSave from a TurretBase component
    /// </summary>
    /// <param name="_turretBase">TurretBaseComponent to save</param>
    public TurretBaseSave(TurretBase _turretBase)
    {
        position = new float[] { _turretBase.transform.position.x, _turretBase.transform.position.y, _turretBase.transform.position.z };
        counter = _turretBase.Counter;
        isBuilding = _turretBase.IsBuilding;
    }

    /// <summary>
    /// Load method inits the TurretBase component of a GameObject with the TurretBaseSave data
    /// </summary>
    /// <param name="_prefab">GameObject with TurretBase component</param>
    /// <returns>TuretBase componenet of the input GameObject after data loading</returns>
    public TurretBase Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        TurretBase _turretBase = _prefab.GetComponent<TurretBase>();
        _turretBase.LoadData(isBuilding,counter);
        return _turretBase;
    }
}
