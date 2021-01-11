using UnityEngine;

/// <summary>
/// GridAdjustment utility class is used to adjust a position to the grid
/// </summary>
public static class GridAdjustment
{
    public static Vector3 GetGridCoordinates(Vector3 _input)
    {
        float _x = Mathf.Floor(_input.x / 10) * 10 + 5;
        float _z = Mathf.Floor(_input.z / 10) * 10 + 5;
        
        float _y = _input.y;
        RaycastHit _hit;
        Ray _ray = new Ray(new Vector3( _x, 100f, _z), -Vector3.up);
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Terrain" })))
        {
            _y = _hit.point.y;
        }

        return new Vector3(_x, _y, _z);
    }

    public static bool IsSameOnGrid(Vector3 _pos1, Vector3 _pos2)
    {
        return (GetGridCoordinates(_pos1)).Equals(GetGridCoordinates(_pos2));
    }
}
