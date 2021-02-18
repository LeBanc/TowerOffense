using UnityEngine;

public class SoldierAnimatorTrigger : MonoBehaviour
{
    public Animator animator;

    private void UpdateSpeedMessage(float _speed)
    {
        animator.SetFloat("Speed", _speed);
    }

    private void UpdateVelocityMessage(Vector3 _velocity)
    {
        animator.SetFloat("Speed", _velocity.magnitude);
        animator.SetFloat("XVelocity", _velocity.x);
        animator.SetFloat("ZVelocity", _velocity.z);
    }

    private void ShootMessage()
    {
        animator.SetTrigger("Shoot");
    }

    private void BuildMessage(bool _value)
    {
        animator.SetBool("Build", _value);
    }

    private void WoundedMessage(bool _value)
    {
        animator.SetBool("Wounded", _value);
    }

    private void DieMessage()
    {
        animator.SetTrigger("Die");
    }
}
