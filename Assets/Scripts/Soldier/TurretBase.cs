using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : Buildable
{
    public GameObject turretPrefab;

    /// <summary>
    /// At Start, subscribe to events, Start the building routine and add the TurretBase to the dedicated PlayManager list
    /// TurretBase is added to a PlayManager list and StartBuilding immediately
    /// </summary>
    protected override void Start()
    {
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
        base.StartBuilding();
        buildingTime = 10f;
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
