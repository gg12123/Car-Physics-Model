using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidMarkCreate : MonoBehaviour
{
    [SerializeField]
    private float m_CreateThreshold = 1.0f;

    private TrailRenderer m_Ren;
    private Rigidbody m_Body;

    // Start is called before the first frame update
    void Awake()
    {
        m_Ren = GetComponent<TrailRenderer>();
        m_Body = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var sideSlip = Mathf.Abs(Vector3.Dot(m_Body.GetPointVelocity(transform.position), transform.forward));
        m_Ren.enabled = sideSlip >= m_CreateThreshold;
    }
}
