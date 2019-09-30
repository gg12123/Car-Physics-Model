using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionOfTravelCalculater : MonoBehaviour
{
    [SerializeField]
    private Transform m_DebugTravelDir;

    [SerializeField]
    private float m_BetaDotDotMax = 100000.0f;

    private RateLimiter m_BetaDotLimiter;
    private float m_Theta;
    private Rigidbody m_Body;
    private Vector3 m_TravelDirLocal;

    public Vector3 TravelDirLocal {  get { return m_TravelDirLocal; } }
    public Vector3 TravelDirWorld {  get { return transform.TransformDirection(m_TravelDirLocal); } }
    public bool IsDrifting { get { return m_Theta != 0.0f; } }

    void Awake()
    {
        m_BetaDotLimiter = new RateLimiter(m_BetaDotDotMax);
        m_Body = GetComponent<Rigidbody>();
        m_Theta = 0.0f;
    }

    private float CalculateThetaDot(float alphaDot)
    {
        m_BetaDotLimiter.Execute(alphaDot, Time.fixedDeltaTime);

        // Constrain beta dot so that it is not opposite in sign
        // to alpha dot and is always less in magnitude.
        if (alphaDot * m_BetaDotLimiter.CurrentValue < 0.0f)
        {
            m_BetaDotLimiter.CurrentValue = 0.0f;
        }
        else if (Mathf.Abs(alphaDot) < Mathf.Abs(m_BetaDotLimiter.CurrentValue))
        {
            m_BetaDotLimiter.CurrentValue = alphaDot;
        }

        // Ensure no inflection whilst drifting
        if ((m_Theta > 0.0f && m_BetaDotLimiter.CurrentValue < 0.0f) ||
            (m_Theta < 0.0f && m_BetaDotLimiter.CurrentValue > 0.0f))
        {
            m_BetaDotLimiter.CurrentValue = 0.0f;
        }

        return alphaDot - m_BetaDotLimiter.CurrentValue;
    }

    public void Execute()
    {
        var alphaDot = Vector3.Dot(m_Body.angularVelocity, transform.up);
        var thetaDot = CalculateThetaDot(alphaDot);

        var prevTheta = m_Theta;
        m_Theta += Time.fixedDeltaTime * thetaDot;

        if (prevTheta * m_Theta < 0.0f && m_BetaDotLimiter.CanReachTarget(alphaDot, Time.fixedDeltaTime))
        {
            m_Theta = 0.0f;
            m_BetaDotLimiter.CurrentValue = alphaDot;
        }

        // Theta is the angle from the direction of travel to the car forward.
        // So -theta is the direction from the car forward to the direction of travel.
        m_TravelDirLocal = Quaternion.Euler(0.0f, -Mathf.Rad2Deg * m_Theta, 0.0f) * Vector3.forward;

        m_DebugTravelDir.rotation = Quaternion.LookRotation(TravelDirWorld);
        m_DebugTravelDir.position = transform.position;
    }
}
