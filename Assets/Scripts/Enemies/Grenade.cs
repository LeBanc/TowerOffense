using System.Collections;
using UnityEngine;

/// <summary>
/// Grenade class defines the grenade behaviour (from an Explosive Tower)
/// </summary>
public class Grenade : MonoBehaviour
{
    // public GameObjects
    public GameObject body;
    public GameObject explosionWarning;
    // public float for grenade speed (movement)
    public float speed = 20;
    // public components for FXs
    public AudioSource sound;
    public ParticleSystem particles;

    // private data
    private Vector3 origin;
    private Vector3 startPos;
    private Vector3 firstReboundPos;
    private Vector3 secondReboundPos;
    private Vector3 destinationPos;
    private float height;
    private float firstRatio;
    private float secondRatio;
    private int explosiveDamages = 0;
    private Renderer bodyRenderer;

    /// <summary>
    /// At Start, get the current location as origin, initializes ratio and fetches the Renderer of the grenade body
    /// </summary>
    private void Start()
    {
        origin = transform.position;
        firstRatio = 2.0f / 3.0f;
        secondRatio = 11.0f / 12.0f;
        bodyRenderer = body.GetComponent<Renderer>();
    }

    /// <summary>
    /// OnDestroy, stop the FXs and unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        if(sound != null) sound.Stop();
        if (particles != null) particles.Stop();
        PlayManager.OnEndDay -= Explode;
    }

    /// <summary>
    /// Launch method computes the data needed for the grenade movement and calls GrenadeMovement Coroutine
    /// </summary>
    /// <param name="_from">Position from where the grenade is launch (Vector3)</param>
    /// <param name="_to">Position to where the grenade will go (Vector3)</param>
    public void Launch(Vector3 _from,Vector3 _to)
    {
        // Set the grenade as active but hide the explosionWarning go
        body.SetActive(true);
        explosionWarning.SetActive(false);

        // Set start and destination positions
        startPos = _from;
        destinationPos = _to;
        
        // Compute rebounds positions
        RaycastHit _hit = new RaycastHit();
        firstReboundPos = startPos + firstRatio*(destinationPos- startPos);
        if (Physics.Raycast(firstReboundPos + 100 * Vector3.up, Vector3.down, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain" })))
        {
            firstReboundPos = _hit.point;
        }

        secondReboundPos = startPos + secondRatio * (destinationPos - startPos);
        if (Physics.Raycast(secondReboundPos + 100 * Vector3.up, Vector3.down, out _hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            secondReboundPos = _hit.point;
        }

        // Compute height value
        height = _from.y - _to.y;

        // Subscribe to OnEndDay event (to make the grenade explode when going back to HQ phase)
        PlayManager.OnEndDay += Explode;
        // Start the Coroutine that will make the grenade move
        StartCoroutine(GrenadeMovement());
    }

    /// <summary>
    /// GrenadeMovement coroutine update the grenade position from start to destination with two rebounds
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrenadeMovement()
    {
        // Projection of the grenade course on the (x,z) plane
        Vector3 _vect = new Vector3(destinationPos.x - startPos.x, 0, destinationPos.z - startPos.z);
        // Direction of the movement
        Vector3 _normVect = _vect.normalized;
        // Distance between start and destination on the (x,z) plane
        float _dist = _vect.magnitude;
        
        // Center of the ellipse (at first: base of startPos with firstRebound height)
        Vector3 _center = new Vector3(startPos.x, firstReboundPos.y, startPos.z);

        // Distance and height (to compute the position on an ellipse)
        float _d = 0.0f;
        float _h = 0.0f;

        // Between start and first rebound
        while(_d <= _dist*firstRatio)
        {
            _d += speed * Time.deltaTime;
            _h = (startPos.y - firstReboundPos.y) * Mathf.Sqrt(Mathf.Max(0.0f,(1 + (_d / (_dist * firstRatio))) * (1 - (_d / (_dist * firstRatio)))));
            transform.position = _center + _normVect * _d + (_h + firstReboundPos.y) * Vector3.up;
            yield return null;
        }

        // Between first and secound rebounds: center is the middle of both positions
        _center = new Vector3((firstReboundPos.x + secondReboundPos.x) / 2, secondReboundPos.y, (firstReboundPos.z+secondReboundPos.z)/2);
        float _centerDist = _dist * (firstRatio + secondRatio) / 2;
        while(_d <= _dist*secondRatio)
        {
            _d += speed * Time.deltaTime;
            _h = height/3 * Mathf.Sqrt(Mathf.Max(0.0f,(1 + ((_d-_centerDist) / (_dist * (secondRatio-firstRatio)))) * (1 - ((_d - _centerDist) / (_dist * (secondRatio-firstRatio))))));
            transform.position = _center + _normVect * (_d-_centerDist) + (_h + secondReboundPos.y) * Vector3.up;
            yield return null;
        }

        // Between secound rebound and destination: center is the middle of both positions
        _center = new Vector3((destinationPos.x + secondReboundPos.x) / 2, destinationPos.y, (destinationPos.z + secondReboundPos.z) / 2);
        _centerDist = _dist * (1 + secondRatio) / 2;
        while (_d <= _dist)
        {
            _d += speed * Time.deltaTime;
            _h = height / 6 * Mathf.Sqrt(Mathf.Max(0.0f,(1 + ((_d - _centerDist) / (_dist*(1-secondRatio)))) * (1 - ((_d - _centerDist) / (_dist*(1-secondRatio))))));
            transform.position = _center + _normVect * (_d-_centerDist) + (_h + secondReboundPos.y) * Vector3.up;
            yield return null;
        }

        // when distance is equals (or greater) than the max distance, set the grenade to its destination
        transform.position = destinationPos;
        // Call ExplosionWarining coroutine
        StartCoroutine(ExplosionWarning());
    }

    /// <summary>
    /// ExplosionWarning coroutine is used to display a sphere around the grenade to warn the player of the explosion
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExplosionWarning()
    {
        // Activate the warning game object
        explosionWarning.SetActive(true);

        // Display a growing sphere (from 0 to ShortRange) to overview the range of the incoming damages => 5 times (= 5 seconds)
        explosionWarning.transform.localScale = Vector3.zero;
        for (int i=0;i<5;i++)
        {
            float _time = 0.0f;
            while(_time < 1.0f)
            {
                explosionWarning.transform.localScale = _time*Vector3.one*PlayManager.data.shortRange;
                _time += Time.deltaTime;
                yield return null;
            }
        }

        // When the warning is done, make the grenade explode
        Explode();
        PlayFX(); // Effects in a dedicated method to avoid the sound to be played at day end
    }

    /// <summary>
    /// Explode method gets all the Shootables in the grenade explosion range and damages them (friends and foes)
    /// </summary>
    private void Explode()
    {
        // Unsubscribe from event and stop coroutines (in case the explosion was trigger by the EndDay event)
        PlayManager.OnEndDay -= Explode;
        StopAllCoroutines();

        // Hide the explosionWarning gameObject and the grenade renderer
        explosionWarning.SetActive(false);
        bodyRenderer.enabled = false;

        // Get the in range shootables and damages them
        foreach (Collider _c in Physics.OverlapSphere(transform.position,PlayManager.data.shortRange/2))
        {
            if(_c.TryGetComponent(out Shootable _s))
            {
                _s.DamageExplosive(explosiveDamages);
            }
        }

        if(particles == null) // Else it will be done after playing the particules effect
        {
            ResetGrenade();
        }
    }

    /// <summary>
    /// PlayFX method plays the sound and particles effects if they exist
    /// </summary>
    private void PlayFX()
    {
        if (sound != null) sound.Play();
        if (particles != null)
        {
            particles.Play();
            StartCoroutine(ResetPosition()); // Reset position only after the particles effect is complete
        }
    }

    /// <summary>
    /// ResetPosition coroutine waits for the particle effect to be complete and then reset the grenade position to its origin
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetPosition()
    {
        if(particles != null)
        {
            while (particles.isPlaying)
            {
                yield return null;
            }
        }
        ResetGrenade();
    }

    /// <summary>
    /// SetDamages method initializes the grenade damage (from ExplosiveTower data)
    /// </summary>
    /// <param name="_damage">Explosive damage of the grenade (int)</param>
    public void SetDamages(int _damage)
    {
        explosiveDamages = _damage;
    }

    /// <summary>
    /// ResetGrenade method resets the grenade to its default position and default state
    /// </summary>
    public void ResetGrenade()
    {
        transform.position = origin;
        body.SetActive(false);
        bodyRenderer.enabled = true;
    }
}
