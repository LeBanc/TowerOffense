using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ranges is a utility class to get easily nearest soldier and tower and know if a shoot is possible
/// </summary>
public static class Ranges
{
    /// <summary>
    /// GetNearsestSoldier searches for the nearest Soldier or Turret in the prefered ranges order
    /// </summary>
    /// <param name="_source">Shootable source (tower or whatever - Shootable to get access to the HitPoints of the tower)</param>
    /// <param name="_shortRangeCoeff">short range coefficient for sort order (attack or defense value)</param>
    /// <param name="_middleRangeCoeff">middle range coefficient for sort order (attack or defense value)</param>
    /// <param name="_longRangeCoeff">long range coefficient for sort order (attack or defense value)</param>
    /// <returns>Returns the nearest soldier (or turret) in preferred range and null if no soldier available</returns>
    public static Shootable GetNearestSoldier(Shootable _source, int _shortRangeCoeff, int _middleRangeCoeff, int _longRangeCoeff)
    {
        Shootable _result = null;
        // Cas s>=m>=l et s>=l>=m
        if(_shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff >= _longRangeCoeff && _shortRangeCoeff > 0)
        {
            _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
            if (_result == null)
            {
                // Cas s=>m>=l
                if(_middleRangeCoeff >= _longRangeCoeff && _middleRangeCoeff > 0)
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                    if(_result == null && _longRangeCoeff>0) _result = GetNearestSoldierInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
                }
                else if(_longRangeCoeff > 0) //_longRangeCoeff > _middleRangeCoeff => Cas s>=l>=m
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
                    if (_result == null && _middleRangeCoeff > 0) _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                }
            }
        }
        // Cas m>=s>=l et m>=l>=s
        else if(_middleRangeCoeff >= _longRangeCoeff && _middleRangeCoeff > 0)
        {
            _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange,PlayManager.ShortRange);
            if (_result == null)
            {
                // Cas m>=s>=l
                if (_shortRangeCoeff >= _longRangeCoeff && _shortRangeCoeff > 0)
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
                    if (_result == null && _longRangeCoeff > 0) _result = GetNearestSoldierInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
                }
                else if(_longRangeCoeff > 0) // _longRangeCoeff > _shortRangeCoeff => Cas m=>l>=s
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
                    if (_result == null && _shortRangeCoeff > 0) _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
                }
            }
        }
        // Cas l>=s>=m et l>=m>=s
        else if(_longRangeCoeff > 0)
        {
            _result = GetNearestSoldierInRange(_source, PlayManager.LongRange,PlayManager.MiddleRange);
            if (_result == null)
            {
                // Cas l>=s>=m
                if (_shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff > 0)
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
                    if (_result == null && _middleRangeCoeff > 0) _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                }
                else if(_middleRangeCoeff > 0) // _middleRangeCoeff > _shortRangeCoeff => Cas _l>=m>=s
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                    if (_result == null && _shortRangeCoeff > 0) _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
                }
            }
        }
        return _result;
    }

    /// <summary>
    /// GetNearestSoldierInRange search for the nearest soldier or turret within a range
    /// </summary>
    /// <param name="_source">Transform source (tower or whatever)</param>
    /// <param name="_rangeMax">float for max range</param>
    /// <param name="_rangeMin">float for min range (optional)</param>
    /// <returns>Returns the transform of the nearest soldier or the source transform if no soldier found</returns>
    private static Shootable GetNearestSoldierInRange(Shootable _source, float _rangeMax, float _rangeMin = 0f)
    {
        List<Shootable> _targets = new List<Shootable>();
        Ray _ray;
        RaycastHit _hit;
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(GridAdjustment.GetGridCoordinates(_source.transform.position), _rangeMax +15f, LayerMask.GetMask("Soldiers"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent(out SoldierUnit _soldier))
                {
                    foreach(Transform _t in _source.hitList)
                    {
                        _ray = new Ray(_t.position, (c.transform.position - _t.position).normalized);
                        if (Physics.Raycast(_ray, out _hit, PlayManager.LongRange, LayerMask.GetMask(new string[] { "Soldiers", "Buildings", "Terrain" })))
                        {
                            if (_hit.collider.gameObject == c.gameObject && !_soldier.IsWounded && IsInRange(_source.transform, _hit.transform, _rangeMax, _rangeMin))
                            {
                                _targets.Add(_soldier);
                            }
                        }
                    }
                }
                if (c.TryGetComponent(out Turret _turret))
                {
                    foreach(Transform _t in _source.hitList)
                    {
                        _ray = new Ray(_t.position, (c.transform.position - _t.position).normalized);
                        if (Physics.Raycast(_ray, out _hit, PlayManager.LongRange, LayerMask.GetMask(new string[] { "Soldiers", "Buildings", "Terrain" })))
                        {
                            if (_hit.collider.gameObject == c.gameObject && IsInRange(_source.transform, _hit.transform, _rangeMax, _rangeMin)) _targets.Add(_turret);
                        }
                    }                    
                }
            }
        }
        return GetNearestInList<Shootable>(_source.transform, _targets);
    }

    /// <summary>
    /// GetNearestEnemy searches for the nearest Enemy in the prefered ranges order
    /// </summary>
    /// <param name="_source">Transform source (squad, soldier, etc.)</param>
    /// <param name="_shortRangeCoeff">int for short range coefficient (attack or defense)</param>
    /// <param name="_middleRangeCoeff">int for middle range coefficient (attack or defense)</param>
    /// <param name="_longRangeCoeff">int for long range coefficient (attack or defense)</param>
    /// <returns>Returns the nearest enemy in preferred range and null if no tower available</returns>
    public static Enemy GetNearestEnemy(Transform _source, int _shortRangeCoeff, int _middleRangeCoeff, int _longRangeCoeff)
    {
        Enemy _result = null;
        // Cas s>=m>=l et s>=l>=m
        if (_shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff >= _longRangeCoeff && _shortRangeCoeff > 0)
        {
            _result = GetNearestEnemyShort(_source);
            if (_result == null)
            {
                // Cas s=>m>=l
                if (_middleRangeCoeff >= _longRangeCoeff && _middleRangeCoeff > 0)
                {
                    _result = GetNearestEnemyMiddle(_source);
                    if (_result == null && _longRangeCoeff > 0) _result = GetNearestEnemyLong(_source);
                }
                else if (_longRangeCoeff > 0) //_longRangeCoeff > _middleRangeCoeff => Cas s>=l>=m
                {
                    _result = GetNearestEnemyLong(_source);
                    if (_result == null && _middleRangeCoeff > 0) _result = GetNearestEnemyMiddle(_source);
                }
            }
        }
        // Cas m>=s>=l et m>=l>=s
        else if (_middleRangeCoeff >= _longRangeCoeff && _middleRangeCoeff > 0)
        {
            _result = GetNearestEnemyMiddle(_source);
            if (_result == null)
            {
                // Cas m>=s>=l
                if (_shortRangeCoeff >= _longRangeCoeff && _shortRangeCoeff > 0)
                {
                    _result = GetNearestEnemyShort(_source);
                    if (_result == null && _longRangeCoeff > 0) _result = GetNearestEnemyLong(_source);
                }
                else if (_longRangeCoeff > 0) // _longRangeCoeff > _shortRangeCoeff => Cas m=>l>=s
                {
                    _result = GetNearestEnemyLong(_source);
                    if (_result == null && _shortRangeCoeff > 0) _result = GetNearestEnemyShort(_source);
                }
            }
        }
        // Cas l>=s>=m et l>=m>=s
        else if (_longRangeCoeff > 0)
        {
            _result = GetNearestEnemyLong(_source);
            if (_result == null)
            {
                // Cas l>=s>=m
                if (_shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff > 0)
                {
                    _result = GetNearestEnemyShort(_source);
                    if (_result == null && _middleRangeCoeff > 0) _result = GetNearestEnemyMiddle(_source);
                }
                else if (_middleRangeCoeff > 0) // _middleRangeCoeff > _shortRangeCoeff => Cas _l>=m>=s
                {
                    _result = GetNearestEnemyMiddle(_source);
                    if (_result == null && _shortRangeCoeff > 0) _result = GetNearestEnemyShort(_source);
                }
            }
        }
        return _result;
    }

    /// <summary>
    /// GetNearestEnemyShort search for the nearest Enemy in short range
    /// </summary>
    /// <param name="_source">Transform source (squad, soldier or whatever)</param>
    /// <returns>Returns the nearest Enemy or null if not found</returns>
    private static Enemy GetNearestEnemyShort(Transform _source)
    {
        List<Enemy> _targets = new List<Enemy>();
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(GridAdjustment.GetGridCoordinates(_source.position), PlayManager.ShortRange +15f, LayerMask.GetMask("Enemies"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent(out Enemy _enemy))
                {
                    if (!_enemy.IsDestroyed() && IsShootableShort(_source, _enemy))
                    {
                        _targets.Add(_enemy);
                    }
                }
            } 
        }
        return GetNearestInList<Enemy>(_source, _targets);
    }

    /// <summary>
    /// GetNearestEnemyMiddle search for the nearest Enemy in middle range
    /// </summary>
    /// <param name="_source">Transform source (squad, soldier or whatever)</param>
    /// <returns>Returns the nearest Enemy or null if not found</returns>
    private static Enemy GetNearestEnemyMiddle(Transform _source)
    {
        List<Enemy> _targets = new List<Enemy>();
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(GridAdjustment.GetGridCoordinates(_source.position), PlayManager.MiddleRange +15f, LayerMask.GetMask("Enemies"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent(out Enemy _enemy))
                {
                    if (!_enemy.IsDestroyed() && IsShootableMiddle(_source, _enemy))
                    {
                        _targets.Add(_enemy);
                    }
                }
            }
        }
        return GetNearestInList<Enemy>(_source, _targets);
    }

    /// <summary>
    /// GetNearestEnemyLong search for the nearest Enemy in long range
    /// </summary>
    /// <param name="_source">Transform source (squad, soldier or whatever)</param>
    /// <returns>Returns the nearest Enemy or null if not found</returns>
    private static Enemy GetNearestEnemyLong(Transform _source)
    {
        List<Enemy> _targets = new List<Enemy>();
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(GridAdjustment.GetGridCoordinates(_source.position), PlayManager.LongRange +15f, LayerMask.GetMask("Enemies"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent(out Enemy _enemy))
                {
                    if (!_enemy.IsDestroyed() && IsShootableLong(_source, _enemy))
                    {
                        _targets.Add(_enemy);
                    }
                }
            }
        }
        return GetNearestInList<Enemy>(_source, _targets);
    }
       
    /// <summary>
    /// GetNearestSquad searches in the SquadUnit list of the PlayManager which SquadUnit is the nearest
    /// </summary>
    /// <param name="_source">Transform of the source</param>
    /// <returns>Nearest SquadUnit</returns>
    public static SquadUnit GetNearestSquad(Transform _source)
    {
        return GetNearestInList<SquadUnit>(_source, PlayManager.squadUnitList);
    }

    /// <summary>
    /// GetNearestInList searches in the List for the element the nearest from the source transform
    /// </summary>
    /// <param name="_source">Transform source</param>
    /// <param name="_list">List of T</param>
    /// <returns>Returns the transform that is nearer from the source than the other</returns>
    public static T GetNearestInList<T>(Transform _source, List<T> _list) where T : MonoBehaviour
    {
        if (_list.Count == 0) return default(T);

        T _result = default(T);
        float _distance = Mathf.Infinity;
        foreach(T t in _list)
        {
            if ((t.transform.position - _source.position).magnitude < _distance)
            {
                _result = t;
                _distance = (t.transform.position - _source.position).magnitude;
            }
        }
        return _result;
    }

    /// <summary>
    /// IsTowerShootable looks if a Tower transform is at range and in sight for shooting at it
    /// </summary>
    /// <param name="_source">Source Transform</param>
    /// <param name="_target">Target Transform</param>
    /// <param name="_rangeMax">float for max range</param>
    /// <param name="_rangeMin">float for min range (optional)</param>
    /// <returns>True if tower is shootable, false otherwise</returns>
    private static bool IsShootable(Transform _source, Transform _target, float _rangeMax, float _rangeMin = 0f)
    {
        string _targetLayer = LayerMask.LayerToName(_target.gameObject.layer);

        if(IsInRange(_source, _target, _rangeMax, _rangeMin))
        {
            Ray _ray = new Ray(_source.position, (_target.position - _source.position).normalized);
            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { _targetLayer, "Buildings", "Terrain" })))
            {
                if (_hit.transform == _target || _hit.transform == _target.parent) return true;
            }
        }
        return false;
    }

    public static bool IsShootableShort<T>(Transform _source, T _target) where T : Shootable
    {
        bool _res = false;
        foreach(Transform _t in _target.hitList)
        {
            _res = _res || IsShootable(_source, _t, PlayManager.ShortRange);
        }
        return _res;
    }

    public static bool IsShootableMiddle<T>(Transform _source, T _target) where T : Shootable
    {
        bool _res = false;
        foreach (Transform _t in _target.hitList)
        {
            _res = _res || IsShootable(_source, _t, PlayManager.MiddleRange, PlayManager.ShortRange);
        }
        return _res;
    }

    public static bool IsShootableLong<T>(Transform _source, T _target) where T : Shootable
    {
        bool _res = false;
        foreach (Transform _t in _target.hitList)
        {
            _res = _res || IsShootable(_source, _t, PlayManager.LongRange, PlayManager.MiddleRange);
        }
        return _res;
    }

    /// <summary>
    /// IsInRange checks if the target transform is in range from the source transform
    /// </summary>
    /// <param name="_source">Source Transform</param>
    /// <param name="_target">Target Transform</param>
    /// <param name="_rangeMax">float for max range</param>
    /// <param name="_rangeMin">float for min range (optional)</param>
    /// <returns>True if the target is within max and min ranges, false otherwise</returns>
    private static bool IsInRange(Transform _source, Transform _target, float _rangeMax, float _rangeMin = 0f)
    {
        float _distance = (GridAdjustment.GetGridCoordinates(_target.position) - GridAdjustment.GetGridCoordinates(_source.position)).magnitude;
        return (_distance <= _rangeMax +5f && ((_rangeMin>0)?(_distance > _rangeMin + 5f):true));
    }

    public static bool IsInShortRange<T>(Transform _source, T _target) where T : Shootable
    {
        bool _res = false;
        foreach (Transform _t in _target.hitList)
        {
            _res = _res || IsInRange(_source, _t, PlayManager.ShortRange);
        }
        return _res;
    }

    public static bool IsInMiddleRange<T>(Transform _source, T _target) where T : Shootable
    {
        bool _res = false;
        foreach (Transform _t in _target.hitList)
        {
            _res = _res || IsInRange(_source, _t, PlayManager.MiddleRange, PlayManager.ShortRange);
        }
        return _res;
    }

    public static bool IsInLongRange<T>(Transform _source, T _target) where T : Shootable
    {
        bool _res = false;
        foreach (Transform _t in _target.hitList)
        {
            _res = _res || IsInRange(_source, _t, PlayManager.LongRange, PlayManager.MiddleRange);
        }
        return _res;
    }
}
