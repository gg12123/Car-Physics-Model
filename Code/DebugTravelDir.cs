using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTravelDir : MonoBehaviour
{
    [SerializeField]
    private Transform m_TravelDir;

    private Rigidbody m_Body;

    void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_TravelDir.position = transform.position;
        m_TravelDir.rotation = Quaternion.LookRotation(m_Body.velocity);
    }
}
