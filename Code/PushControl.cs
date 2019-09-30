using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushControl : MonoBehaviour
{
    [SerializeField]
    private float m_PushSpeed = 10.0f;

    [SerializeField]
    private float m_GainSideSlide = 10.0f;

    [SerializeField]
    private float m_GainPush = 10.0f;

    private DirectionOfTravelCalculater m_DirOfTravel;
    private Rigidbody m_Body;

    // Start is called before the first frame update
    void Awake()
    {
        m_DirOfTravel = GetComponent<DirectionOfTravelCalculater>();
        m_Body = GetComponent<Rigidbody>();
    }

    public void Execute()
    {
        var perp = transform.TransformDirection(Quaternion.Euler(0.0f, 90.0f, 0.0f) * m_DirOfTravel.TravelDirLocal);
        var actualSpeedPerp = Vector3.Dot(m_Body.velocity, perp);
        m_Body.AddForce(-actualSpeedPerp * m_GainSideSlide * perp);

        var travelWorld = m_DirOfTravel.TravelDirWorld;
        var acualSpeedTravelDir = Vector3.Dot(m_Body.velocity, travelWorld);
        m_Body.AddForce((m_PushSpeed - acualSpeedTravelDir) * m_GainPush * travelWorld);
    }
}
