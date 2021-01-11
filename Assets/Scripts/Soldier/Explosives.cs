using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosives : Buildable
{
    private float explosionTime = 5f;
    private float explosionCounter;

    private Tower target;
    private int explosiveDamages = 50;

    /// <summary>
    /// At Start, subscribe to events, Start the building routine
    /// </summary>
    protected override void Start()
    {
        base.Start();
        StartBuilding();
    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    protected override void OnDestroy()
    {
        GameManager.PlayUpdate -= Countdown;
        PlayManager.OnEndDay -= Explode;
        base.OnDestroy();
    }

    /// <summary>
    /// StarBuilding method set the Buildable as "isBuilding", creates the HealthBar and add Build method as PlayUpdate subscribers
    /// Explosives buildingTime is 10 seconds and builderCapacity is Explosives
    /// </summary>
    public override void StartBuilding()
    {
        base.StartBuilding();
        buildingTime = 10f;
        builderCapacity = SoldierData.Capacities.Explosives;
        PlayManager.explosivesList.Add(this);
    }

    /// <summary>
    /// EndBuilding method ends the building state by removing the HealthBar and unsubscribe from PlayUpdate event
    /// Explosives
    /// </summary>
    protected override void EndBuilding()
    {
        base.EndBuilding();

        // Move the squad from the current location
        builder.Squad.MoveAfterReplaced();

        // Launch the explosives countdown
        GameManager.PlayUpdate += Countdown;

        // Make the explosives explode at the end of the day (if they have not exploded before)
        PlayManager.OnEndDay += Explode;

        // Activate the lights blinking
        LightBlink[] _lights = GetComponentsInChildren<LightBlink>();
        foreach(LightBlink _l in _lights)
        {
            _l.Activate();
        }
    }

    /// <summary>
    /// GetAvailablePositions method search for the available positions (terrain, not building, not enemies) around the turret
    /// But for Explosives => return only explosives position
    /// </summary>
    /// <returns>List of the available positions around the turret (List<Vector3>)</returns>
    public override List<Vector3> GetAvailablePositions()
    {
        List<Vector3> _availableCells = new List<Vector3>();
        _availableCells.Add(GridAdjustment.GetGridCoordinates(transform.position));
        return _availableCells;
    }

    /// <summary>
    /// Countdown method start the countdown and make the explosives explode at the end
    /// </summary>
    private void Countdown()
    {
        explosionCounter += Time.deltaTime;
        if(explosionCounter > explosionTime)
        {
            Explode();
        }
    }

    /// <summary>
    /// Explode method activates SFX and VFX, damages the target (Tower) and destroy the explosives
    /// </summary>
    private void Explode()
    {
        // SFX and VFX (TBD)

        // Damage the target (tower)
        target.DamageExplosive(explosiveDamages);

        // Damage the soldier (own and enemies) that are on the same position as the explosives
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(GridAdjustment.GetGridCoordinates(transform.position), PlayManager.data.shortRange/2, LayerMask.GetMask("Enemies", "Soldiers"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                {
                    _soldier.DamageExplosive(explosiveDamages);
                }
                if (c.TryGetComponent<EnemySoldier>(out EnemySoldier _enemy))
                {
                    _enemy.DamageExplosive(explosiveDamages);
                }
            }
        }

        // Remove the explosives
        PlayManager.explosivesList.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// SetTarget defines the targeted Tower
    /// </summary>
    /// <param name="_tower"></param>
    public void SetTarget(Tower _tower)
    {
        target = _tower;
    }
}
