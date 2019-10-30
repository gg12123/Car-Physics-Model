using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    private Wheel m_Wheel;
    private float m_UnCompressedLength;
    private CarControl m_Car;
    private Rigidbody m_Body;

    public float CurrentSpringLength { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        m_Wheel = GetComponentInChildren<Wheel>();
        m_Car = GetComponentInParent<CarControl>();
        m_Body = GetComponentInParent<Rigidbody>();
        m_UnCompressedLength = Mathf.Abs(Vector3.Dot(transform.position - m_Wheel.transform.position, transform.up)) - m_Wheel.CalculateRadius();
        CurrentSpringLength = m_UnCompressedLength;
    }

    // Update is called once per frame
    public void Execute()
    {
        var currPos = transform.position;
        var currUp = transform.up;

        RaycastHit hit;
        if (Physics.Raycast(new Ray(currPos, -currUp), out hit))
        {
            var prevCompression = m_UnCompressedLength - CurrentSpringLength;

            CurrentSpringLength = Mathf.Min(hit.distance - 2.0f * m_Wheel.Radius, m_UnCompressedLength);

            if (CurrentSpringLength < 0.0f)
            {
                Debug.LogWarning("Suspension spring length is negative!");
            }

            var compression = m_UnCompressedLength - CurrentSpringLength;
            var compressionDot = (compression - prevCompression) / Time.fixedDeltaTime;

            m_Body.AddForceAtPosition((compression * m_Car.SuspensionStiffness + compressionDot * m_Car.SuspensionDamper) * currUp, currPos);
        }
    }
}
