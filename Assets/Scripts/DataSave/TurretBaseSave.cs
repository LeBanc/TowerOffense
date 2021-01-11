using UnityEngine;
using System.Xml.Serialization;

public class TurretBaseSave
{
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    [XmlAttribute("Counter")]
    public float counter;

    [XmlAttribute("Building")]
    public bool isBuilding;

    public TurretBaseSave()
    {
        position = new float[] { 0f, 0f, 0f };
        counter = 0f;
        isBuilding = false;
    }

    public TurretBaseSave(TurretBase _turretBase)
    {
        position = new float[] { _turretBase.transform.position.x, _turretBase.transform.position.y, _turretBase.transform.position.z };
        counter = _turretBase.Counter;
        isBuilding = _turretBase.IsBuilding;
    }

    public TurretBase Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        TurretBase _turretBase = _prefab.GetComponent<TurretBase>();
        _turretBase.LoadData(isBuilding,counter);
        return _turretBase;
    }
}
