using UnityEngine;
using System.Xml.Serialization;

public class HQSave
{
    [XmlAttribute("Position")]
    public float[] position = new float[3];

    public HQSave()
    {
        position = new float[] { 0f, 0f, 0f };
    }

    public HQSave(HQ _hq)
    {
        position = new float[] { _hq.transform.position.x, _hq.transform.position.y, _hq.transform.position.z };
    }

    public Vector3 Load()
    {
        return new Vector3(position[0], position[1], position[2]);
    }
}
