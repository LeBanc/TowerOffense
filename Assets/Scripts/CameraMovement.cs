using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Events
    public delegate void CamMoveEventHandler();
    public event CamMoveEventHandler OnCameraMovement;

    private Camera cam;

    private bool allowMoveDown;
    private bool allowMoveUp;

    public bool MoveUp
    {
        set { allowMoveUp = value; }
        get { return allowMoveUp; }
    }
    public bool MoveDown
    {
        set { allowMoveDown = value; }
        get { return allowMoveDown;}
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        allowMoveDown = true;
        allowMoveUp = true;

        PlayManager.OnLoadSquadsOnNewDay += EnableCameraMovement;
        PlayManager.OnHQPhase += DisableCameraMovement;
    }

    private void OnDestroy()
    {
        OnCameraMovement = null;
        DisableCameraMovement();
        PlayManager.OnLoadSquadsOnNewDay -= EnableCameraMovement;
        PlayManager.OnHQPhase -= DisableCameraMovement;
    }

    void EnableCameraMovement()
    {
        GameManager.PlayUpdate += CameraUpdate;
    }

    void DisableCameraMovement()
    {
        GameManager.PlayUpdate -= CameraUpdate;
    }

    void CameraUpdate()
    {
        // Camera movement with arrow keys (up and down)
        if (Input.GetKey(KeyCode.UpArrow) && allowMoveUp)
        {
            // Move Camera up and update HealthBars
            transform.position += new Vector3(0f, 0f, 1f);
            OnCameraMovement?.Invoke();
        }
        if (Input.GetKey(KeyCode.DownArrow) && allowMoveDown)
        {
            // Move Camera down and update HealthBars
            transform.position += new Vector3(0f, 0f, -1f);
            OnCameraMovement?.Invoke();
        }
    }
}
