using UnityEngine;
using System.Xml.Serialization;

public class TurretSave
{
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    [XmlAttribute("HP")]
    public int currentHP;

    [XmlAttribute("Turns")]
    public int turns;

    public TurretSave()
    {
        position = new float[] { 0f, 0f, 0f };
        currentHP = 0;
        turns = 0;
    }

    public TurretSave(Turret _turret)
    {
        position = new float[] { _turret.transform.position.x, _turret.transform.position.y, _turret.transform.position.z };
        currentHP = _turret.HP;
        turns = _turret.Turns;
    }

    public Turret Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        Turret _turret = _prefab.GetComponent<Turret>();
        _turret.LoadData(currentHP, turns);
        return _turret;
    }
}
