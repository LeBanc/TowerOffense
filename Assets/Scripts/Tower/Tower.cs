using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // Detection ranges, tower detects soldiers taht are inbetween larger and smaller detection range
    public float largerDetectionRange = 1f;
    public float smallerDetectionRange = 0f;

    // Boolean to set the tower as active or not (active towers are shown as towers, not buildings)
    private bool active = false;

    // Selected target of the tower
    private Transform selectedTarget;

    // For debug purpose
    // /*
    private LineRenderer line;
    private Material _red;
    // */
    
    private void Start()
    {
        // Initialization
        GameManager.PlayUpdate += TowerUpdate;

        // For debug purpose
        // /*
        _red = Resources.Load("Materials/UnlitRed_mat", typeof(Material)) as Material;
        line = gameObject.AddComponent<LineRenderer>();
        line.enabled = false;
        // */
    }

    private void OnDestroy()
    {
        GameManager.PlayUpdate -= TowerUpdate;
    }

    /// <summary>
    /// Activate method activates the tower and do anything linked to the activation (GFX, SFX, etc.)
    /// </summary>
    private void Activate()
    {
        active = true;
        // GFX
        GetComponent<MeshRenderer>().material.color = Color.red;
        // SFX
    }

    /// <summary>
    /// IsActive method returns true if tower is active and false otherwise
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return active;
    }

    /// <summary>
    /// TowerUpdate is the Update methods of the Tower, to call in Update in test scene or to add to GameManager event in game
    /// </summary>
    private void TowerUpdate()
    {
        selectedTarget = Ranges.GetNearestSoldier(this.transform, 1, 1, 1);
        if (!active && selectedTarget != null) Activate();

        // For debug purpose
        // /*
        // Checks if a target is selected and draws a line to it
        if (selectedTarget != null)
        {
            Vector3[] _positions = new Vector3[] { transform.position, selectedTarget.transform.position };
            line.material = _red;
            line.SetPositions(_positions);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
        // */
    }

}
