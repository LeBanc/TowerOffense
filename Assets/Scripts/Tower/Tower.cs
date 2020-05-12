using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    // Boolean to set the tower's states
    private bool active = false;
    private bool destroyed = false;

    // Selected target of the tower
    private SoldierUnit selectedTarget;

    // Tower available cells in ranges
    private List<Vector3> shortRangeCells;
    private List<Vector3> middleRangeCells;
    private List<Vector3> longRangeCells;

    private int hp = 200;
    private int maxHP = 200;
    private int shortRangeAttack = 10;
    private int middleRangeAttack = 10;
    private int longRangeAttack = 10;
    private int shortRangeDefense = 0;
    private int middleRangeDefense = 0;
    private int longRangeDefense = 0;

    // temp
    private float shootingDelay=5f;
    private float shootingDataDuration=5f;

    // UI
    private HealthBar healthBar;

    // Events
    public delegate void TowerEventHandler();
    public event TowerEventHandler OnDestruction;

    // For debug purpose
    // /*
    private LineRenderer line;
    private Material _red;
    // */

    private void Start()
    {
        // Initialization
        GameManager.PlayUpdate += TowerUpdate;

        healthBar = PlayManager.AddHealthBar(transform);
        healthBar.Hide();

        // For debug purpose
        // /*
        _red = Resources.Load("Materials/UnlitRed_mat", typeof(Material)) as Material;
        line = gameObject.AddComponent<LineRenderer>();
        line.enabled = false;
        // */

        shortRangeCells = new List<Vector3>();
        middleRangeCells = new List<Vector3>();
        longRangeCells = new List<Vector3>();
        InitializeAvailableCells();
    }

    private void OnDestroy()
    {
        GameManager.PlayUpdate -= TowerUpdate;
    }

    /// <summary>
    /// Activate method activates the tower and do anything linked to the activation (GFX, SFX, etc.)
    /// </summary>
    private void Activate()
    {
        active = true;
        // GFX
        GetComponent<MeshRenderer>().material.color = Color.red;
        healthBar.Show();
        // SFX
    }

    private void DestroyTower()
    {
        destroyed = true;
        active = false;
        GameManager.PlayUpdate -= TowerUpdate;
        // GFX
        GetComponent<MeshRenderer>().material.color = Color.black;
        healthBar.Remove();
        // SFX

        // For debug purpose
        // /*
        line.enabled = false;
        // */
    }

    /// <summary>
    /// IsActive method returns true if tower is active and false otherwise
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return active;
    }

    private void SetTarget(SoldierUnit _target)
    {
        if (selectedTarget != null) ClearTarget();
        selectedTarget = _target;
        if (selectedTarget != null)
        {
            selectedTarget.OnWounded += ClearTarget;
        }
    }

    private void ClearTarget()
    {
        if (selectedTarget != null) selectedTarget.OnWounded -= ClearTarget;
        selectedTarget = null;
    }

    /// <summary>
    /// TowerUpdate is the Update methods of the Tower, to call in Update in test scene or to add to GameManager event in game
    /// </summary>
    private void TowerUpdate()
    {
        SoldierUnit _target = Ranges.GetNearestSoldier(this.transform, 1, 1, 1);
        if (_target != selectedTarget) SetTarget(_target);
        
        if (!active && selectedTarget != null) Activate();

        //healthBar.rectTransform.position =  Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f,10f,3f));

        // For debug purpose
        // /*
        // Checks if a target is selected and draws a line to it
        if (selectedTarget != null)
        {
            Vector3[] _positions = new Vector3[] { transform.position, selectedTarget.transform.position };
            line.material = _red;
            line.SetPositions(_positions);
            line.enabled = true;

            Shoot(selectedTarget);
        }
        else
        {
            line.enabled = false;
        }
        // */

        shootingDelay = Mathf.Max(0f, shootingDelay - Time.deltaTime);
    }

    private void InitializeAvailableCells()
    {
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                if (i == 0 && j == 0) continue;

                Vector3 _cell = new Vector3((float)10 * i, 0f, (float)10 * j);

                RaycastHit _hit;
                Ray _ray = new Ray(GridAdjustment.GetGridCoordinates(transform.position) + _cell + 100 * Vector3.up, -Vector3.up);
                if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain", "Buildings" })))
                {
                    if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {
                        _hit = new RaycastHit();
                        _ray = new Ray(transform.position, (_cell - 7.5f * Vector3.up).normalized);
                        if (!Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings" })))
                        {
                            if (_cell.magnitude <= (PlayManager.ShortRange + 5f)) shortRangeCells.Add(_cell);
                            else if (_cell.magnitude <= (PlayManager.MiddleRange + 5f)) middleRangeCells.Add(_cell);
                            else if (_cell.magnitude <= (PlayManager.LongRange + 5f)) longRangeCells.Add(_cell);
                        }
                    }
                }
            }
        }
    }

    public List<Vector3> ShortRangeCells
    {
        get { return shortRangeCells; }
    }
    public List<Vector3> MiddleRangeCells
    {
        get { return middleRangeCells; }
    }
    public List<Vector3> LongRangeCells
    {
        get { return longRangeCells; }
    }

    public void DamageShortRange(int dmg)
    {
        int _temp = dmg - shortRangeDefense;
        if (_temp > 0) DamageTower(_temp);
    }

    public void DamageMiddleRange(int dmg)
    {
        int _temp = dmg - middleRangeDefense;
        if (_temp > 0) DamageTower(_temp);
    }

    public void DamageLongRange(int dmg)
    {
        int _temp = dmg - longRangeDefense;
        if (_temp > 0) DamageTower(_temp);
    }

    private void DamageTower(int dmg)
    {
        hp -= dmg;
        if(hp <= 0)
        {
            hp = 0;
            DestroyTower();
            OnDestruction?.Invoke();
        }
        healthBar.UpdateValue(hp, maxHP);
    }

    private void Shoot(SoldierUnit _t)
    {
        if (shootingDelay > 0f) return;

        if (Ranges.IsInShortRange(transform, _t))
        {
            _t.DamageShortRange(shortRangeAttack);
        }
        else if (Ranges.IsInMiddleRange(transform, _t))
        {
            _t.DamageMiddleRange(middleRangeAttack);
        }
        else if (Ranges.IsInLongRange(transform, _t))
        {
            _t.DamageLongRange(longRangeAttack);
        }
        else
        {
            return;
        }
        shootingDelay = shootingDataDuration;
    }
}
