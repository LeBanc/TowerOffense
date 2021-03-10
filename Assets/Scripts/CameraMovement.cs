using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Max Speed (m/s)
    public float maxSpeed = 100f;
    // Accelearation (m/s²)
    public float accel = 5f;


    // Events
    public delegate void CamMoveEventHandler();
    public event CamMoveEventHandler OnCameraMovement;

    private Camera cam;

    private bool allowMoveDown;
    private bool allowMoveUp;

    private float speed;

    private HQ hq;

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

        PlayManager.OnLoadSquadsOnNewDay += BeginDay;
        PlayManager.OnHQPhase += DisableCameraMovement;

        hq = FindObjectOfType<HQ>();
        if (hq == null) Debug.LogError("[CameraMovement] Cannot find HQ!");
    }

    private void OnDestroy()
    {
        OnCameraMovement = null;
        DisableCameraMovement();
        PlayManager.OnLoadSquadsOnNewDay -= BeginDay;
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
        // Camera movement with Vertical axe (keyboard & controller)
        if ((Input.GetAxis("Vertical") > 0) && allowMoveUp)
        {
            speed += accel;
            if (speed > maxSpeed) speed = maxSpeed;

            // Move Camera up and update HealthBars
            transform.position += new Vector3(0f, 0f, speed*Time.deltaTime);
            OnCameraMovement?.Invoke();
        }
        else if ((Input.GetAxis("Vertical") < 0) && allowMoveDown)
        {
            speed -= accel;
            if (speed < -maxSpeed) speed = -maxSpeed;

            // Move Camera down and update HealthBars
            transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
            OnCameraMovement?.Invoke();
        }
        else
        {
            // Stop movement
            if(speed > accel)
            {
                speed -= accel;
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            else if(speed < -accel)
            {
                speed += accel;
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            else
            {
                speed = 0;
            }
        }
    }

    void BeginDay()
    {
        if (!hq.GetComponentInChildren<Renderer>().isVisible)
        {
            if ((hq.transform.position.z) < transform.position.z)
            {
                StartCoroutine(MoveDownRoutine());
            }
            else if ((hq.transform.position.z) > transform.position.z)
            {
                StartCoroutine(MoveUpRoutine());
            }
        }
        else
        {
            EnableCameraMovement();
        }
    }

    IEnumerator MoveUpRoutine()
    {
        speed = 0f;
        // Move up to find HQ
        while (!hq.GetComponentInChildren<Renderer>().isVisible)
        {
            if (speed < 2 * maxSpeed)
            {
                speed += accel;
            }
            else
            {
                speed = 2 * maxSpeed;
            }
            if (allowMoveUp)
            {
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            yield return null;
        }
        
        // Wait for a small delay
        float delayStartTime = Time.time;
        while(delayStartTime + 0.35f > Time.time)
        {
            if (allowMoveUp)
            {
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            yield return null;
        }

        // Deceleration
        while(speed > accel)
        {
            speed -= accel;
            if (allowMoveUp)
            {
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            yield return null;
        }
        speed = 0;
        EnableCameraMovement();
    }

    IEnumerator MoveDownRoutine()
    {
        speed = 0f;
        // Move down to find HQ
        while (!hq.GetComponentInChildren<Renderer>().isVisible)
        {
            if (speed > -2 * maxSpeed)
            {
                speed -= accel;
            }
            else
            {
                speed = -2 * maxSpeed;
            }
            if (allowMoveDown)
            {
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }                
            yield return null;
        }

        // Wait for a small delay
        float delayStartTime = Time.time;
        while (delayStartTime + 0.2 > Time.time)
        {
            if (allowMoveDown)
            {
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            yield return null;
        }

        // Deceleration
        while (speed < -accel)
        {
            speed += accel;
            if (allowMoveDown)
            {
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            yield return null;
        }
        speed = 0;
        EnableCameraMovement();
    }
}
