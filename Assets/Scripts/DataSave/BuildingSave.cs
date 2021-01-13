using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// BuildingSave class defines how to save and load a Building
/// </summary>
public class BuildingSave
{
    // Position on the terrain
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    /// <summary>
    /// Empty Constructor
    /// </summary>
    public BuildingSave()
    {
        position = new float[] { 0f, 0f, 0f };
    }

    /// <summary>
    /// BuildingSave constructor from a Building (GameObject)
    /// </summary>
    /// <param name="_building">GameObject to save</param>
    public BuildingSave(GameObject _building)
    {
        position = new float[] { _building.transform.position.x, _building.transform.position.y, _building.transform.position.z };
    }

    /// <summary>
    /// Load methods move the GameObject at input to the saved position
    /// </summary>
    /// <param name="_prefab">Building (GameObject) to move at saved position</param>
    /// <returns>Input GameObject after data loading</returns>
    public GameObject Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        return _prefab;
    }
}
