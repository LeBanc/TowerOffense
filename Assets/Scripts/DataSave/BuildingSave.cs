using UnityEngine;
using System.Xml.Serialization;

public class BuildingSave
{
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    public BuildingSave()
    {
        position = new float[] { 0f, 0f, 0f };
    }

    public BuildingSave(GameObject _building)
    {
        position = new float[] { _building.transform.position.x, _building.transform.position.y, _building.transform.position.z };
    }

    public GameObject Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        return _prefab;
    }
}
