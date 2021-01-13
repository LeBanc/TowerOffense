using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// HQSave class defines how to save and load the HQ
/// </summary>
public class HQSave
{
    // Position of the HQ on the terrain
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    /// <summary>
    /// Empty constructor
    /// </summary>
    public HQSave()
    {
        position = new float[] { 0f, 0f, 0f };
    }

    /// <summary>
    /// Constructor of HQSave from an HQ component
    /// </summary>
    /// <param name="_hq">HQ component to save</param>
    public HQSave(HQ _hq)
    {
        position = new float[] { _hq.transform.position.x, _hq.transform.position.y, _hq.transform.position.z };
    }

    /// <summary>
    /// Load methods return the HQ position (Vector3) from the HQSave data
    /// </summary>
    /// <returns>Position of the HQ (Vector3)</returns>
    public Vector3 Load()
    {
        return new Vector3(position[0], position[1], position[2]);
    }
}
