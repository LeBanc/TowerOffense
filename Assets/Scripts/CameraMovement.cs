using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Keyboard & controller data
    public float keyboardMaxSpeed = 100f;
    public float keyboardSensitivity = 0.5f;

    // Mouse wheel data
    public float wheelMaxSpeed = 200f;
    public float wheelSensitivity = 10f;

    // Accelearation (m/s²)
    public float accel = 5f;


    // Events
    public delegate void CamMoveEventHandler();
    public event CamMoveEventHandler OnCameraMovement;

    private Camera cam;

    private bool allowMoveDown;
    private bool allowMoveUp;

    private float speed;
    private float currentMaxSpeed;
    private float destination;

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

        destination = 0f;
        currentMaxSpeed = keyboardMaxSpeed;
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
        float _keyboardInput = Input.GetAxis("Vertical");
        float _mouseInput = Input.mouseScrollDelta.y;

        // Get destination from keyboard and controller
        if (_keyboardInput != 0f)
        {
            currentMaxSpeed = keyboardMaxSpeed;
            if (Mathf.Sign(destination) != Mathf.Sign(_keyboardInput) && destination != 0f)
            {
                destination = 0f;
            }
            else
            {
                destination += keyboardSensitivity * _keyboardInput;
            }
        }

        // Get destination from mouse wheel
        if(_mouseInput != 0f)
        {
            currentMaxSpeed = wheelMaxSpeed;
            if (Mathf.Sign(destination) != Mathf.Sign(_mouseInput) && destination != 0f)
            {
                destination = 0f;
            }
            else
            {
                destination += wheelSensitivity * _mouseInput;
            }
        }


        // Move the camera to the destination (with accel and decel)
        if(destination > 0f && allowMoveUp)
        {
            // Move up to destination
            speed += accel;
            if (speed > currentMaxSpeed) speed = currentMaxSpeed;

            // Move Camera up and update HealthBars
            transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
            destination -= speed * Time.deltaTime;
            if (destination < 0f) destination = 0f;
            OnCameraMovement?.Invoke();

        }
        else if (destination < 0f && allowMoveDown)
        {
            // Move down to destination
            speed -= accel;
            if (speed < -currentMaxSpeed) speed = -currentMaxSpeed;

            // Move Camera down and update HealthBars
            transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
            destination -= speed * Time.deltaTime;
            if (destination > 0f) destination = 0f;

            OnCameraMovement?.Invoke();
        }
        else
        {
            // Reduce speed and stop movement
            if (speed > accel)
            {
                speed -= accel;
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            else if (speed < -accel)
            {
                speed += accel;
                transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
                OnCameraMovement?.Invoke();
            }
            else
            {
                speed = 0f;
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
            if (speed < 2 * keyboardMaxSpeed)
            {
                speed += accel;
            }
            else
            {
                speed = 2 * keyboardMaxSpeed;
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
            if (speed > -2 * keyboardMaxSpeed)
            {
                speed -= accel;
            }
            else
            {
                speed = -2 * keyboardMaxSpeed;
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
