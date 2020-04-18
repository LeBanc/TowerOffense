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

    // List of soldiers seen by the tower
    private List<GameObject> targets;

    // Selected target of the tower
    private GameObject selectedTarget;

    // For debug purpose
    // /*
    private LineRenderer line;
    private Material _red;
    // */
    
    private void Start()
    {
        // Initialization
        targets = new List<GameObject>();
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
    /// AddTarget adds a target to the target list if it isn't already in the list
    /// </summary>
    /// <param name="target">GameObject to add to the list</param>
    private void AddTarget(GameObject target)
    {
        if (!targets.Contains(target)) targets.Add(target);
    }

    /// <summary>
    /// RemoveTarget removes the target from the list if it contains it
    /// </summary>
    /// <param name="target">GameObject to remove from the list</param>
    private void RemoveTarget(GameObject target)
    {
        if (targets.Contains(target)) targets.Remove(target);
    }

    /// <summary>
    /// GetNearestInSightTarget sets the nearest in range and in sight soldier as the selected target (null if none)
    /// </summary>
    private void GetNearsetInSightTarget()
    {
        // For debug purpose
        line.enabled = false;

        selectedTarget = null; // Clear selection
        if (!(targets.Count > 0)) return; // If there is no in range targets, return
        
        float _targetDist = Mathf.Infinity; // float used to get the nearest target
        foreach (GameObject t in targets)
        {
            // Checks for each target in range if it is a soldier that can be hit by a raycast
            Vector3 rayDirection = (t.transform.position - transform.position);
            rayDirection.Normalize();
            
            //Debug.DrawRay(transform.position, rayDirection * 100f, Color.yellow);

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Soldiers", "Buildings" })))
            {
                if (hit.collider.gameObject == t && hit.collider.TryGetComponent<Soldier>(out Soldier _soldier))
                {
                    // Sets the nearest soldier as the selected target
                    if ((hit.transform.position - transform.position).magnitude < _targetDist)
                    {
                        _targetDist = (t.transform.position - transform.position).magnitude;
                        selectedTarget = t;
                    }
                }
            }
        }
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
        // */
    }

    /// <summary>
    /// GetInRangeTargets search for any in range targets (in range: between smaller and larger detection ranges)
    /// </summary>
    private void GetInRangeTargets()
    {
        targets.Clear();
        Collider[] _soldiers;
        _soldiers = Physics.OverlapSphere(transform.position, largerDetectionRange, LayerMask.GetMask("Soldiers"));
        if (_soldiers.Length > 0)
        {
            foreach (Collider c in _soldiers)
            {
                AddTarget(c.gameObject);
            }
        }

        if (smallerDetectionRange > 0f && smallerDetectionRange < largerDetectionRange)
        {
            _soldiers = Physics.OverlapSphere(transform.position, smallerDetectionRange, LayerMask.GetMask("Soldiers"));
            if (_soldiers.Length > 0)
            {
                foreach (Collider c in _soldiers)
                {
                    RemoveTarget(c.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// TowerUpdate is the Update methods of the Tower, to call in Update in test scene or to add to GameManager event in game
    /// </summary>
    private void TowerUpdate()
    {
        GetInRangeTargets();
        GetNearsetInSightTarget();
        if (!active && selectedTarget != null) Activate();
    }

}
