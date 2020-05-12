using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public static class Ranges
{
    /// <summary>
    /// GetNearsestSoldier searches for the nearest Soldier in the prefered ranges order
    /// </summary>
    /// <param name="_source">Transform source (tower or whatever)</param>
    /// <param name="_shortRangeCoeff">short range coefficient for sort order (attack or defense value)</param>
    /// <param name="_middleRangeCoeff">middle range coefficient for sort order (attack or defense value)</param>
    /// <param name="_longRangeCoeff">long range coefficient for sort order (attack or defense value)</param>
    /// <returns>Returns the nearest soldier in preferred range and null if no soldier available</returns>
    public static SoldierUnit GetNearestSoldier(Transform _source, int _shortRangeCoeff, int _middleRangeCoeff, int _longRangeCoeff)
    {
        SoldierUnit _result = null;
        if(_shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff >= _longRangeCoeff)
        {
            _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
            if (_result == null)
            {
                if(_middleRangeCoeff >= _longRangeCoeff)
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                    if(_result == null) _result = GetNearestSoldierInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
                }
            }
        }
        else if(_middleRangeCoeff >= _longRangeCoeff)
        {
            _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange,PlayManager.ShortRange);
            if (_result == null)
            {
                if (_shortRangeCoeff >= _longRangeCoeff)
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
                    if (_result == null) _result = GetNearestSoldierInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
                }
            }
        }
        else
        {
            _result = GetNearestSoldierInRange(_source, PlayManager.LongRange,PlayManager.MiddleRange);
            if (_result == null)
            {
                if (_shortRangeCoeff >= _middleRangeCoeff)
                {
                    _result = GetNearestSoldierInRange(_source, PlayManager.ShortRange);
                    if (_result == null) _result = GetNearestSoldierInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                }
            }
        }
        return _result;
    }

    /// <summary>
    /// GetNearestSoldierInRange search for the nearest soldier within a range
    /// </summary>
    /// <param name="_source">Transform source (tower or whatever)</param>
    /// <param name="_rangeMax">float for max range</param>
    /// <param name="_rangeMin">float for min range (optional)</param>
    /// <returns>Returns the transform of the nearest soldier or the source transform if no soldier found</returns>
    public static SoldierUnit GetNearestSoldierInRange(Transform _source, float _rangeMax, float _rangeMin = 0f)
    {
        List<SoldierUnit> _targets = new List<SoldierUnit>();
        Ray _ray;
        RaycastHit _hit;
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(_source.position, _rangeMax, LayerMask.GetMask("Soldiers"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if (c.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                {
                    _ray = new Ray(_source.position, (c.transform.position - _source.position).normalized);
                    if (Physics.Raycast(_ray, out _hit))
                    {
                        if (_hit.collider.gameObject == c.gameObject && !_soldier.IsWounded()) _targets.Add(_soldier);
                    }
                }                    
            }
        }

        if (_rangeMin > 0f && _rangeMin < _rangeMax)
        {
            _foundTransforms = Physics.OverlapSphere(_source.position, _rangeMin, LayerMask.GetMask("Soldiers"));
            if (_foundTransforms.Length > 0)
            {
                foreach (Collider c in _foundTransforms)
                {
                    if (c.TryGetComponent<SoldierUnit>(out SoldierUnit _soldier))
                    {
                        _ray = new Ray(_source.position, (c.transform.position - _source.position).normalized);
                        if (Physics.Raycast(_ray, out _hit))
                        {
                            if (_hit.collider.gameObject == c.gameObject) _targets.Add(_soldier);
                        }
                    }
                }
            }
        }
        return GetNearestInList<SoldierUnit>(_source, _targets);
    }

    /// <summary>
    /// GetNearestTower searches for the nearest Tower in the prefered ranges order
    /// </summary>
    /// <param name="_source">Transform source (squad, soldier, etc.)</param>
    /// <param name="_shortRangeCoeff">int for short range coefficient (attack or defense)</param>
    /// <param name="_middleRangeCoeff">int for middle range coefficient (attack or defense)</param>
    /// <param name="_longRangeCoeff">int for long range coefficient (attack or defense)</param>
    /// <returns>Returns the nearest tower in preferred range and null if no tower available</returns>
    public static Tower GetNearestTower(Transform _source, int _shortRangeCoeff, int _middleRangeCoeff, int _longRangeCoeff)
    {
        Tower _result = null;
        if (_shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff >= _longRangeCoeff && _shortRangeCoeff > 0)
        {
            _result = GetNearestTowerInRange(_source, PlayManager.ShortRange);
            if (_result == null && _middleRangeCoeff >= _longRangeCoeff && _middleRangeCoeff > 0)
            {
                _result = GetNearestTowerInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
                if (_result == null && _longRangeCoeff > 0) _result = GetNearestTowerInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
            }
        }
        else if (_middleRangeCoeff >= _longRangeCoeff && _middleRangeCoeff >= 0 && _middleRangeCoeff > 0)
        {
            _result = GetNearestTowerInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
            if (_result == null && _shortRangeCoeff >= _longRangeCoeff && _shortRangeCoeff > 0)
            {
                _result = GetNearestTowerInRange(_source, PlayManager.ShortRange);
                if (_result == null && _longRangeCoeff > 0) _result = GetNearestTowerInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
            }
        }
        else if(_longRangeCoeff > 0)
        {
            _result = GetNearestTowerInRange(_source, PlayManager.LongRange, PlayManager.MiddleRange);
            if (_result == null && _shortRangeCoeff >= _middleRangeCoeff && _shortRangeCoeff > 0)
            {
                _result = GetNearestTowerInRange(_source, PlayManager.ShortRange);
                if (_result == null && _middleRangeCoeff > 0) _result = GetNearestTowerInRange(_source, PlayManager.MiddleRange, PlayManager.ShortRange);
            }
        }
        return _result;
    }

    /// <summary>
    /// GetNearestTowerInRange search for the nearest tower within a range
    /// </summary>
    /// <param name="_source">Transform source (squad, soldier or whatever)</param>
    /// <param name="_rangeMax">float for max range</param>
    /// <param name="_rangeMin">float for min range (optional)</param>
    /// <returns>Returns the transform of the nearest tower or the source transform if no soldier found</returns>
    public static Tower GetNearestTowerInRange(Transform _source, float _rangeMax, float _rangeMin = 0f)
    {
        List<Tower> _targets = new List<Tower>();
        Ray _ray;
        RaycastHit _hit;
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(_source.position, _rangeMax, LayerMask.GetMask("Buildings"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                if(c.TryGetComponent<Tower>(out Tower _tower))
                {
                    _ray = new Ray(_source.position, (c.transform.position - _source.position).normalized);
                    if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] {"Buildings" , "Terrain" })))
                    {
                        if (_hit.collider.gameObject == c.gameObject && _tower.IsActive()) _targets.Add(_tower);
                    }
                }
            }
        }

        if (_rangeMin > 0f && _rangeMin < _rangeMax)
        {
            _foundTransforms = Physics.OverlapSphere(_source.position, _rangeMin, LayerMask.GetMask("Buildings"));
            if (_foundTransforms.Length > 0)
            {
                foreach (Collider c in _foundTransforms)
                {
                    if (c.TryGetComponent<Tower>(out Tower _tower))
                    {
                        _ray = new Ray(_source.position, (c.transform.position - _source.position).normalized);
                        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Terrain" })))
                        {
                            if (_hit.collider.gameObject == c.gameObject) _targets.Remove(_tower);
                        }
                    }
                }
            }
        }
        return GetNearestInList<Tower>(_source, _targets);
    }

    /// <summary>
    /// GetNearestInList searches in the List for the element the nearest from the source transform
    /// </summary>
    /// <param name="_source">Transform source</param>
    /// <param name="_list">List of T</param>
    /// <returns>Returns the transform that is nearer from the source than the other</returns>
    private static T GetNearestInList<T>(Transform _source, List<T> _list) where T : MonoBehaviour
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
        if(IsInRange(_source, _target, _rangeMax, _rangeMin))
        {
            Ray _ray = new Ray(_source.position, (_target.position - _source.position).normalized);
            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Terrain" })))
            {
                if (_hit.transform == _target) return true;
            }
        }
        return false;
    }

    public static bool IsShootableMiddle<T>(Transform _source, T _target) where T : MonoBehaviour
    {
        return IsShootable(_source, _target.transform, PlayManager.ShortRange);
            
    }

    public static bool IsShootableLong<T>(Transform _source, T _target) where T : MonoBehaviour
    {
        return IsShootable(_source, _target.transform, PlayManager.MiddleRange, PlayManager.ShortRange);

    }

    public static bool IsShootableShort<T>(Transform _source, T _target) where T : MonoBehaviour
    {
        return IsShootable(_source, _target.transform, PlayManager.LongRange, PlayManager.MiddleRange);

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
        return (_distance <= _rangeMax +5f && _distance > _rangeMin + 5f);
    }

    public static bool IsInShortRange<T>(Transform _source, T _target) where T : MonoBehaviour
    {
        return IsInRange(_source, _target.transform, PlayManager.ShortRange);
    }

    public static bool IsInMiddleRange<T>(Transform _source, T _target) where T : MonoBehaviour
    {
        return IsInRange(_source, _target.transform, PlayManager.MiddleRange,PlayManager.ShortRange);
    }

    public static bool IsInLongRange<T>(Transform _source, T _target) where T : MonoBehaviour
    {
        return IsInRange(_source, _target.transform, PlayManager.LongRange,PlayManager.MiddleRange);
    }
}
