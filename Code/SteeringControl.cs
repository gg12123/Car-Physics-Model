using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringControl : MonoBehaviour
{
    [SerializeField]
    private float m_AlphaDotMax = 2.0f;

    private Rigidbody m_Body;

    void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
    }

    public void Execute()
    {
        var steer = GetSteer();
        var demand = steer * m_AlphaDotMax;
        var actual = Vector3.Dot(m_Body.angularVelocity, transform.up);

        var req_acc = (demand - actual) / Time.fixedDeltaTime;

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
