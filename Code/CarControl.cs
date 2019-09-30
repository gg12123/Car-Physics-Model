using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    // Maybe put these on a suspension parent class or something
    [SerializeField]
    private float m_SuspensionStiffness = 1.0f;
    [SerializeField]
    private float m_SuspensionDamper = 1.0f;

    private SteeringControl m_Steer;
    private FrictionApplier m_Friction;

    private Wheel[] m_Wheels;
    private Suspension[] m_Suspensions;

    public float SuspensionStiffness {  get { return m_SuspensionStiffness; } }
    public float SuspensionDamper { get { return m_SuspensionDamper; } }

    void Awake()
    {
        m_Steer = GetComponent<SteeringControl>();
        m_Friction = GetComponent<FrictionApplier>();
        m_Suspensions = GetComponentsInChildren<Suspension>();
        m_Wheels = GetComponentsInChildren<Wheel>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Steer.Execute();
        m_Friction.Execute();

        for (int i = 0; i < m_Suspensions.Length; i++)
            m_Suspensions[i].Execute();

        for (int i = 0; i < m_Wheels.Length; i++)
            m_Wheels[i].Execute();
    }
}
