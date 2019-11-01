using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringControl : MonoBehaviour
{
    [SerializeField]
    private float m_MaxSteeringAngle = 60.0f;

    private float m_HalfCarLength;

    private Rigidbody m_Body;

    private void CalculateHalfCarLength()
    {
        var mf = GetComponentInChildren<MeshFilter>();
        var verts = mf.mesh.vertices;

        var f = transform.forward;

        var min = Mathf.Infinity;
        var max = Mathf.NegativeInfinity;

        foreach (var v in verts)
        {
            var vWorld = mf.transform.TransformPoint(v);

            var comp = Vector3.Dot(vWorld, f);

            if (comp > max)
                max = comp;

            if (comp < min)
                min = comp;
        }

        m_HalfCarLength = (max - min) / 2.0f;
    }

    void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
        CalculateHalfCarLength();
    }

    public void Execute()
    {
        var steer = GetSteer();
        var steerAngle = steer * m_MaxSteeringAngle * Mathf.Deg2Rad;

        var sDot = m_Body.velocity.magnitude;
        var thetaDotDemand = (sDot * Mathf.Tan(steerAngle)) / m_HalfCarLength;

        var actual = Vector3.Dot(m_Body.angularVelocity, transform.up);

        var req_acc = (thetaDotDemand - actual) / Time.fixedDeltaTime;

        // TODO - I'm not sure if this is the correct way to use the inertia.
        // How do you get the matrix form?
        m_Body.AddRelativeTorque(0.0f, m_Body.inertiaTensor.y * req_acc, 0.0f);
    }

    protected virtual float GetSteer()
    {
        var mousePos = (Vector2)Input.mousePosition;
        var x = mousePos - new Vector2(Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f);

        return x.x / (Camera.main.pixelWidth / 2.0f);
    }
}
