using UnityEngine;

public static class LineIntersection
{
    public static bool TryGetIntersection(Vector2 point1, Vector2 dir1, Vector2 point2, Vector2 dir2, out Vector3 intersection)
    {
        intersection = Vector2.zero;

        float _denominator = (dir1.x * dir2.y) - (dir1.y * dir2.x);

        // Check if lines are parallel
        if(Mathf.Approximately(_denominator, 0f))
        {
            return false;
        }

        float _dx = point2.x - point1.x;
        float _dy = point2.y - point1.y;

        float _t = ((_dx * dir2.y) - (_dy * dir2.x)) / _denominator;

        intersection = point1 + (dir1 * _t);
        return true;
    }
}
