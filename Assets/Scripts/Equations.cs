using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Equations : MonoBehaviour
{
    public float DotProduct(Vector3 vec1, Vector3 vec2)
    {
        float Output = 0;

        Output = (vec1.x * vec2.x) + (vec1.y * vec2.y) + (vec1.z * vec2.z);

        return Output;
    }
    public Vector3 CrossProduct(Vector3 vec1, Vector3 vec2)
    {
        Vector3 Output = new Vector3();

        Output.x = (vec1.y * vec2.z) - (vec2.y * vec1.z);
        Output.y = -((vec1.x * vec2.z) - (vec2.x * vec1.z));
        Output.z = (vec1.x * vec2.y) - (vec2.x * vec1.y);
        
        return Output;
    }
    public float[] QuadraticEquation(float A, float B, float C)
    {
        float[] Output = new float[2];

        float Sqrt = Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C);

        Output[0] = (-B + Sqrt) / (2 * A);
        Output[1] = (-B - Sqrt) / (2 * A);

        return Output;
    }
    public float Magnitude(Vector3 Vec)
    {
        float Output = 0;
        Output = Mathf.Sqrt(Mathf.Pow(Vec.x, 2) + Mathf.Pow(Vec.y, 2) + Mathf.Pow(Vec.z, 2));
        return Output;
    }
    public Vector3 Normalize(Vector3 Vec)
    {
        Vector3 Output = new Vector3();

        Output = Vec / Magnitude(Vec);

        return Output;
    }
    public bool Negative(float Value)
    {
        if(Mathf.Abs(Value) == Value || Value == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public Vector3 Projection(Vector3 vec1, Vector3 vec2)
    {
        Vector3 output = new Vector3();
        output = DotProduct(vec1, vec2) / DotProduct(vec2,vec2) * vec2;
        return output;
    }
    public Vector3 GetNormals(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Vector3 Output = new Vector3();

        Vector3 vec1 = point3 - point1;
        Vector3 vec2 = point2 - point1;

        Output = -CrossProduct(vec1, vec2);

        return Output;
    }
    public float GetConstant(Vector3 Normals, Vector3 point)
    {
        float Output = 0;

        Output = DotProduct(Normals, point);

        return Output;
    }

    public class CollisionData
    {
        public float TimeAtCollision = Mathf.Infinity;
        public float Distance = Mathf.Infinity;
        public Vector3 Velocity = Vector3.zero;
        public Vector3 IntersectionPoint = Vector3.positiveInfinity;
        public Vector3 PlaneNormals = Vector3.positiveInfinity;
    }
}
