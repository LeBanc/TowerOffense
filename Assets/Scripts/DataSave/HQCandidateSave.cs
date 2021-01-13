using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// HQCandidateSave class defines how to save and load a HQCandidate
/// </summary>
public class HQCandidateSave
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
    /// Empty constructor
    /// </summary>
    public HQCandidateSave()
    {
        position = new float[] { 0f, 0f, 0f };
        counter = 0f;
        isBuilding = false;
    }

    /// <summary>
    /// HQCandidateSave constructor from an HQCandidate component
    /// </summary>
    /// <param name="_candidate">HQCandidate component to save</param>
    public HQCandidateSave(HQCandidate _candidate)
    {
        position = new float[] { _candidate.transform.position.x, _candidate.transform.position.y, _candidate.transform.position.z };
        counter = _candidate.Counter;
        isBuilding = _candidate.IsBuilding;
    }

    /// <summary>
    /// Load method inits the HQCandidate component of a GameObject with the HQCandidateSave data
    /// </summary>
    /// <param name="_prefab">GameObject with HQCandidate component</param>
    /// <returns>HQCandidate componenet of the input GameObject after data loading</returns>
    public HQCandidate Load(GameObject _prefab)
    {
        _prefab.transform.position = new Vector3(position[0], position[1], position[2]);
        HQCandidate _candidate = _prefab.GetComponent<HQCandidate>();
        _candidate.LoadData(isBuilding, counter);
        return _candidate;
    }
}
