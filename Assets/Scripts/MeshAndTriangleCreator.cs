using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeshAndTriangleCreator : Equations
{
    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;
    public Material MeshMaterial;
    public Material TriangleMaterial;
    [Space(15)]
    public Vector3 Normals;
    public float Constant;
    
    public void Start()
    {
        
    }
    public void Update()
    {
        
    }

    public Mesh CreateMesh(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        Normals = GetNormals(vec1,vec2,vec3);
        Constant = GetConstant(Normals,vec1);
        
        Mesh Output = new Mesh();

        Output.vertices = new Vector3[] 
        {
            vec1,vec2,vec3
        };
        Output.triangles = new int[]
        {
            0,1,2
        };
        GameObject Triangle = new GameObject("Triangle", typeof(MeshFilter), typeof(MeshRenderer));
        Triangle.transform.position = Vector3.zero;
        Triangle.GetComponent<MeshFilter>().mesh = Output;
        Triangle.GetComponent<MeshRenderer>().material = TriangleMaterial;
        return Output;
    }

#if UNITY_EDITOR
[CustomEditor(typeof(MeshAndTriangleCreator))]
    public class ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MeshAndTriangleCreator Mesh = (MeshAndTriangleCreator)target;
            if (GUILayout.Button("Create Mesh"))
            {
                Mesh mesh = Mesh.CreateMesh(Mesh.point1, Mesh.point2, Mesh.point3);

                GameObject Triangle = new GameObject("Triangle", typeof(MeshFilter), typeof(MeshRenderer));
                Triangle.transform.position = Vector3.zero;
                Triangle.GetComponent<MeshFilter>().mesh = mesh;
                Triangle.GetComponent<MeshRenderer>().material = Mesh.TriangleMaterial;
            }
        }
    }
#endif
}
