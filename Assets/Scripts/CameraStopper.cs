using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopper : MonoBehaviour
{

    public enum Direction
    {
        Up,
        Down
    }

    public Direction blockedDirection = Direction.Up;
    
    private CameraMovement activeCamMove;

    private void Start()
    {
        activeCamMove = Camera.main.GetComponent<CameraMovement>();
        if (activeCamMove == null) Debug.LogError("[CameraStopper] Cannot find main camera CameraMovement script!");
    }


    private void OnBecameVisible()
    {
        if(blockedDirection == Direction.Up)
        {
            activeCamMove.MoveUp = false;
        }
        else if(blockedDirection == Direction.Down)
        {
            activeCamMove.MoveDown = false;
        }
    }

    private void OnBecameInvisible()
    {
        if (blockedDirection == Direction.Up)
        {
            activeCamMove.MoveUp = true;
        }
        else if (blockedDirection == Direction.Down)
        {
            activeCamMove.MoveDown = true;
        }
    }
}
