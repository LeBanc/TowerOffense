using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    protected bool isBuilding = false;
    protected float buildingTime;
    protected float buildingCounter;
    protected SoldierUnit builder;
    protected HealthBar healthBar;
    protected SoldierData.Capacities builderCapacity;

    public bool IsBuilding
    {
        get { return isBuilding; }
    }

    public float Counter
    {
        get { return buildingCounter; }
    }

    /// <summary>
    /// At Start, subscribe to events, Start the building routine
    /// </summary>
    protected virtual void Start()
    {
        PlayManager.OnLoadSquadsOnNewDay += Init;
        PlayManager.OnEndDay += Hide;
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected virtual void OnDestroy()
    {
        PlayManager.OnLoadSquadsOnNewDay -= Init;
        PlayManager.OnEndDay -= Hide;
        GameManager.PlayUpdate -= Build;
    }

    /// <summary>
    /// Init method initialize the HealthBar
    /// </summary>
    protected virtual void Init()
    {
        // If the TurretBase is still building, shows the HealthBar
        if (isBuilding)
        {
            healthBar.Show();
            GameManager.PlayUpdate += Build;
        }
        // If not, hides it and destroys it
        if (!isBuilding && healthBar != null)
        {
            healthBar.Remove();
            healthBar = null;
        }
    }

    /// <summary>
    /// Hide method is used to hide the TurretBase HealthBar
    /// </summary>
    protected virtual void Hide()
    {
        if (isBuilding)
        {
            healthBar.Hide();
            GameManager.PlayUpdate -= Build;
        }
    }

    /// <summary>
    /// StarBuilding method set the Buildable as "isBuilding", creates the HealthBar and add Build method as PlayUpdate subscribers
    /// </summary>
    public virtual void StartBuilding()
    {
        isBuilding = true;
        healthBar = PlayManager.AddHealthBar(transform);
        healthBar.green = new Color(0f, 0.93f, 1f, 1f);
        healthBar.orange = new Color(0f, 0.93f, 1f, 1f);
        healthBar.red = new Color(0f, 0.93f, 1f, 1f);
        int buildingAmount = (int)((buildingCounter / buildingTime) * 100);
        healthBar.UpdateValue(Mathf.Min(buildingAmount, 100), 100);
        GameManager.PlayUpdate += Build;
    }

    /// <summary>
    /// Build method search for a builder in ShortRange and call AddToBuilding if found. The HealthBar position is updated
    /// </summary>
    protected virtual void Build()
    {
        builder = GetBuilderInShortRange();
        // Search for a builder in ShortRange and if found, add building amount
        if (builder != null && isBuilding)
        {
            AddToBuilding();
            builder.IsBuilding = true;
        }
        if (healthBar != null) healthBar.UpdatePosition();
    }

    /// <summary>
    /// AddToBuilding method increments the building time and call EndBuilding if it has expired
    /// </summary>
    protected virtual void AddToBuilding()
    {
        buildingCounter += Time.deltaTime;
        int buildingAmount = (int)((buildingCounter / buildingTime) * 100);
        healthBar.UpdateValue(Mathf.Min(buildingAmount, 100), 100);
        if (buildingCounter >= buildingTime)
        {
            EndBuilding();
        }
    }

    /// <summary>
    /// EndBuilding method ends the building state by removing the HealthBar and unsubscribe from PlayUpdate event
    /// </summary>
    protected virtual void EndBuilding()
    {
        healthBar.Remove();
        healthBar = null;
        isBuilding = false;
        GameManager.PlayUpdate -= Build;
    }

    /// <summary>
    /// IsBuilderInShortRange searches for a SoldierUnit with builderCapacity capacity in ShortRange
    /// </summary>
    /// <returns>true if at least a Soldier is found, false otherwise</returns>
    protected virtual SoldierUnit GetBuilderInShortRange()
    {
        List<SoldierUnit> _targets = new List<SoldierUnit>();
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(GridAdjustment.GetGridCoordinates(transform.position), PlayManager.ShortRange * 1.2f, LayerMask.GetMask("Soldiers"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                {
                    if (_soldier.Soldier.Data.capacities.Contains(builderCapacity) && !_soldier.IsWounded) _targets.Add(_soldier);
                }
            }
        }

        if(_targets.Count > 0)
        {
            return Ranges.GetNearestInList<SoldierUnit>(transform, _targets);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// GetAvailablePositions method search for the available positions (terrain, not building, not enemies) around the turret
    /// </summary>
    /// <returns>List of the available positions around the turret (List<Vector3>)</returns>
    public virtual List<Vector3> GetAvailablePositions()
    {
        List<Vector3> _availableCells = new List<Vector3>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                Vector3 _cell = new Vector3((float)10 * i, 0f, (float)10 * j);

                RaycastHit _hit;
                Ray _ray = new Ray(GridAdjustment.GetGridCoordinates(transform.position) + _cell + 100 * Vector3.up, -Vector3.up);
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain", "Buildings", "Enemies" })))
                {
                    if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {
                        _hit = new RaycastHit();
                        _ray = new Ray(transform.position, (_cell - 7.5f * Vector3.up).normalized);
                        if (!Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Enemies" })))
                        {
                            if (_cell.magnitude <= PlayManager.ShortRange) _availableCells.Add(GridAdjustment.GetGridCoordinates(transform.position + _cell));
                        }
                    }
                }
            }
        }

        foreach (SquadUnit _su in PlayManager.squadUnitList)
        {
            _availableCells.Remove(GridAdjustment.GetGridCoordinates(_su.Destination));
        }

        return _availableCells;
    }

    public virtual void LoadData(bool _isBuilding, float _counter)
    {
        isBuilding = _isBuilding;
        buildingCounter = _counter;
    }
}
