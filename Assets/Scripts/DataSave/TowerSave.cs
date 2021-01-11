using UnityEngine;
using System.Xml.Serialization;

public class TowerSave
{
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    [XmlAttribute("HP")]
    public int currentHP;

    [XmlAttribute("data")]
    public string dataPath;

    [XmlAttribute("Active")]
    public bool active;

    public TowerSave()
    {
        position = new float[] {0f,0f,0f};
        currentHP = 0;
        dataPath = "";
        active = false;
    }

    public TowerSave(Tower _tower)
    {
        position = new float[] { _tower.transform.position.x, _tower.transform.position.y, _tower.transform.position.z };
        currentHP = _tower.HP;
        dataPath = _tower.data.name;
        active = _tower.IsActive();
    }

    public Tower Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0],position[1],position[2]);
        Tower _tower = _prefab.GetComponent<Tower>();
        _tower.LoadData(dataPath, currentHP, active);
        return _tower;
    }
}
