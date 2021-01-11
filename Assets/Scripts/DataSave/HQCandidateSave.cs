using UnityEngine;
using System.Xml.Serialization;

public class HQCandidateSave
{
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    [XmlAttribute("Counter")]
    public float counter;

    [XmlAttribute("Building")]
    public bool isBuilding;

    public HQCandidateSave()
    {
        position = new float[] { 0f, 0f, 0f };
        counter = 0f;
        isBuilding = false;
    }

    public HQCandidateSave(HQCandidate _candidate)
    {
        position = new float[] { _candidate.transform.position.x, _candidate.transform.position.y, _candidate.transform.position.z };
        counter = _candidate.Counter;
        isBuilding = _candidate.IsBuilding;
    }

    public HQCandidate Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        HQCandidate _candidate = _prefab.GetComponent<HQCandidate>();
        _candidate.LoadData(isBuilding, counter);
        return _candidate;
    }
}
