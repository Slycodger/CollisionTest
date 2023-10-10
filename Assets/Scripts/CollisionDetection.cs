using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using static UnityEditor.Rendering.CameraUI;

public class CollisionDetection : Equations
{
    public float CollidingCushion;
    public float TickSpeed;
    public CollisionData CollidingTime(Vector3 Position, Vector3 Normals, Vector3 Velocity, float Constant, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        CollisionData Output = new CollisionData();
        float Distance = GetDistance(Normals, Position, Constant);
        float T0 = (1 - Distance) / DotProduct(Normalize(Normals), Velocity);
        float TimeAtCollision = T0;
        Vector3 IntersectionPoint = Position - Normalize(Normals) + (T0 * Velocity);
        if (!CheckInsideTri(IntersectionPoint, point1, point2, point3))
        {
            float Smallest;
            _ Vert = CheckVertexTri(Velocity, Position, new Vector3[] { point1, point2, point3 });
            _ Edge = CheckEdgeTri(Position, Velocity, new Vector3[] { point1, point2, point3 });
            if (Vert.time <= Edge.time)
            {
                Smallest = Vert.time;
                IntersectionPoint = Vert.intersectionPoint;
            }
            else
            {
                Smallest = Edge.time;
                IntersectionPoint = Edge.intersectionPoint;
            }
            TimeAtCollision = Smallest;
            Distance = Smallest * Magnitude(Velocity);
        }
        Output.Distance = Distance;
        Output.TimeAtCollision = TimeAtCollision;
        Output.Velocity = Velocity;
        Output.IntersectionPoint = IntersectionPoint;
        Output.PlaneNormals = Normals;
        return Output;
    }

    public bool CheckInsideTri(Vector3 IntersectionPoint, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        bool InsideCollision = false;
        float AOfCombTris;
        float TriangleArea = GetAreaOfTriangle(point1, point2, point3);
        AOfCombTris = GetAreaOfTriangle(IntersectionPoint, point1, point2) + GetAreaOfTriangle(IntersectionPoint, point2, point3) + GetAreaOfTriangle(IntersectionPoint, point1, point3);
        if (AOfCombTris + CollidingCushion >= TriangleArea && AOfCombTris - CollidingCushion <= TriangleArea)
        {
            InsideCollision = true;
        }
        return InsideCollision;
    }
    public _ CheckVertexTri(Vector3 Velocity, Vector3 BallPos, Vector3[] Vertices)
    {
        _ output = new _();
        float Smaller = Mathf.Infinity;
        for (int i = 0; i < Vertices.Count(); i++)
        {
            float Smallest = 0;
            float[] temp = new float[2];
            temp = QuadraticEquation(DotProduct(Velocity, Velocity), 2 * DotProduct(Velocity, BallPos - Vertices[i]), Mathf.Pow(Magnitude(Vertices[i] - BallPos), 2) - 1);
            if (temp[0] <= temp[1])
            {
                Smallest = temp[0];
            }
            else
            {
                Smallest = temp[1];
            }
            if(Smallest <= Smaller)
            {
                Smaller = Smallest;
                output.intersectionPoint = Vertices[i];
            }
        }
        output.time = Smaller;
        return output;
    }
    public _ CheckEdgeTri(Vector3 BallPos, Vector3 Velocity, Vector3[] Verts)
    {
        _ output = new _();
        float Smallest = Mathf.Infinity;
        Vector3[] Edges = new Vector3[3];
        Edges[0] = Verts[1] - Verts[0];
        Edges[1] = Verts[2] - Verts[1];
        Edges[2] = Verts[0] - Verts[2];
        for (int i = 0; i < Edges.Count(); i++)
        {
            Vector3 PointToVert = Verts[i] - BallPos;
            float[] temp = new float[2];
            float tempSmallest;
            temp = QuadraticEquation(Mathf.Pow(Magnitude(Edges[i]), 2) * -Mathf.Pow(Magnitude(Velocity), 2) + Mathf.Pow(DotProduct(Edges[i], Velocity), 2), Mathf.Pow(Magnitude(Edges[i]), 2) * 2 * DotProduct(Velocity, PointToVert) - 2 * DotProduct(Edges[i], Velocity) * DotProduct(Edges[i], PointToVert), Mathf.Pow(Magnitude(Edges[i]), 2) * (1 - Mathf.Pow(Magnitude(PointToVert), 2)) + Mathf.Pow(DotProduct(Edges[i], PointToVert), 2));
            if (temp[0] <= temp[1])
            {
                tempSmallest = temp[0];
            }
            else
            {
                tempSmallest = temp[1];
            }
            float F0 = ((DotProduct(Edges[i], Velocity) * tempSmallest) - DotProduct(Edges[i], PointToVert)) / Mathf.Pow(Magnitude(Edges[i]), 2);
            if (F0 <= 1 && F0 > 0)
            {   
                if(tempSmallest <= Smallest)
                {
                    Smallest = tempSmallest;
                    output.intersectionPoint = Verts[i] + (F0 * Edges[i]);
                }
            }
       
        }
        output.time = Smallest;
        return output;
    }


    public float GetAreaOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float Output = 0;

        float s1 = Mathf.Sqrt(Mathf.Pow(p1.x - p2.x, 2) + Mathf.Pow(p1.y - p2.y, 2) + Mathf.Pow(p1.z - p2.z, 2));
        float s2 = Mathf.Sqrt(Mathf.Pow(p1.x - p3.x, 2) + Mathf.Pow(p1.y - p3.y, 2) + Mathf.Pow(p1.z - p3.z, 2));
        float s3 = Mathf.Sqrt(Mathf.Pow(p2.x - p3.x, 2) + Mathf.Pow(p2.y - p3.y, 2) + Mathf.Pow(p2.z - p3.z, 2));
        float SemiPer = (s1 + s2 + s3) / 2;

        Output = Mathf.Sqrt(SemiPer * ((SemiPer - s1) * (SemiPer - s2) * (SemiPer - s3)));
        return Mathf.Abs(Output);
    }
    public float GetDistance(Vector3 Normals, Vector3  ComparePoint, float constant)
    {
        return (DotProduct(Normals, ComparePoint) - constant) / Magnitude(Normals);
    }
    public class _
    {
        public float time = Mathf.Infinity;
        public Vector3 intersectionPoint;
    }
}
