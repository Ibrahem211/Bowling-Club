using System.Collections.Generic;
using UnityEngine;

public class AABB
{
    public Vector3 min;
    public Vector3 max;
    public Vector3 center;
    public Vector3 extents;

    public AABB(List<MassPoint> points)
    {
        if (points == null || points.Count == 0)
        {
            min = max = center = Vector3.zero;
            extents = Vector3.zero;
            return;
        }

        min = max = points[0].position;

        foreach (var p in points)
        {
            min = Vector3.Min(min, p.position);
            max = Vector3.Max(max, p.position);
        }

        center = (min + max) * 0.5f;
        extents = (max - min) * 0.5f;
    }

    public bool Intersects(AABB other)
    {
        return (min.x <= other.max.x && max.x >= other.min.x) &&
               (min.y <= other.max.y && max.y >= other.min.y) &&
               (min.z <= other.max.z && max.z >= other.min.z);
    }
}
