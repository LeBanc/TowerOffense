using UnityEngine;

/// <summary>
/// HQCandidate class is a Buildable for any candidate place to build a new HQ
/// </summary>
public class HQCandidate : Buildable
{
    /// <summary>
    /// At Start, init the HQCandidate in the PlayManager list
    /// </summary>
    protected override void Start()
    {
        base.Start();
        PlayManager.hqCandidateList.Add(this);
    }

    /// <summary>
    /// StarBuilding method set the Buildable as "isBuilding", creates the HealthBar and add Build method as PlayUpdate subscribers
    /// HQCansidate buildingTime is 20 seconds and builderCapacity is HQBuild
    /// </summary>
    public override void StartBuilding()
    {
        base.StartBuilding();
        buildingTime = 20f;
        builderCapacity = SoldierData.Capacities.HQBuild;
    }

    /// <summary>
    /// EndBuilding method ends the building state by removing the HealthBar and unsubscribe from PlayUpdate event
    /// HQCandidate moves the HQ to the current position and setups it, then it destroys itself
    /// </summary>
    protected override void EndBuilding()
    {
        HQ.InstantiateHQCandidate(PlayManager.hq.transform.position);
        PlayManager.hq.transform.position = transform.position;
        PlayManager.hq.SetSpawnPoints();
        PlayManager.hqPos = GridAdjustment.GetGridCoordinates(new Vector3(transform.position.x, 0f, transform.position.z));
        PlayManager.SetNewHQPosition();

        base.EndBuilding();

        PlayManager.hqCandidateList.Remove(this);
        Destroy(gameObject, Time.deltaTime);
    }
}
