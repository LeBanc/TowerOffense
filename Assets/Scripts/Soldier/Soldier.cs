using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{

    private NavMeshAgent navAgent;
    private Squad squad;
    private Transform target;
    private Transform secondaryTarget;
    private SoldierData data;
    private int hP;

    /// <summary>
    /// Soldier setup sets the parent squad, the soldier's navMeshAgent and the events used
    /// </summary>
    /// <param name="_squad"></param>
    public void Setup(Squad _squad, SoldierData _data, int _HP)
    {
        squad = _squad;
        data = _data;
        hP = _HP;
        navAgent = GetComponent<NavMeshAgent>();
        // Events used are PlayUpdate and Squad target change
        squad.OnTargetChange += SetTarget;
        GameManager.PlayUpdate += SoldierUpdate;
    }

    /// <summary>
    /// Clears event links when soldier is destroyed
    /// </summary>
    private void OnDestroy()
    {
        squad.OnTargetChange -= SetTarget;
        GameManager.PlayUpdate -= SoldierUpdate;
    }

    /// <summary>
    /// SetDestination sets the soldier's NavMeshAgent SetDestination (easily called by Squad this way)
    /// </summary>
    /// <param name="_destination"></param>
    public void SetDestination(Vector3 _destination)
    {
        navAgent.SetDestination(_destination);
    }

    /// <summary>
    /// SetTarget is used to define the soldier's target, it is called when the squad OnTargetChange event is triggered
    /// </summary>
    /// <param name="_target"></param>
    private void SetTarget(Transform _target)
    {
        target = _target;
        secondaryTarget = null;
    }

    /// <summary>
    /// FaceTarget method is used to rotate the soldier toward its target
    /// </summary>
    /// <param name="_target"></param>
    private void FaceTarget(Transform _target)
    {
        // Gets the projection on XZ plane of the vector between target and squad positions
        Vector3 _diff = _target.position - transform.position;
        _diff = new Vector3(_diff.x, 0f, _diff.z);
        // Gets the angle between the _diff vector and the forward squad axe
        float _angle = Vector3.SignedAngle(transform.forward, Vector3.Normalize(_diff), transform.up);
        // Clamps that angle value depending of the NavMeshAgent parameters and rotates the squad
        _angle = Mathf.Clamp(_angle, -navAgent.angularSpeed * Time.deltaTime, navAgent.angularSpeed * Time.deltaTime);
        transform.Rotate(transform.up, _angle);
    }

    /// <summary>
    /// SoldierUpdate is the Update method of Soldier, it changes the soldier forward direction setting if a target is set or not
    /// </summary>
    void SoldierUpdate()
    {
        if (target == null)
        {
            navAgent.updateRotation = true;
        }
        else
        {
            navAgent.updateRotation = false;
            // if target reachable
                FaceTarget(target);
            // shoot target

            // else (target not reachable)
            // if a secondary target can be found
            secondaryTarget = Ranges.GetNearestTower(this.transform, data.shortRangeAttack,data.middleRangeAttack,data.longRangeAttack);
            if(secondaryTarget != null)
            {
                FaceTarget(secondaryTarget);
                // shoot secondary target
            }
            else
            {
                //else (no secondary target found)
                FaceTarget(target);
            }


        }
    }
}
