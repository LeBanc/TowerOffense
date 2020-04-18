using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{

    private NavMeshAgent navAgent;
    private Squad squad;
    private Transform target;

    /// <summary>
    /// Soldier setup sets the parent squad, the soldier's navMeshAgent and the events used
    /// </summary>
    /// <param name="_squad"></param>
    public void Setup(Squad _squad)
    {
        squad = _squad;
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
    /// <param name="_target"></param>
    public void SetDestination(Vector3 _target)
    {
        navAgent.SetDestination(_target);
    }

    /// <summary>
    /// SetTarget is used to define the soldier's target, it is called when the squad OnTargetChange event is triggered
    /// </summary>
    /// <param name="_target"></param>
    private void SetTarget(Transform _target)
    {
        target = _target;
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
            FaceTarget(target);
        }
    }
}
