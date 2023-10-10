using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Ball : MonoBehaviour
{
    public Vector3 Velocity;
    public float VelocityRampSpeed;
    public float BallSpeed;
    public TextMeshProUGUI VelocityText;
    [HideInInspector]
    public Vector3 Position;
    public float VelocitySpeed;
    public Vector3 VelPos;
    public Vector3 TempVelocity;
    public Vector3 Gravity;
    public CollisionDetection CD;
    public void Start()
    {
        VelPos = transform.position;
        //StartCoroutine(Timer());
    }
    public void Update()
    {
        CheckInputs();
        transform.position += new Vector3(-Input.GetAxis("Horizontal") * BallSpeed * Time.deltaTime, Input.GetAxis("Jump") * Time.deltaTime * BallSpeed, -Input.GetAxis("Vertical") * Time.deltaTime * BallSpeed);
        VelocityText.text = Velocity.ToString();
    }
/*    public IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(VelocitySpeed);
            VelPos = transform.position;
            transform.position = CD.CollisionPos(transform.position, VelPos, CD.Mesh.Normals, Velocity, CD.Mesh.Constant, VelocitySpeed, CD.Mesh.point1, CD.Mesh.point2, CD.Mesh.point3);
            VelPos = transform.position;
            transform.position = CD.CollisionPos(transform.position, VelPos, CD.Mesh.Normals, Gravity, CD.Mesh.Constant, VelocitySpeed, CD.Mesh.point1, CD.Mesh.point2, CD.Mesh.point3);
        }
    }*/
    public void CheckInputs()
    {
        if (Input.GetKey(KeyCode.I))
        {
            Velocity.z -= VelocityRampSpeed;
        }
        if (Input.GetKey(KeyCode.K))
        {
            Velocity.z += VelocityRampSpeed;
        }
        if (Input.GetKey(KeyCode.J))
        {
            Velocity.x += VelocityRampSpeed;
        }
        if (Input.GetKey(KeyCode.L))
        {
            Velocity.x -= VelocityRampSpeed;
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            Velocity.y += VelocityRampSpeed;
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            Velocity.y -= VelocityRampSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            VelPos = transform.position;
            Velocity = Vector3.zero;
        }
    }
}
