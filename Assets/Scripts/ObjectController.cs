using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
public class ObjectController : CollisionDetection
{
    [NonReorderable]
    public List<collisionObject> gameObjects;
    public MeshAndTriangleCreator Mesh;
    volatile bool running = false;
    private void Start()
    {
        //StartCoroutine(TickTimer(TickSpeed));
        Thread thread = new Thread(new ThreadStart(Test));
        running = true;
        thread.Start();
        thread.Join();
    }
    public void Test()
    {
        while (true)
        {
            Thread.Sleep(100);
            Debug.Log("test");
        }
    }
    //Makes ticks happen a set amount of times per second
    public IEnumerator TickTimer(float TickSpeed)
    {
        bool Colliders = false;
        foreach (GameObject game in FindObjectsOfType<GameObject>())
        {   
            gameObjects.Add(new collisionObject());
            gameObjects[gameObjects.Count - 1].gameObject = game;
        }
        for (int i = 0; i < gameObjects.Count(); i++)
        {
            GameObject Object = gameObjects[i].gameObject;
            if (!Object.activeInHierarchy || Object.GetComponent<Object>() as Object == null || !Object.GetComponent<Object>().isActiveAndEnabled)
            {
                gameObjects.RemoveAt(i);
                i--;
                continue;
            }
            Mesh ObjectMesh = Object.GetComponent<MeshFilter>().mesh;
            if (Object.GetComponent<Object>().Collider)
            {
                for (int Tri = 0; Tri < ObjectMesh.triangles.Count(); Tri+=3)
                {
                    gameObjects[i].Triangles.Add(new Triangle());
                    Vector3[] Vertices = GetVertices(Tri, ObjectMesh, gameObjects[i].gameObject.transform);
                    Vector3 Normals = GetNormals(Vertices[0], Vertices[1], Vertices[2]);
                    float Constant = GetConstant(Normals, Vertices[0]);
                    gameObjects[i].Triangles[Tri / 3].Vertices = Vertices;
                    gameObjects[i].Triangles[Tri / 3].Normals = Normals;
                    gameObjects[i].Triangles[Tri / 3].Constant = Constant;
                    //Mesh.CreateMesh(Vertices[0], Vertices[1], Vertices[2]);
                }
                Colliders = true;
            }
        }
        while (true)
        {
            yield return new WaitForSeconds(TickSpeed);
            //Put everything you want to happen every tick here
            foreach (collisionObject gameObject in gameObjects)
            {   
                Object ObjectCollision = gameObject.gameObject.GetComponent<Object>();
                if (ObjectCollision.Collider)
                {
                    continue;
                }
                if (ObjectCollision.Velocity == Vector3.zero)
                {
                    continue;
                }
                if (Colliders)
                {
                    ObjectCollision.Checked = false;
                    ObjectCollision.transform.position = MoveObject(ObjectCollision.transform.position, ObjectCollision.Velocity, gameObjects.ToArray(), gameObject);
                    continue;
                }
                ObjectCollision.transform.position += ObjectCollision.Velocity * TickSpeed;
            }
        }
    }
    public Vector3 MoveObject(Vector3 position, Vector3 velocity, collisionObject[] Colliders, collisionObject Object)
    {
        CollisionData Smallest = new CollisionData();
        foreach (collisionObject Collider in Colliders)
        {   
            if(Collider == Object)
            {
                continue;
            }
            
            for (int i = 0; i < Collider.Triangles.Count(); i++)
            {
                Vector3[] Vertices = Collider.Triangles[i].Vertices;
                Vector3 Normals = Collider.Triangles[i].Normals;
                float Constant = Collider.Triangles[i].Constant;
                CollisionData t = CollidingTime(position, Normals, velocity, Constant, Vertices[0], Vertices[1], Vertices[2]);
                if (t.TimeAtCollision < Smallest.TimeAtCollision && !Negative(t.TimeAtCollision))
                {
                    Smallest = t;
                }
            }
        }
        Smallest.Velocity = velocity;
        return TranslationWithCollision(Smallest, position, Colliders, Object);
    }
    public Vector3 TranslationWithCollision(CollisionData collisionData, Vector3 Position, collisionObject[] Colliders, collisionObject collisionObject)
    {
        Object Object = collisionObject.gameObject.GetComponent<Object>();
        float TimeAtCollision = collisionData.TimeAtCollision;
        Vector3 Velocity = collisionData.Velocity;
        if ((TimeAtCollision <= TickSpeed && !Negative(TimeAtCollision)) || TimeAtCollision == 0 )
        {
            Vector3 IntersectionPoint = collisionData.IntersectionPoint;
            Vector3 PlaneNormals = collisionData.PlaneNormals;
            Vector3 PointOnPlane = Position + (Velocity * TimeAtCollision);
            Vector3 SlidingPlaneNormals = PointOnPlane - IntersectionPoint;
            PointOnPlane += Normalize(PlaneNormals) * CollidingCushion;
            Vector3 newVelocity = (Velocity - Projection(Velocity, SlidingPlaneNormals)) * (1 + CollidingCushion);
            if (!Object.Checked)
            {
                Object.Checked = true;
                return MoveObject(PointOnPlane, newVelocity, Colliders, collisionObject);
            }
            return (Position + PointOnPlane) / 2;
        }
        return Position + (Velocity * TickSpeed);
    }
    public Vector3[] GetVertices(int Triangle, Mesh mesh, Transform Pos)
    {
        Matrix4x4 localToWorld = Pos.localToWorldMatrix;
        Vector3[] output = new Vector3[3];
        output[0] = localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[Triangle]]);
        output[1] = localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[Triangle + 1]]);
        output[2] = localToWorld.MultiplyPoint3x4(mesh.vertices[mesh.triangles[Triangle + 2]]);
        return output;
    }
    [System.Serializable]
    public class collisionObject
    {
        public GameObject gameObject;
        [NonReorderable]
        public List<Triangle> Triangles = new List<Triangle>();
    }
    [System.Serializable]
    public class Triangle
    {
        [NonReorderable]
        public Vector3[] Vertices = new Vector3[3];
        public Vector3 Normals;
        public float Constant;
    }
}
