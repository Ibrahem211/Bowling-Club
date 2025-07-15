using System.Collections.Generic;
using UnityEngine;

public static class GJK
{
    private struct Simplex
    {
        public Vector3[] points;
        public int count;

        public Simplex(int capacity)
        {
            points = new Vector3[capacity];
            count = 0;
        }

        // تعديل: إدراج العنصر الجديد في بداية المصفوفة
        public void Add(Vector3 point)
        {
            if (count < points.Length)
            {
                for (int i = count; i > 0; i--)
                {
                    points[i] = points[i - 1];
                }
                points[0] = point;
                count++;
            }
        }

        public Vector3 this[int index] => points[index];

        public void RemoveAt(int index)
        {
            for (int i = index; i < count - 1; i++)
                points[i] = points[i + 1];
            count--;
        }

        // جديد: تعيين نقاط جديدة مباشرة
        public void Set(params Vector3[] newPoints)
        {
            count = newPoints.Length;
            for (int i = 0; i < count; i++)
                points[i] = newPoints[i];
        }
    }

    public static bool CheckCollision(List<MassPoint> shapeA, List<MassPoint> shapeB)
    {
        if (shapeA == null || shapeB == null || shapeA.Count == 0 || shapeB.Count == 0)
            return false;

        Vector3 direction = FindInitialDirection(shapeA, shapeB);
        if (direction == Vector3.zero)
            direction = Vector3.right;

        Simplex simplex = new Simplex(4);
        Vector3 support = Support(shapeA, shapeB, direction);
        simplex.Add(support);
        direction = -support;

        int maxIterations = 20;

        for (int i = 0; i < maxIterations; i++)
        {
            support = Support(shapeA, shapeB, direction);

            if (Vector3.Dot(support, direction) < 0)
                return false;

            simplex.Add(support);

            if (UpdateSimplex(ref simplex, ref direction))
                return true;
        }

        return false;
    }

    private static Vector3 FindInitialDirection(List<MassPoint> shapeA, List<MassPoint> shapeB)
    {
        Vector3 centerA = CalculateCenter(shapeA);
        Vector3 centerB = CalculateCenter(shapeB);
        return centerB - centerA;
    }

    private static Vector3 CalculateCenter(List<MassPoint> points)
    {
        Vector3 center = Vector3.zero;
        foreach (var p in points)
            center += p.position;
        return center / points.Count;
    }

    private static Vector3 Support(List<MassPoint> shapeA, List<MassPoint> shapeB, Vector3 direction)
    {
        return GetFarthestPoint(shapeA, direction) - GetFarthestPoint(shapeB, -direction);
    }

    private static Vector3 GetFarthestPoint(List<MassPoint> points, Vector3 direction)
    {
        float maxDot = float.NegativeInfinity;
        Vector3 farthest = Vector3.zero;

        foreach (var p in points)
        {
            float dot = Vector3.Dot(p.position, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                farthest = p.position;
            }
        }

        return farthest;
    }

    private static bool UpdateSimplex(ref Simplex simplex, ref Vector3 direction)
    {
        switch (simplex.count)
        {
            case 2: return LineCase(ref simplex, ref direction);
            case 3: return TriangleCase(ref simplex, ref direction);
            case 4: return TetrahedronCase(ref simplex, ref direction);
            default: return false;
        }
    }

    private static bool LineCase(ref Simplex simplex, ref Vector3 direction)
    {
        Vector3 a = simplex[0];
        Vector3 b = simplex[1];
        Vector3 ab = b - a;
        Vector3 ao = -a;

        if (Vector3.Dot(ab, ao) > 0)
        {
            direction = Vector3.Cross(Vector3.Cross(ab, ao), ab);
        }
        else
        {
            simplex.Set(a);
            direction = ao;
        }

        return false;
    }

    private static bool TriangleCase(ref Simplex simplex, ref Vector3 direction)
    {
        Vector3 a = simplex[0];
        Vector3 b = simplex[1];
        Vector3 c = simplex[2];
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ao = -a;
        Vector3 abc = Vector3.Cross(ab, ac);

        if (Vector3.Dot(Vector3.Cross(abc, ac), ao) > 0)
        {
            if (Vector3.Dot(ac, ao) > 0)
            {
                simplex.Set(a, c);
                direction = Vector3.Cross(Vector3.Cross(ac, ao), ac);
            }
            else
            {
                return LineCase(ref simplex, ref direction);
            }
        }
        else
        {
            if (Vector3.Dot(Vector3.Cross(ab, abc), ao) > 0)
            {
                return LineCase(ref simplex, ref direction);
            }
            else
            {
                if (Vector3.Dot(abc, ao) > 0)
                {
                    simplex.Set(a, b, c);
                    direction = abc;
                }
                else
                {
                    simplex.Set(a, c, b);
                    direction = -abc;
                }
            }
        }

        return false;
    }

    private static bool TetrahedronCase(ref Simplex simplex, ref Vector3 direction)
    {
        Vector3 a = simplex[0];
        Vector3 b = simplex[1];
        Vector3 c = simplex[2];
        Vector3 d = simplex[3];
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ad = d - a;
        Vector3 ao = -a;

        Vector3 abc = Vector3.Cross(ab, ac);
        Vector3 acd = Vector3.Cross(ac, ad);
        Vector3 adb = Vector3.Cross(ad, ab);

        if (Vector3.Dot(abc, ao) > 0)
        {
            simplex.Set(a, b, c);
            direction = abc;
            return false;
        }

        if (Vector3.Dot(acd, ao) > 0)
        {
            simplex.Set(a, c, d);
            direction = acd;
            return false;
        }

        if (Vector3.Dot(adb, ao) > 0)
        {
            simplex.Set(a, d, b);
            direction = adb;
            return false;
        }

        return true;
    }
}
