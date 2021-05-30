using UnityEngine;

/// <summary>
/// HQCandidate class is a Buildable for any candidate place to build a new HQ
/// </summary>
public class HQCandidate : Buildable
{
    public Renderer buldingRenderer;

    /// <summary>
    /// At Start, init the HQCandidate in the PlayManager list and subscribe to events
    /// </summary>
    protected override void Start()
    {
        buildingTime = PlayManager.data.hqBuildTime;
        base.Start();
        if(!PlayManager.hqCandidateList.Contains(this)) PlayManager.hqCandidateList.Add(this);

        SquadActionPanel.OnShowHQCHighlight += ShowHighlight;
        SquadActionPanel.OnHideHQCHighlight += HideHighlight;

    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        SquadActionPanel.OnShowHQCHighlight -= ShowHighlight;
        SquadActionPanel.OnHideHQCHighlight -= HideHighlight;

        base.OnDestroy();
    }

    /// <summary>
    /// StarBuilding method set the Buildable as "isBuilding", creates the HealthBar and add Build method as PlayUpdate subscribers
    /// HQCansidate buildingTime is 20 seconds and builderCapacity is HQBuild
    /// </summary>
    public override void StartBuilding()
    {
        base.StartBuilding();
        buildingTime = PlayManager.data.hqBuildTime;
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

        PlayManager.soldierNav.UpdateNavMesh(PlayManager.soldierNav.navMeshData);
        PlayManager.squadNav.UpdateNavMesh(PlayManager.squadNav.navMeshData);

        base.EndBuilding();

        PlayManager.hqCandidateList.Remove(this);
        Destroy(gameObject, Time.deltaTime);
    }

    /// <summary>
    /// ShowHighlight method enables the highlight (emission) on the HQCandidate material
    /// </summary>
    private void ShowHighlight()
    {
        if (buldingRenderer.material.GetFloat("_EnableHighlight") < 1f)
        {
            Material _mat = buldingRenderer.material;
            _mat.SetFloat("_EnableHighlight", 1f);
            buldingRenderer.material = _mat;
        }
    }

    /// <summary>
    /// HideHighlight method disables the highlight (emission) on the HQCandidate material
    /// </summary>
    private void HideHighlight()
    {
        if (buldingRenderer.material.GetFloat("_EnableHighlight") > 0f)
        {
            Material _mat = buldingRenderer.material;
            _mat.SetFloat("_EnableHighlight", 0f);
            buldingRenderer.material = _mat;
        }
    }
}
