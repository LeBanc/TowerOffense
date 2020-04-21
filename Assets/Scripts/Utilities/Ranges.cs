using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ranges
{
    public static Transform GetNearestSoldier(Transform _source, int _shortRangeCoeff, int _middleRangeCoeff, int _longRangeCoeff)
    {
        Transform _result = _source;
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

        if (_result == _source) return null;
        return _result;
    }

    public static Transform GetNearestSoldierInRange(Transform _source, float _rangeMax, float _rangeMin = 0f)
    {
        List<Transform> _targets = new List<Transform>();
        Ray _ray;
        RaycastHit _hit;
        Collider[] _foundTransforms;
        _foundTransforms = Physics.OverlapSphere(_source.position, _rangeMax, LayerMask.GetMask("Soldiers"));
        if (_foundTransforms.Length > 0)
        {
            foreach (Collider c in _foundTransforms)
            {
                _ray = new Ray(_source.position, (c.transform.position - _source.position).normalized);
                if(Physics.Raycast(_ray,out _hit))
                {
                    if(_hit.collider.gameObject == c.gameObject) _targets.Add(c.transform);
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
                    _ray = new Ray(_source.position, (c.transform.position - _source.position).normalized);
                    if (Physics.Raycast(_ray, out _hit))
                    {
                        if (_hit.collider.gameObject == c.gameObject) _targets.Remove(c.transform);
                    }
                }
            }
        }
        return GetNearestTransformInList(_source, _targets);
    }

    public static Transform GetNearestTower(Transform _source, int _shortRangeCoeff, int _middleRangeCoeff, int _longRangeCoeff)
    {
        Transform _result = _source;
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

        if (_result == _source) return null;
        return _result;
    }

    public static Transform GetNearestTowerInRange(Transform _source, float _rangeMax, float _rangeMin = 0f)
    {
        List<Transform> _targets = new List<Transform>();
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
                        if (_hit.collider.gameObject == c.gameObject) _targets.Add(c.transform);
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
                            if (_hit.collider.gameObject == c.gameObject) _targets.Remove(c.transform);
                        }
                    }
                }
            }
        }
        return GetNearestTransformInList(_source, _targets);
    }

    private static Transform GetNearestTransformInList(Transform _source, List<Transform> _transforms)
    {
        if (_transforms.Count == 0) return null;

        Transform _result = _source;
        float _distance = Mathf.Infinity;
        foreach(Transform t in _transforms)
        {
            if ((t.position - _source.position).magnitude < _distance)
            {
                _result = t;
                _distance = (t.position - _source.position).magnitude;
            }
        }
            return _result;
    }

    public static bool IsTowerShootable(Transform _source, Transform _tower, float _rangeMax, float _rangeMin = 0f)
    {
        float _distance = (_tower.position - _source.position).magnitude;
        if ( _distance <= _rangeMax && _distance > _rangeMin)
        {
            Ray _ray = new Ray(_source.position, (_tower.position - _source.position).normalized);
            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Buildings", "Terrain" })))
            {
                if (_hit.transform == _tower) return true;
            }
        }
        return false;
    }
    

}
