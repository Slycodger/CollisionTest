using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Collision : MonoBehaviour
{
    [Header("Variables")]
    [Space(15)]
    public float speed;
    public float CollisionCushionRange;
    public float PlaySpeed;
    public GameObject ball;
    public GameObject cam1;
    public GameObject cam2;
    public GameObject TinyCube;
    public TextMeshProUGUI DistanceText;
    public TextMeshProUGUI VelocityText;
    public TextMeshProUGUI T0Text;
    public TextMeshProUGUI T1Text;
    public TextMeshProUGUI IntersectionPointText;
    public Material TriangleMaterial;
    public Vector3 Velocity;
    [Header("Points")]
    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;
    public Vector3 compareDistance;
    [Space(15)]
    [Header("Plane variables")]
    public Vector3 normals;
    public float constant;
    public Material Material;
    public float Scale;
    [Space(15)]
    [Header("Outputs")]
    public float Distance;
    public float T0;
    public float T1;
    private Vector3 BallPos;
    public Vector3 IntersectionPoint;
    public float TriangleArea;
    public Mesh TriangleMesh;
    bool pressed;
    bool playingThrough = false;
    public float playDuration;
    public float TVertex;
    public float TEdge;
    private Vector3[] Vertices = new Vector3[3];
    private Vector3[] Edges = new Vector3[3];
    private bool VertexCollision;
    private bool EdgeCollision;
    public void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        TriangleArea = GetAreaOfTriangle(point1, point2, point3);
        Vertices[0] = point1;
        Vertices[1] = point2;
        Vertices[2] = point3;
        Edges[0] = point3 - point1;
        Edges[1] = point2 - point1;
        Edges[2] = point2 - point3;
    }
    public void Update()
    {
        if (playingThrough)
        {
            if(playDuration <= 1 && playDuration >= -1) 
            {
                ball.transform.position += Velocity * PlaySpeed;
                playDuration += PlaySpeed;
            }
            else
            {
                playDuration = 0;
                playingThrough = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playingThrough = true;
                pressed = true; 
                PlaySpeed = Mathf.Abs(PlaySpeed);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playingThrough = true;
                pressed = true;
                PlaySpeed = Mathf.Abs(PlaySpeed) * -1;
            }
            if (!pressed)
            {   
                ball.transform.position += new Vector3(-Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0);
                BallPos = ball.transform.position;
                T0 = (1 - Distance) / DotProduct(-Normalize(normals), Velocity);
                T1 = (-1 - Distance) / DotProduct(-Normalize(normals), Velocity);
                IntersectionPoint = BallPos + Normalize(normals) + (T0 * Velocity);
                if (IntersectionPoint.x >= 0 || IntersectionPoint.x <= 0)
                {
                    TinyCube.transform.position = IntersectionPoint;
                }
                if (Input.GetKey(KeyCode.I))
                {
                    Velocity.z -= Time.deltaTime * speed;
                }
                if (Input.GetKey(KeyCode.K))
                {
                    Velocity.z += Time.deltaTime * speed;
                }
                if (Input.GetKey(KeyCode.J))
                {
                    Velocity.x += Time.deltaTime * speed;
                }
                if (Input.GetKey(KeyCode.L))
                {
                    Velocity.x -= Time.deltaTime * speed;
                }
                if (Input.GetKey(KeyCode.PageUp))
                {
                    Velocity.y += Time.deltaTime * speed;
                }
                if (Input.GetKey(KeyCode.PageDown))
                {
                    Velocity.y -= Time.deltaTime * speed;
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ball.transform.position = BallPos + Velocity;
                pressed = true;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                ball.transform.position = BallPos + Velocity * T0;
                pressed = true;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ball.transform.position = BallPos + Velocity * T1;
                pressed = true;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                ball.transform.position = BallPos;
                pressed = false;
            }
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Velocity = Vector3.zero;
                pressed = false;
            }

            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                if (cam1.activeInHierarchy)
                {
                    cam1.SetActive(false);
                    cam2.SetActive(true);
                }
                else
                {
                    cam1.SetActive(true);
                    cam2.SetActive(false);
                }
            }
            compareDistance = ball.transform.position;
            GetDistance();

            DistanceText.text = Distance.ToString();
            VelocityText.text = Velocity.ToString();
            float AOfCombTris;
            AOfCombTris = GetAreaOfTriangle(IntersectionPoint, point1, point2) + GetAreaOfTriangle(IntersectionPoint, point2, point3) + GetAreaOfTriangle(IntersectionPoint, point1, point3);
            if((T0 <= 1 && T0 > 0) || (T1 <= 1 && T1 >0)) 
            {
                if (AOfCombTris >= TriangleArea - CollisionCushionRange && AOfCombTris <= TriangleArea + CollisionCushionRange)
                {
                    TriangleMaterial.color = Color.red;
                }
                else
                {
                    VertexCollision = false;
                    EdgeCollision = false;
                    for(int i = 0; i < 3; i++)
                    {
                        float[] temp = new float[2];
                        temp = QuadraticEquation(DotProduct(Velocity, Velocity), 2 * DotProduct(Velocity, BallPos - Vertices[i]), Mathf.Pow(Magnitude(Vertices[i] - BallPos), 2) - 1);
                        float Smaller = 0;
                        if (temp[0] <= temp[1])
                        {
                            Smaller = temp[0];
                        }
                        else
                        {
                            Smaller = temp[1];
                        }
                        if(Smaller <= 1 && Smaller > 0)
                        {
                            TVertex = Smaller; 
                            VertexCollision = true;
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 PointToVert = new Vector3();
                        if (i != 2)
                        {
                            PointToVert = point1 - BallPos;
                        }
                        else
                        {
                            PointToVert = point3 - BallPos;
                        }
                        float[] temp = new float[2];
                        temp = QuadraticEquation(Mathf.Pow(Magnitude(Edges[i]), 2) * -Mathf.Pow(Magnitude(Velocity), 2) + Mathf.Pow(DotProduct(Edges[i], Velocity), 2), Mathf.Pow(Magnitude(Edges[i]), 2) * 2 * DotProduct(Velocity, PointToVert) - 2 * DotProduct(Edges[i], Velocity) * DotProduct(Edges[i], PointToVert), Mathf.Pow(Magnitude(Edges[i]), 2) * (1 - Mathf.Pow(Magnitude(PointToVert), 2)) + Mathf.Pow(DotProduct(Edges[i], PointToVert), 2));
                        float Smallest;
                        if (temp[0] <= temp[1])
                        {
                            Smallest = temp[0];
                        }
                        else
                        {
                            Smallest = temp[1];
                        }
                        float F0 = ((DotProduct(Edges[i], Velocity) * Smallest) - DotProduct(Edges[i], PointToVert)) / Mathf.Pow(Magnitude(Edges[i]), 2);
                        if((F0 <= 1 && F0 > 0) && (Smallest <= 1 && Smallest > 0))
                        {
                            TEdge = Smallest;
                            EdgeCollision = true;
                        }
                    }
                    if (VertexCollision || EdgeCollision)
                    {
                        
                        TriangleMaterial.color = Color.red;
                    }
                    else
                    {
                        TriangleMaterial.color = Color.blue;
                    }
                }
            }
            else
            {
                TriangleMaterial.color = Color.blue;
            }
            T0Text.text = T0.ToString();
            T1Text.text = T1.ToString();
            IntersectionPointText.text = IntersectionPoint.ToString();
        }
    }
    public Mesh CreateMesh(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Mesh Output = new Mesh();
        List<Vector2> uv = new List<Vector2>();
        Vector3[] vertices =
        {
            p1,p2,p3
        };
        int[] indices =
        {
            0, 1, 2,
            2, 1, 0
        };
        foreach(Vector3 i in vertices) 
        {
            Vector2 spot = new Vector2(i.x, i.y);
            spot.Normalize();
            uv.Add(spot);
        }


        Output.vertices = vertices;
        Output.uv = uv.ToArray();
        Output.triangles = indices;

        return Output;
    }
    public Vector3 CrossProduct(Vector3 Vec1, Vector3 Vec2) 
    {
        Vector3 Output = new Vector3();

        Output.x = (Vec1.y * Vec2.z) - (Vec2.y * Vec1.z);
        Output.y = -((Vec1.x * Vec2.z) - (Vec2.x * Vec1.z));
        Output.z = (Vec1.x * Vec2.y) - (Vec2.x * Vec1.y);

        return Output;
    }
    public float[] QuadraticEquation(float A, float B, float C)
    {
        float[] Output = new float[2];

        float Sqrt = Mathf.Sqrt(Mathf.Pow(B,2) - 4*A*C);

        Output[0] = (-B + Sqrt) / (2 * A);
        Output[1] = (-B - Sqrt) / (2 * A);

        return Output;
    }
    public float DotProduct(Vector3 Vec1, Vector3 Vec2)
    {
        float Output = 0;

        Output = (Vec1.x * Vec2.x) + (Vec1.y * Vec2.y) + (Vec1.z * Vec2.z); 

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
    public float SumOf(Vector3 Vec) 
    {
        float Output = 0;
        Output = Vec.x + Vec.y + Vec.z;
        return Output;
    }
    public void getNormals()
    {
        Vector3 p1 = (point2 - point1);
        Vector3 p2 = (point3 - point1);
        normals = CrossProduct(p1, p2);
    }
    public void getConstant()
    {
        constant = DotProduct(normals,point1);
    }
    public Vector3 VecAbs(Vector3 Vec) 
    {
        Vector3 Output = new Vector3();

        Output.x = Mathf.Abs(Vec.x);
        Output.y = Mathf.Abs(Vec.y);
        Output.z = Mathf.Abs(Vec.z);

        return Output;
    }
    public void GetDistance()
    {
        Distance = -(DotProduct(normals, compareDistance) - constant) / Magnitude(normals);
    }
    public float GetAreaOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3) 
    {
        float Output = 0;

        float s1 = Mathf.Sqrt(Mathf.Pow(p1.x - p2.x,2) + Mathf.Pow(p1.y - p2.y, 2) + Mathf.Pow(p1.z - p2.z, 2));
        float s2 = Mathf.Sqrt(Mathf.Pow(p1.x - p3.x, 2) + Mathf.Pow(p1.y - p3.y, 2) + Mathf.Pow(p1.z - p3.z, 2));
        float s3 = Mathf.Sqrt(Mathf.Pow(p2.x - p3.x, 2) + Mathf.Pow(p2.y - p3.y, 2) + Mathf.Pow(p2.z - p3.z, 2));
        float SemiPer = (s1 + s2 + s3) / 2;

        Output = Mathf.Sqrt(SemiPer * ((SemiPer - s1) * (SemiPer - s2) * (SemiPer - s3)));
        return Mathf.Abs(Output);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Collision))]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Collision collision = (Collision)target;

        if(GUILayout.Button("Get Values"))
        {
            collision.getNormals();
            collision.getConstant();
        }
        if (GUILayout.Button("Get Distance"))
        {
            collision.GetDistance();
        }
        if (GUILayout.Button("Create Mesh"))
        {
            collision.TriangleMesh = collision.CreateMesh(new Vector3(collision.constant / collision.normals.x, 0, 0), new Vector3(0, collision.constant / collision.normals.y, 0), new Vector3(0, 0, collision.constant / collision.normals.z));

            collision.getNormals();
            collision.getConstant();

            GameObject MeshCreated = new GameObject("The great object", typeof(MeshFilter), typeof(MeshRenderer));
            MeshCreated.GetComponent<MeshFilter>().mesh = collision.TriangleMesh;
            MeshCreated.GetComponent<MeshRenderer>().material = collision.Material;

            GameObject Parent = new GameObject("Parent");
            Parent.transform.position = (collision.TriangleMesh.vertices[0] + collision.TriangleMesh.vertices[1] + collision.TriangleMesh.vertices[2]) / 3;
            MeshCreated.transform.parent = Parent.transform;
            Parent.transform.localScale *= collision.Scale;
            GameObject Triangle = new GameObject("Triangle", typeof(MeshFilter), typeof(MeshRenderer));

            Mesh mesh = collision.CreateMesh(collision.point1, collision.point2, collision.point3);

            Triangle.GetComponent<MeshFilter>().mesh = mesh;
            Triangle.GetComponent<MeshRenderer>().material = collision.TriangleMaterial;
        }
    }
}
#endif