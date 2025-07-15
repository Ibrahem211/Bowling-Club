/*using System.Collections.Generic;
using UnityEngine;

public static class EPA
{
    private struct Face
    {
        public Vector3 a, b, c;
        public Vector3 normal;
        public float distance;

        public Face(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            distance = Vector3.Dot(normal, a);
            if (distance < 0)
            {
                // تأكد أن الوجه يواجه للخارج (الوجه يكون موجهاً بعيداً عن الأصل)
                normal = -normal;
                distance = -distance;
                Vector3 temp = b;
                b = c;
                c = temp;
            }
        }
    }

    public struct EPAResult
    {
        public bool success;
        public Vector3 normal;
        public float depth;
        public Vector3 contactPoint;
    }

    private const int MAX_ITERATIONS = 50;
    private const float TOLERANCE = 0.0001f;

    // طريقة بدء الخوارزمية مع الـ simplex الرباعي من GJK
    public static EPAResult ExpandPolytope(GJK.Simplex simplex, List<MassPoint> shapeA, List<MassPoint> shapeB)
    {
        EPAResult result = new EPAResult { success = false };

        // بناء الـ polytope من الـ simplex الرباعي (4 نقاط)
        List<Face> faces = new List<Face>
        {
            new Face(simplex[0], simplex[1], simplex[2]),
            new Face(simplex[0], simplex[3], simplex[1]),
            new Face(simplex[0], simplex[2], simplex[3]),
            new Face(simplex[1], simplex[3], simplex[2])
        };

        for (int iter = 0; iter < MAX_ITERATIONS; iter++)
        {
            // 1. إيجاد الوجه الأقرب للأصل (0,0,0)
            Face closestFace = faces[0];
            float minDistance = faces[0].distance;

            foreach (var face in faces)
            {
                if (face.distance < minDistance)
                {
                    minDistance = face.distance;
                    closestFace = face;
                }
            }

            // 2. نقطة دعم جديدة في اتجاه وجه أقرب
            Vector3 support = Support(shapeA, shapeB, closestFace.normal);
            float distance = Vector3.Dot(closestFace.normal, support);

            // 3. تحقق من التقارب (إذا كانت النقطة الجديدة ليست أبعد بكثير)
            if (distance - minDistance < TOLERANCE)
            {
                // نعيد النتائج: العمق والاتجاه ونقطة التماس (تقريب نقطة التماس على الوجه الأقرب)
                result.success = true;
                result.normal = closestFace.normal;
                result.depth = distance;

                // نقطة التماس تقريباً عند إسقاط الأصل على الوجه
                result.contactPoint = support - closestFace.normal * distance;
                return result;
            }

            // 4. توسيع polytope: إزالة الوجوه التي تواجه نقطة الدعم الجديدة
            List<(int, Face)> toRemove = new List<(int, Face)>();
            List<(Vector3, Vector3)> edges = new List<(Vector3, Vector3)>();

            for (int i = 0; i < faces.Count; i++)
            {
                if (Vector3.Dot(faces[i].normal, support - faces[i].a) > 0)
                {
                    toRemove.Add((i, faces[i]));
                }
            }

            // 5. إيجاد الحواف الحدودية للوجوه التي تم حذفها
            for (int i = 0; i < toRemove.Count; i++)
            {
                Face f1 = toRemove[i].Item2;
                for (int j = i + 1; j < toRemove.Count; j++)
                {
                    Face f2 = toRemove[j].Item2;

                    TryAddEdge(edges, f1.a, f1.b, f2);
                    TryAddEdge(edges, f1.b, f1.c, f2);
                    TryAddEdge(edges, f1.c, f1.a, f2);
                }
            }

            // 6. حذف الوجوه التي تواجه نقطة الدعم
            for (int i = toRemove.Count - 1; i >= 0; i--)
            {
                faces.RemoveAt(toRemove[i].Item1);
            }

            // 7. إضافة وجوه جديدة مع نقطة الدعم الجديدة
            foreach (var edge in edges)
            {
                faces.Add(new Face(edge.Item1, edge.Item2, support));
            }
        }

        // إذا وصلنا لهنا، لم ينجح التوسع
        return result;
    }

    // دالة مساعدة لإدارة الحواف
    private static void TryAddEdge(List<(Vector3, Vector3)> edges, Vector3 a, Vector3 b, Face otherFace)
    {
        // إذا الحافة موجودة بالعكس، نحذفها (لأنها داخلية)
        for (int i = 0; i < edges.Count; i++)
        {
            if ((edges[i].Item1 == b && edges[i].Item2 == a))
            {
                edges.RemoveAt(i);
                return;
            }
        }

        // الحافة غير موجودة، نضيفها
        edges.Add((a, b));
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
}*/
