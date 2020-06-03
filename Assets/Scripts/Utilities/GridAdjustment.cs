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

        return new Vector3(_x, 0, _z);
    }
}
