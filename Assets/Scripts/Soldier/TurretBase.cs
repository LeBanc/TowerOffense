using UnityEngine;

/// <summary>
/// TurretBase class is a Buildable for Turret bases
/// </summary>
public class TurretBase : Buildable
{
    // Prefab of a turret base
    public GameObject turretPrefab;

    /// <summary>
    /// At Start, subscribe to events, Start the building routine and add the TurretBase to the dedicated PlayManager list
    /// TurretBase is added to a PlayManager list and StartBuilding immediately
    /// </summary>
    protected override void Start()
    {
        buildingTime = PlayManager.data.turretBuildTime;
        base.Start();
        PlayManager.turretBaseList.Add(this);
        StartBuilding();
    }

    /// <summary>
    /// StarBuilding method set the Buildable as "isBuilding", creates the HealthBar and add Build method as PlayUpdate subscribers
    /// TurretBase buildingTime is 10 seconds and builderCapacity is TurretBuild
    /// </summary>
    public override void StartBuilding()
    {
        buildingTime = PlayManager.data.turretBuildTime;
        base.StartBuilding();
        builderCapacity = SoldierData.Capacities.TurretBuild;
    }

    /// <summary>
    /// EndBuilding method ends the building state by removing the HealthBar and unsubscribe from PlayUpdate event
    /// TurretBase creates a new Turret at its position and is removed from PlayManager list before being destroyed
    /// </summary>
    protected override void EndBuilding()
    {
        // Instanciate a new turret at position
        Instantiate(turretPrefab, transform.position, Quaternion.identity, GameObject.Find("Turrets").transform);

        base.EndBuilding();

        PlayManager.turretBaseList.Remove(this);
        Destroy(gameObject, Time.deltaTime);
    }
}
